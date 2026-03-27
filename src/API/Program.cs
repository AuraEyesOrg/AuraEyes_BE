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

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpClient();

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
});

// Register SignalR hub service for notification,chat broadcasting
builder.Services.AddScoped<INotificationHubService, NotificationHubService>();
builder.Services.AddScoped<IChatHubService, ChatHubService>();
builder.Services.AddSingleton<IUserIdProvider, SignalRUserIdProvider>();

var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection is not configured.");

var hangfireConnectionBuilder = new NpgsqlConnectionStringBuilder(defaultConnection)
{
    // Use a tiny dedicated pool for Hangfire to avoid saturating Supabase session pool.
    MaxPoolSize = 5,
    MinPoolSize = 0
};

var enableHangfireServer = builder.Configuration.GetValue<bool?>("Hangfire:ServerEnabled")
    ?? !builder.Environment.IsDevelopment();

var hangfireWorkerCount = builder.Configuration.GetValue<int?>("Hangfire:WorkerCount") ?? 1;

// Hangfire - Background job processing
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(hangfireConnectionBuilder.ConnectionString)));

if (enableHangfireServer)
{
    builder.Services.AddHangfireServer(options =>
    {
        // Keep worker count very low when using Supabase pooled connection.
        options.WorkerCount = Math.Max(1, hangfireWorkerCount);
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

var rateLimitingSection = builder.Configuration.GetSection("RateLimiting");
var readPermitLimit = rateLimitingSection.GetValue<int?>("ReadPermitLimit")
    ?? rateLimitingSection.GetValue<int?>("GlobalPermitLimit")
    ?? 240;
var readWindowSeconds = rateLimitingSection.GetValue<int?>("ReadWindowSeconds")
    ?? rateLimitingSection.GetValue<int?>("GlobalWindowSeconds")
    ?? 60;
var readQueueLimit = rateLimitingSection.GetValue<int?>("ReadQueueLimit")
    ?? rateLimitingSection.GetValue<int?>("GlobalQueueLimit")
    ?? 0;

var writePermitLimit = rateLimitingSection.GetValue<int?>("WritePermitLimit")
    ?? rateLimitingSection.GetValue<int?>("GlobalPermitLimit")
    ?? 80;
var writeWindowSeconds = rateLimitingSection.GetValue<int?>("WriteWindowSeconds")
    ?? rateLimitingSection.GetValue<int?>("GlobalWindowSeconds")
    ?? 60;
var writeQueueLimit = rateLimitingSection.GetValue<int?>("WriteQueueLimit")
    ?? rateLimitingSection.GetValue<int?>("GlobalQueueLimit")
    ?? 0;

var sensitivePermitLimit = rateLimitingSection.GetValue<int?>("SensitivePermitLimit")
    ?? rateLimitingSection.GetValue<int?>("AuthPermitLimit")
    ?? 8;
var sensitiveWindowSeconds = rateLimitingSection.GetValue<int?>("SensitiveWindowSeconds")
    ?? rateLimitingSection.GetValue<int?>("AuthWindowSeconds")
    ?? 60;
var sensitiveQueueLimit = rateLimitingSection.GetValue<int?>("SensitiveQueueLimit")
    ?? rateLimitingSection.GetValue<int?>("AuthQueueLimit")
    ?? 0;

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
            ? sensitivePermitLimit
            : isReadRequest
                ? readPermitLimit
                : writePermitLimit;

        var windowSeconds = isSensitiveEndpoint
            ? sensitiveWindowSeconds
            : isReadRequest
                ? readWindowSeconds
                : writeWindowSeconds;

        var queueLimit = isSensitiveEndpoint
            ? sensitiveQueueLimit
            : isReadRequest
                ? readQueueLimit
                : writeQueueLimit;

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

        await Infrastructure.Services.DatabaseSeeder.SeedAsync(context, userManager, roleManager, seederLogger);
        Log.Information("Database seeding completed successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while seeding the database");
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
app.UseMiddleware<RequestLoggingMiddleware>();

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

app.MapHealthChecks("/health");

// Hangfire Dashboard (development only for security)
if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire");
}

var defaultQuotaResetCron = app.Environment.IsDevelopment()
    ? "*/2 * * * *"
    : "0 0 * * *";

var quotaResetCron = Environment.GetEnvironmentVariable("HANGFIRE_DAILY_QUOTA_RESET_CRON");
if (string.IsNullOrWhiteSpace(quotaResetCron))
{
    quotaResetCron = defaultQuotaResetCron;
}

var slotMaintenanceCron = Environment.GetEnvironmentVariable("HANGFIRE_SLOT_MAINTENANCE_CRON");
if (string.IsNullOrWhiteSpace(slotMaintenanceCron))
{
    slotMaintenanceCron = "*/5 * * * *";
}

if (enableHangfireServer)
{
    // Register recurring jobs
    var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
    recurringJobManager.AddOrUpdate<DailyQuotaResetJob>(
        "daily-quota-reset",
        job => job.ExecuteAsync(),
        quotaResetCron,
        new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

    recurringJobManager.AddOrUpdate<SlotMaintenanceJob>(
        "slot-maintenance-expire-unused",
        job => job.ExpireUnusedSlotsAsync(CancellationToken.None),
        slotMaintenanceCron,
        new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });
}
else
{
    Log.Warning("Hangfire server is disabled. Recurring jobs are not running in this environment.");
}

app.Run();
