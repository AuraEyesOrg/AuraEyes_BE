using API.Hubs;
using API.Middleware;
using API.Services;
using Application;
using Application.Common.Interfaces;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure;
using Infrastructure.Services;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text.Json.Serialization;

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
        ? new[] { "http://localhost:5173", "http://localhost:4173", "http://localhost:3000" }
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

// Register SignalR hub service for notification broadcasting
builder.Services.AddScoped<INotificationHubService, NotificationHubService>();

// Hangfire - Background job processing
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(
            builder.Configuration.GetConnectionString("DefaultConnection"))));
builder.Services.AddHangfireServer(options =>
{
    // Limit workers to prevent Supabase connection pool exhaustion (MaxClientsInSessionMode)
    options.WorkerCount = 2;
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

// Note: Static files are stored in S3, not wwwroot
// app.UseStaticFiles(); // Removed - using S3 for file storage

app.UseCors("FrontendCors");

// Add authentication before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map SignalR hubs for real-time notifications
app.MapHub<NotificationHub>("/hubs/notifications");

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

// Register recurring jobs
var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
recurringJobManager.AddOrUpdate<DailyQuotaResetJob>(
    "daily-quota-reset",
    job => job.ExecuteAsync(),
    quotaResetCron,
    new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

var slotMaintenanceCron = Environment.GetEnvironmentVariable("HANGFIRE_SLOT_MAINTENANCE_CRON");
if (string.IsNullOrWhiteSpace(slotMaintenanceCron))
{
    slotMaintenanceCron = "*/5 * * * *";
}

recurringJobManager.AddOrUpdate<SlotMaintenanceJob>(
    "slot-maintenance-expire-unused",
    job => job.ExpireUnusedSlotsAsync(CancellationToken.None),
    slotMaintenanceCron,
    new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

app.Run();
