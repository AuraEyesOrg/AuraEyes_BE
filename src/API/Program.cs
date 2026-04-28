using API.Hubs;
using API.Middleware;
using API.Services;
using Application;
using Application.Common.Interfaces;
using Application.Common.Models;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure;
using Infrastructure.Services;
using Microsoft.AspNetCore.ResponseCompression;
using Infrastructure.Services.Testing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Models;
using Npgsql;
using Serilog;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog.Sinks.Grafana.Loki;
using System.Collections.Generic;
using System;

static string ResolveHangfireSchema(string? configuredSchema, bool isDevelopment)
{
    var rawSchema = configuredSchema;

    if (string.IsNullOrWhiteSpace(rawSchema) && isDevelopment)
    {
        rawSchema = $"hangfire_dev_{Environment.MachineName}";
    }

    if (string.IsNullOrWhiteSpace(rawSchema))
    {
        rawSchema = "hangfire";
    }

    var normalized = new string(
        rawSchema
            .Trim()
            .ToLowerInvariant()
            .Select(ch => char.IsLetterOrDigit(ch) ? ch : '_')
            .ToArray());

    if (string.IsNullOrWhiteSpace(normalized))
    {
        normalized = "hangfire";
    }

    if (char.IsDigit(normalized[0]))
    {
        normalized = $"h_{normalized}";
    }

    return normalized.Length > 63 ? normalized[..63] : normalized;
}

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "AuraEyes.API")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.GrafanaLoki(
        builder.Configuration["LOKI_URL"] ?? "http://loki:3100",
        new List<LokiLabel> { new LokiLabel { Key = "app", Value = "auraeyes-api" } })
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpClient();

// ── OpenTelemetry & Monitoring ─────────────────────────────────────────────
var otelResource = ResourceBuilder.CreateDefault()
    .AddService(
        serviceName: "auraeyes-api",
        serviceInstanceId: System.Environment.MachineName)
    .AddAttributes(new Dictionary<string, object>
    {
        ["deployment.environment"] = builder.Configuration["ASPNETCORE_ENVIRONMENT"] ?? "Production",
        ["service.namespace"] = "AuraEyes"
    });

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .SetResourceBuilder(otelResource)
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation(options => {
            options.SetDbStatementForText = true;
        })
        .AddOtlpExporter(opt => {
            opt.Endpoint = new Uri(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "http://otel-collector:4317");
        }))
    .WithMetrics(metrics => metrics
        .SetResourceBuilder(otelResource)
        .AddRuntimeInstrumentation()
        .AddProcessInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddPrometheusExporter());

if (builder.Environment.IsEnvironment("Test"))
{
    // Replace external integrations with in-process test doubles.
    builder.Services.AddScoped<IEmailService, FakeEmailService>();
    builder.Services.AddScoped<IPayOSService, FakePayOSService>();
}

// Register HttpContextAccessor and CurrentUserService
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Serialize enums as strings (e.g. "Public" instead of 0)
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();

var configuredOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

var allowedOrigins = configuredOrigins ??
    (builder.Environment.IsDevelopment()
        ? new[] { "http://localhost:5173", "http://localhost:4173", "http://localhost:3000", "https://localhost:5001", "https://n8n.auraeyes.site", "https://localhost:5001",
            "http://localhost:5000" }
        : Array.Empty<string>());

