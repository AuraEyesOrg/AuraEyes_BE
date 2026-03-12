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

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Health Checks
builder.Services.AddHealthChecks();

// Hangfire - Background job processing
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(
            builder.Configuration.GetConnectionString("DefaultConnection"))));
builder.Services.AddHangfireServer();

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

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

// Note: Static files are stored in S3, not wwwroot
// app.UseStaticFiles(); // Removed - using S3 for file storage

app.UseCors("AllowAll");

// Add authentication before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

// Hangfire Dashboard (development only for security)
if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire");
}

// Register recurring jobs
RecurringJob.AddOrUpdate<DailyQuotaResetJob>(
    "daily-quota-reset",
    job => job.ExecuteAsync(),
    "0 0 * * *", // 00:00 UTC = 07:00 AM Vietnam
    new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

app.Run();
