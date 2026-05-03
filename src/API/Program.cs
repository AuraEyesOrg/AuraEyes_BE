using API.Hubs;
using API.Middleware;
using API.Services;
using API.Extensions;
using Application;
using Application.Common.Interfaces;
using Hangfire;
using Infrastructure;
using Infrastructure.Services;
using Infrastructure.Services.Testing;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ── Logging ──────────────────────────────────────────────────────────────
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

// ── Services ─────────────────────────────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddMonitoring(builder.Configuration, builder.Environment.EnvironmentName);

if (builder.Environment.IsEnvironment("Test"))
{
    builder.Services.AddScoped<IEmailService, FakeEmailService>();
    builder.Services.AddScoped<IPayOSService, FakePayOSService>();
}

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? 
    (builder.Environment.IsDevelopment() ? new[] { "http://localhost:5173", "http://localhost:4173", "http://localhost:3000", "https://localhost:5001", "https://n8n.auraeyes.site", "http://localhost:5000" } : Array.Empty<string>());

builder.Services.AddCors(options => options.AddPolicy("FrontendCors", policy => {
    if (allowedOrigins.Length > 0) policy.WithOrigins(allowedOrigins);
    policy.AllowAnyMethod().AllowAnyHeader().AllowCredentials();
}));

builder.Services.AddHealthChecks();
builder.Services.AddSignalR(options => {
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
}).AddJsonProtocol(options => {
    options.PayloadSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddScoped<INotificationHubService, NotificationHubService>();
builder.Services.AddScoped<IChatHubService, ChatHubService>();
builder.Services.AddScoped<IInternalChatHubService, InternalChatHubService>();
builder.Services.AddSingleton<IUserIdProvider, SignalRUserIdProvider>();

builder.Services.AddBackgroundJobs(builder.Configuration, builder.Environment.IsDevelopment());
builder.Services.AddResponseCompression(options => {
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json", "application/xml", "text/plain", "image/svg+xml" });
});

builder.Services.AddOutputCache(options => {
    options.AddBasePolicy(b => b.Expire(TimeSpan.FromSeconds(10)));
    options.AddPolicy("PublicData", b => b.Expire(TimeSpan.FromMinutes(5)).SetVaryByQuery("*"));
});

builder.Services.AddCustomRateLimiter(builder.Configuration);

// ── Pipeline ─────────────────────────────────────────────────────────────
var app = builder.Build();

await SeedDatabaseAsync(app, builder.Configuration);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => {
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
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AURA API v1");
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging(options => options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms");
app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseCors("FrontendCors");
app.UseOutputCache();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/api/hubs/notifications");
app.MapHub<ChatHub>("/api/hubs/chat");
app.MapHub<InternalChatHub>("/api/hubs/internal-chat");
app.MapHealthChecks("/health");
app.UseOpenTelemetryPrometheusScrapingEndpoint();

if (app.Environment.IsDevelopment()) app.UseHangfireDashboard("/hangfire");

RegisterRecurringJobs(app);

app.MapPrometheusScrapingEndpoint();
app.Run();

// ── Helper Methods ───────────────────────────────────────────────────────

static async Task SeedDatabaseAsync(WebApplication app, IConfiguration configuration)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<Infrastructure.Persistence.ApplicationDbContext>();
        var userManager = services.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<Infrastructure.Identity.ApplicationUser>>();
        var roleManager = services.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<Infrastructure.Identity.ApplicationRole>>();
        var seederLogger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeder");

        await DatabaseSeeder.SeedAsync(context, userManager, roleManager, configuration, seederLogger);
        Log.Information("Database seeding completed successfully");
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "An error occurred during seeding. Startup aborted.");
        throw;
    }
}

static void RegisterRecurringJobs(WebApplication app)
{
    var enableHangfireServer = app.Configuration.GetValue<bool?>("Hangfire:ServerEnabled") ?? !app.Environment.IsDevelopment();
    if (!enableHangfireServer) return;

    var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
    var slotCron = Environment.GetEnvironmentVariable("HANGFIRE_SLOT_MAINTENANCE_CRON") ?? "*/5 * * * *";
    var genCron = Environment.GetEnvironmentVariable("HANGFIRE_FULLTIME_SLOT_GENERATION_CRON") ?? "0 0 * * *";

    recurringJobManager.AddOrUpdate<SlotMaintenanceJob>("slot-maintenance-expire-unused", j => j.ExpireUnusedSlotsAsync(CancellationToken.None), slotCron, new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });
    recurringJobManager.AddOrUpdate<FullTimeSlotGenerationJob>("fulltime-slot-rolling-window", j => j.ExecuteAsync(CancellationToken.None), genCron, new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });
    recurringJobManager.AddOrUpdate<MonthlyLeaveFundJob>("monthly-leave-fund-increment", j => j.IncrementMonthlyLeaveDaysAsync(CancellationToken.None), "0 0 1 * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

    try { CleanupLegacyHangfireJobs(JobStorage.Current.GetMonitoringApi()); }
    catch (Exception ex) { Log.Warning(ex, "Failed to cleanup legacy jobs."); }
}

static void CleanupLegacyHangfireJobs(Hangfire.Storage.IMonitoringApi monitoringApi)
{
    var failedJobs = monitoringApi.FailedJobs(0, 100);
    foreach (var failed in failedJobs)
    {
        var error = failed.Value.ExceptionDetails + failed.Value.ExceptionMessage;
        if (error.Contains("Infrastructure.Services.FullTimeSlotGenerationJob") || error.Contains("target method was not found"))
            BackgroundJob.Delete(failed.Key);
    }
}