// Configure Swagger with JWT Bearer authentication
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AURA - Retinal Screening API",
        Version = "v1",
        Description = "API for AURA Healthcare System - Retinal screening and ophthalmology services",
        Contact = new OpenApiContact
        {
            Name = "AURA Team",
            Email = "support@aura.health"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License"
        }
    });

    // Add JWT Bearer authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = """
            JWT Authorization header using the Bearer scheme.
            
            Enter your token in the text input below.
            
            Example: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
            
            Note: Do NOT include the word 'Bearer' - it will be added automatically.
            """
    });

    // Add global security requirement
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Include XML comments for API documentation
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Add custom operation filter for better documentation
    options.EnableAnnotations();

    // Fix Schema ID collision by using full type name (namespace + class name)
    // This prevents conflicts when same class names exist in different namespaces
    options.CustomSchemaIds(type =>
    {
        var fullName = type.FullName ?? type.Name;
        // Replace nested class '+' with '.'
        return fullName.Replace("+", ".").Replace("[", "Of").Replace("]", "").Replace(",", "").Replace(" ", "");
    });
});

// Add CORS with SignalR support
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendCors", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins);
        }

        policy.AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add Health Checks
builder.Services.AddHealthChecks();

// Add SignalR for real-time notifications
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
})
.AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Register SignalR hub service for notification,chat broadcasting
builder.Services.AddScoped<INotificationHubService, NotificationHubService>();
builder.Services.AddScoped<IChatHubService, ChatHubService>();
builder.Services.AddScoped<IInternalChatHubService, InternalChatHubService>();
builder.Services.AddSingleton<IUserIdProvider, SignalRUserIdProvider>();

var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection is not configured.");

var hangfireConnectionBuilder = new NpgsqlConnectionStringBuilder(defaultConnection)
{
    // Use a tiny dedicated pool for Hangfire to avoid saturating Supabase session pool.
    MaxPoolSize = 5,
    MinPoolSize = 0
};

var configuredHangfireSchema = builder.Configuration["Hangfire:Schema"]
    ?? Environment.GetEnvironmentVariable("HANGFIRE_SCHEMA");
var hangfireSchema = ResolveHangfireSchema(configuredHangfireSchema, builder.Environment.IsDevelopment());
Log.Information("Using Hangfire schema '{HangfireSchema}'", hangfireSchema);

var enableHangfireServer = builder.Configuration.GetValue<bool?>("Hangfire:ServerEnabled")
    ?? !builder.Environment.IsDevelopment();

var hangfireWorkerCount = builder.Configuration.GetValue<int?>("Hangfire:WorkerCount") ?? 1;

// Hangfire - Background job processing
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(
        options => options.UseNpgsqlConnection(hangfireConnectionBuilder.ConnectionString),
        new PostgreSqlStorageOptions
        {
            SchemaName = hangfireSchema
        }));

if (enableHangfireServer)
{
    builder.Services.AddHangfireServer(options =>
    {
        // Keep worker count very low when using Supabase pooled connection.
        options.WorkerCount = Math.Max(1, hangfireWorkerCount);
        options.ServerName = $"{Environment.MachineName}:{Environment.ProcessId}:{hangfireSchema}";
    });
}

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true; // Bắt buộc bật nếu dùng HTTPS
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/json", "application/xml", "text/plain", "image/svg+xml" }); // Chỉ định nén file
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    // Nén mức độ ưu tiên tốc độ để không block CPU server
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    // Nén mức độ ưu tiên tốc độ để không block CPU server
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

// Configure Output Caching
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromSeconds(10)));

    options.AddPolicy("PublicData", builder =>
        builder.Expire(TimeSpan.FromMinutes(5))
               .SetVaryByQuery("*")); // Vary cache by query parameters
});

var rateLimitingSettings = builder.Configuration
    .GetSection(Infrastructure.Settings.RateLimitingSettings.SectionName)
    .Get<Infrastructure.Settings.RateLimitingSettings>() ?? new Infrastructure.Settings.RateLimitingSettings();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter =
                Math.Ceiling(retryAfter.TotalSeconds).ToString();
        }

        var response = ApiResponseFactory.Error("Too many requests. Please retry later.");
        await context.HttpContext.Response.WriteAsJsonAsync(response, cancellationToken: token);
    };

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var path = httpContext.Request.Path;
        var method = httpContext.Request.Method;

        var isSensitiveEndpoint =
            (HttpMethods.IsPost(method) &&
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
              || path.StartsWithSegments("/api/two-factor/recovery-codes", StringComparison.OrdinalIgnoreCase)));

        var isReadRequest = HttpMethods.IsGet(method) || HttpMethods.IsHead(method) || HttpMethods.IsOptions(method);

        var partitionKey = httpContext.User.Identity?.IsAuthenticated == true
            ? $"user:{httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? httpContext.User.Identity.Name ?? "unknown"}"
            : $"ip:{httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown"}";

        var policyKey = isSensitiveEndpoint ? "sensitive" : isReadRequest ? "read" : "write";
        partitionKey = $"{policyKey}:{partitionKey}";

        var permitLimit = isSensitiveEndpoint
            ? rateLimitingSettings.ActualSensitivePermitLimit
            : isReadRequest
                ? rateLimitingSettings.ActualReadPermitLimit
                : rateLimitingSettings.ActualWritePermitLimit;

        var windowSeconds = isSensitiveEndpoint
            ? rateLimitingSettings.ActualSensitiveWindowSeconds
            : isReadRequest
                ? rateLimitingSettings.ActualReadWindowSeconds
                : rateLimitingSettings.ActualWriteWindowSeconds;

        var queueLimit = isSensitiveEndpoint
            ? rateLimitingSettings.ActualSensitiveQueueLimit
            : isReadRequest
                ? rateLimitingSettings.ActualReadQueueLimit
                : rateLimitingSettings.ActualWriteQueueLimit;

        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = permitLimit,
            Window = TimeSpan.FromSeconds(windowSeconds),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = queueLimit,
            AutoReplenishment = true
        });
    });
});

var app = builder.Build();

// Seed domain entities (Organisation, Ophthalmologist, Patient)
// Note: This will skip if roles already exist (idempotent)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<Infrastructure.Persistence.ApplicationDbContext>();
        var userManager = services.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<Infrastructure.Identity.ApplicationUser>>();
        var roleManager = services.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<Infrastructure.Identity.ApplicationRole>>();
        var loggerFactory = services.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>();
        var seederLogger = loggerFactory.CreateLogger("DatabaseSeeder");

        await Infrastructure.Services.DatabaseSeeder.SeedAsync(
            context,
            userManager,
            roleManager,
            builder.Configuration,
            seederLogger);
        Log.Information("Database seeding completed successfully");
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "An error occurred while seeding or migrating the database. Application startup aborted.");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AURA API v1");
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        options.EnableTryItOutByDefault();
        options.DisplayRequestDuration();
    });
}
else
{
    app.UseMiddleware<SwaggerBasicAuthMiddleware>();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AURA API v1");
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        options.DisplayRequestDuration();
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
// app.UseMiddleware<RequestLoggingMiddleware>(); // Thay bằng Serilog Request Logging bên dưới

// Serilog Request Logging: Log chi tiết mọi HTTP request (Method, Path, Status Code, Response Time...)
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

app.UseHttpsRedirection();

app.UseResponseCompression();

app.UseRateLimiter();

app.UseCors("FrontendCors");
app.UseOutputCache();

// Add authentication before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map SignalR hubs for real-time notifications
app.MapHub<NotificationHub>("/api/hubs/notifications");
app.MapHub<ChatHub>("/api/hubs/chat");
app.MapHub<InternalChatHub>("/api/hubs/internal-chat");

app.MapHealthChecks("/health");

// Prometheus metrics endpoint
app.UseOpenTelemetryPrometheusScrapingEndpoint();

// Hangfire Dashboard (development only for security)
if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire");
}





var slotMaintenanceCron = Environment.GetEnvironmentVariable("HANGFIRE_SLOT_MAINTENANCE_CRON");
if (string.IsNullOrWhiteSpace(slotMaintenanceCron))
{
    slotMaintenanceCron = "*/5 * * * *";
}

var fullTimeSlotGenerationCron = Environment.GetEnvironmentVariable("HANGFIRE_FULLTIME_SLOT_GENERATION_CRON");
if (string.IsNullOrWhiteSpace(fullTimeSlotGenerationCron))
{
    // Default: run weekly at 01:00 UTC (every 7 days cadence).
    fullTimeSlotGenerationCron = "0 1 * * 1";
}

var monthlySalaryCron = Environment.GetEnvironmentVariable("HANGFIRE_MONTHLY_SALARY_CRON");
if (string.IsNullOrWhiteSpace(monthlySalaryCron))
{
    monthlySalaryCron = "0 0 5 * *";
}

if (enableHangfireServer)
{
    // Register recurring jobs
    var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
    var legacyRecurringJobIds = new[]
    {
        "monthly-quota-reset",
        "daily-quota-reset",
        "fulltime-slot-generation",
        "full-time-slot-generation",
        "fulltime-slot-generation-job",
        // Remove the current id first to force a clean re-registration payload.
        "fulltime-slot-rolling-window",
        "monthly-salary-payout"
    };

    foreach (var recurringJobId in legacyRecurringJobIds)
    {
        try
        {
            recurringJobManager.RemoveIfExists(recurringJobId);
        }
        catch (Exception ex)
        {
            Log.Warning("Could not remove legacy job {JobId} due to lock or timeout: {Message}", recurringJobId, ex.Message);
        }
    }



    recurringJobManager.AddOrUpdate<SlotMaintenanceJob>(
        "slot-maintenance-expire-unused",
        job => job.ExpireUnusedSlotsAsync(CancellationToken.None),
        slotMaintenanceCron,
        new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

    recurringJobManager.AddOrUpdate<FullTimeSlotGenerationJob>(
        "fulltime-slot-rolling-window",
        job => job.ExecuteAsync(CancellationToken.None),
        fullTimeSlotGenerationCron,
        new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });


    try
    {
        var monitoringApi = JobStorage.Current.GetMonitoringApi();
        const int pageSize = 100;
        var from = 0;
        var deletedCount = 0;

        while (true)
        {
            var failedJobs = monitoringApi.FailedJobs(from, pageSize);
            if (failedJobs.Count == 0)
            {
                break;
            }

            foreach (var failed in failedJobs)
            {
                var jobId = failed.Key;
                var details = failed.Value;

                var errorText = string.Join(
                    " | ",
                    new[]
                    {
                        details.ExceptionType,
                        details.ExceptionMessage,
                        details.ExceptionDetails
                    }.Where(text => !string.IsNullOrWhiteSpace(text)));

                var isIncompatibleLegacyJob =
                    errorText.Contains("Could not load type 'Infrastructure.Services.FullTimeSlotGenerationJob'", StringComparison.OrdinalIgnoreCase)
                    || errorText.Contains("target method was not found", StringComparison.OrdinalIgnoreCase)
                    || errorText.Contains("Hangfire.Common.JobLoadException", StringComparison.OrdinalIgnoreCase)
                    || errorText.Contains("System.TypeLoadException", StringComparison.OrdinalIgnoreCase);

                if (isIncompatibleLegacyJob && BackgroundJob.Delete(jobId))
                {
                    deletedCount++;
                }
            }

            from += pageSize;
        }

        if (deletedCount > 0)
        {
            Log.Warning(
                "Deleted {DeletedCount} incompatible legacy Hangfire failed jobs during startup cleanup.",
                deletedCount);
        }
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Failed to cleanup incompatible legacy Hangfire jobs during startup.");
    }
}
else
{
    Log.Warning("Hangfire server is disabled. Recurring jobs are not running in this environment.");
}

app.MapPrometheusScrapingEndpoint();

app.Run();
