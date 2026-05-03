using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Application.Common.Models;
using Infrastructure.Settings;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Hangfire;
using Hangfire.PostgreSql;
using Npgsql;
using Serilog;

namespace API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "AURA - Retinal Screening API",
                Version = "v1",
                Description = "API for AURA Healthcare System - Retinal screening and ophthalmology services",
                Contact = new OpenApiContact { Name = "AURA Team", Email = "support@aura.health" },
                License = new OpenApiLicense { Name = "MIT License" }
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Enter your token below."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
            if (File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath);

            options.EnableAnnotations();
            options.CustomSchemaIds(type =>
            {
                var fullName = type.FullName ?? type.Name;
                return fullName.Replace("+", ".").Replace("[", "Of").Replace("]", "").Replace(",", "").Replace(" ", "");
            });
        });

        return services;
    }

    public static IServiceCollection AddCustomRateLimiter(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection(RateLimitingSettings.SectionName).Get<RateLimitingSettings>() ?? new RateLimitingSettings();

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, token) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    context.HttpContext.Response.Headers.RetryAfter = Math.Ceiling(retryAfter.TotalSeconds).ToString();

                var response = ApiResponseFactory.Error("Too many requests. Please retry later.");
                await context.HttpContext.Response.WriteAsJsonAsync(response, cancellationToken: token);
            };

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var path = httpContext.Request.Path;
                var method = httpContext.Request.Method;
                var isSensitive = IsSensitiveEndpoint(path, method);
                var isRead = HttpMethods.IsGet(method) || HttpMethods.IsHead(method) || HttpMethods.IsOptions(method);

                var partitionKey = GetPartitionKey(httpContext);
                var policyKey = isSensitive ? "sensitive" : isRead ? "read" : "write";
                partitionKey = $"{policyKey}:{partitionKey}";

                return GetFixedWindowLimiter(partitionKey, isSensitive, isRead, settings);
            });
        });

        return services;
    }

    private static bool IsSensitiveEndpoint(PathString path, string method)
    {
        return HttpMethods.IsPost(method) &&
               (path.StartsWithSegments("/api/auth/login", StringComparison.OrdinalIgnoreCase)
                || path.StartsWithSegments("/api/auth/google-login", StringComparison.OrdinalIgnoreCase)
                || path.StartsWithSegments("/api/auth/register", StringComparison.OrdinalIgnoreCase)
                || path.StartsWithSegments("/api/auth/forgot-password", StringComparison.OrdinalIgnoreCase)
                || path.StartsWithSegments("/api/auth/reset-password", StringComparison.OrdinalIgnoreCase)
                || path.StartsWithSegments("/api/auth/resend-confirmation", StringComparison.OrdinalIgnoreCase)
                || path.StartsWithSegments("/api/auth/refresh", StringComparison.OrdinalIgnoreCase)
                || path.StartsWithSegments("/api/two-factor/setup", StringComparison.OrdinalIgnoreCase)
                || path.StartsWithSegments("/api/two-factor/enable", StringComparison.OrdinalIgnoreCase)
                || path.StartsWithSegments("/api/two-factor/disable", StringComparison.OrdinalIgnoreCase)
                || path.StartsWithSegments("/api/two-factor/recovery-codes", StringComparison.OrdinalIgnoreCase));
    }

    private static string GetPartitionKey(HttpContext context)
    {
        return context.User.Identity?.IsAuthenticated == true
            ? $"user:{context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? context.User.Identity.Name ?? "unknown"}"
            : $"ip:{context.Connection.RemoteIpAddress?.ToString() ?? "unknown"}";
    }

    private static RateLimitPartition<string> GetFixedWindowLimiter(string partitionKey, bool isSensitive, bool isRead, RateLimitingSettings settings)
    {
        var permitLimit = isSensitive ? settings.ActualSensitivePermitLimit : isRead ? settings.ActualReadPermitLimit : settings.ActualWritePermitLimit;
        var windowSeconds = isSensitive ? settings.ActualSensitiveWindowSeconds : isRead ? settings.ActualReadWindowSeconds : settings.ActualWriteWindowSeconds;
        var queueLimit = isSensitive ? settings.ActualSensitiveQueueLimit : isRead ? settings.ActualReadQueueLimit : settings.ActualWriteQueueLimit;

        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = permitLimit,
            Window = TimeSpan.FromSeconds(windowSeconds),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = queueLimit,
            AutoReplenishment = true
        });
    }

    public static IServiceCollection AddMonitoring(this IServiceCollection services, IConfiguration configuration, string environmentName)
    {
        var otelResource = ResourceBuilder.CreateDefault()
            .AddService(serviceName: "auraeyes-api", serviceInstanceId: Environment.MachineName)
            .AddAttributes(new Dictionary<string, object>
            {
                ["deployment.environment"] = environmentName,
                ["service.namespace"] = "AuraEyes"
            });

        services.AddOpenTelemetry()
            .WithTracing(tracing => tracing
                .SetResourceBuilder(otelResource)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
                .AddOtlpExporter(opt => opt.Endpoint = new Uri(configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "http://otel-collector:4317")))
            .WithMetrics(metrics => metrics
                .SetResourceBuilder(otelResource)
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddPrometheusExporter());

        return services;
    }

    public static IServiceCollection AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection is not configured.");
        var hangfireConnection = new NpgsqlConnectionStringBuilder(connectionString) { MaxPoolSize = 5, MinPoolSize = 0 }.ConnectionString;

        var configuredSchema = configuration["Hangfire:Schema"] ?? Environment.GetEnvironmentVariable("HANGFIRE_SCHEMA");
        var hangfireSchema = ResolveHangfireSchema(configuredSchema, isDevelopment);

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(hangfireConnection), new PostgreSqlStorageOptions { SchemaName = hangfireSchema }));

        var enableServer = configuration.GetValue<bool?>("Hangfire:ServerEnabled") ?? !isDevelopment;
        if (enableServer)
        {
            var workerCount = configuration.GetValue<int?>("Hangfire:WorkerCount") ?? 1;
            services.AddHangfireServer(options =>
            {
                options.WorkerCount = Math.Max(1, workerCount);
                options.ServerName = $"{Environment.MachineName}:{Environment.ProcessId}:{hangfireSchema}";
            });
        }

        return services;
    }

    private static string ResolveHangfireSchema(string? configuredSchema, bool isDevelopment)
    {
        var rawSchema = configuredSchema ?? (isDevelopment ? $"hangfire_dev_{Environment.MachineName}" : "hangfire");
        var normalized = new string(rawSchema.Trim().ToLowerInvariant().Select(ch => char.IsLetterOrDigit(ch) ? ch : '_').ToArray());
        if (string.IsNullOrWhiteSpace(normalized)) normalized = "hangfire";
        if (char.IsDigit(normalized[0])) normalized = $"h_{normalized}";
        return normalized.Length > 63 ? normalized[..63] : normalized;
    }
}
