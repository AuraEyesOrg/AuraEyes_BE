using System.Text;
using Application.Common.Models;
using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Screenings.Interfaces;
using Application.Scheduling.ScheduleTemplates.Interfaces;
using Application.SystemAdmin.Interfaces;
using Application.SystemSettings.Interfaces;
using Domain.Common;
using Domain.Repositories;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Persistence.Queries;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Infrastructure.Identity.Authorization;


namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register audit interceptor
        services.AddScoped<AuditInterceptor>();

        // Register MediatR handlers that live in this assembly (query handlers, etc.)
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        // Database configuration
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

            // Add audit interceptor for automatic audit logging (FR-43)
            options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
        });

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());

        // JWT Settings
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.SectionName, jwtSettings);
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        // SMTP Settings
        services.Configure<SmtpSettings>(configuration.GetSection(SmtpSettings.SectionName));

        // Rate Limiting Settings
        services.Configure<RateLimitingSettings>(configuration.GetSection(RateLimitingSettings.SectionName));

        // Admin notification settings
        services.Configure<AdminNotificationSettings>(configuration.GetSection(AdminNotificationSettings.SectionName));

        // Cloudinary Settings
        services.Configure<CloudinarySettings>(configuration.GetSection(CloudinarySettings.SectionName));

        // Supabase Storage Settings (Keep for potential legacy needs)
        services.Configure<SupabaseStorageSettings>(configuration.GetSection(SupabaseStorageSettings.SectionName));

        // Google Meet Settings
        services.Configure<GoogleMeetSettings>(configuration.GetSection(GoogleMeetSettings.SectionName));

        // Google AI Studio Settings
        services.Configure<GoogleAiStudioSettings>(configuration.GetSection(GoogleAiStudioSettings.SectionName));

        // Google Auth Settings (for Google Login)
        services.Configure<GoogleAuthSettings>(configuration.GetSection(GoogleAuthSettings.SectionName));

        // BetterStack settings
        services.Configure<BetterStackSettings>(configuration.GetSection(BetterStackSettings.SectionName));

        // ASP.NET Core Identity configuration
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 4;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

            // SignIn settings
            options.SignIn.RequireConfirmedEmail = true;
            options.SignIn.RequireConfirmedAccount = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Configure JWT Authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = true; // Set to false for development only
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ValidateIssuer = jwtSettings.ValidateIssuer,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = jwtSettings.ValidateAudience,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = jwtSettings.ValidateLifetime,
                ClockSkew = TimeSpan.FromSeconds(jwtSettings.ClockSkewSeconds)
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken)
                        && path.StartsWithSegments("/api/hubs"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers["Token-Expired"] = "true";
                    }
                    return Task.CompletedTask;
                }
            };
        });

        // Configure Authorization Policies
        var authBuilder = services.AddAuthorizationBuilder()
            .AddPolicy(Policies.Authenticated, policy => policy.RequireAuthenticatedUser())
            .AddPolicy(Policies.PatientOnly, policy => policy.RequireRole(Roles.Patient))
            .AddPolicy(Policies.OphthalmologistOnly, policy => policy.RequireRole(Roles.Ophthalmologist))
            .AddPolicy(Policies.ClinicStaffOnly, policy => policy.RequireRole(Roles.ClinicStaff))
            .AddPolicy(Policies.ClinicalTeam, policy => policy.RequireRole(Roles.Ophthalmologist, Roles.ClinicStaff, Roles.SystemAdmin))
            .AddPolicy(Policies.SystemAdminOnly, policy => policy.RequireRole(Roles.SystemAdmin))
            .AddPolicy(Policies.AdminsOnly, policy => policy.RequireRole(Roles.Admins))
            .AddPolicy(Policies.MedicalStaff, policy => policy.RequireRole(Roles.Medical))
            .AddPolicy(Policies.VerifiedOphthalmologist, policy =>
                policy.RequireRole(Roles.Ophthalmologist));

        // Register Permission-based policies dynamically from Permissions constant class
        foreach (var prop in typeof(Permissions).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy))
        {
            if (prop.IsLiteral && !prop.IsInitOnly && prop.FieldType == typeof(string))
            {
                var permissionValue = (string)prop.GetValue(null)!;
                authBuilder.AddPolicy(permissionValue, policy => 
                    policy.Requirements.Add(new PermissionRequirement(permissionValue)));
            }
        }

        services.AddScoped<IAuthorizationHandler, PermissionHandler>();

        // Register repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IOphthalmologistRepository, OphthalmologistRepository>();
        services.AddScoped<IClinicStaffRepository, ClinicStaffRepository>();
        services.AddScoped<IOphthalmologistLeaveRequestRepository, OphthalmologistLeaveRequestRepository>();
        services.AddScoped<IScheduleTemplateRepository, ScheduleTemplateRepository>();
        services.AddScoped<IAppointmentSlotRepository, AppointmentSlotRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<ISlotAssignmentRepository, SlotAssignmentRepository>();
        services.AddScoped<IPatientVisitRepository, PatientVisitRepository>();
        services.AddScoped<IHealthRoadmapRepository, HealthRoadmapRepository>();
        services.AddScoped<IConsultationSessionRepository, ConsultationSessionRepository>();
        services.AddScoped<IClinicFeedbackRepository, ClinicFeedbackRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IAiScreeningQuery, AiScreeningQuery>();
        services.AddScoped<IOphthalmologistScreeningsReadRepository, OphthalmologistScreeningsReadRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
        services.AddScoped<ILeavePolicyRepository, LeavePolicyRepository>();

        // Register Identity Services
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IAuthService, AuthService>();

        // Register other services
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddScoped<IClinicVisitService, ClinicVisitService>();
        services.AddScoped<IFileStorageService, CloudinaryStorageService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IGoogleMeetService, GoogleMeetService>();
        services.AddScoped<IAdminQueryService, AdminQueryService>();
        services.AddScoped<IDashboardMetricsService, DashboardMetricsService>();
        services.AddScoped<ISystemSettingService, SystemSettingService>();
        services.AddScoped<IPatientScreeningPdfService, PatientScreeningPdfService>();
        services.AddScoped<IMedicalRecordPdfService, MedicalRecordPdfService>();
        services.AddSingleton<IAiAssetBaseUrlProvider, AiAssetBaseUrlProvider>();
        services.AddSingleton<IBetterStackHeartbeatService, BetterStackHeartbeatService>();
        services.AddScoped<IFullTimeSlotGenerationService, FullTimeSlotGenerationService>();

        // Background workers
        services.AddHostedService<SessionReminderWorker>();
        services.AddHostedService<ConsultationStateWorker>();


        // Register Hangfire daily job

        services.AddScoped<SlotMaintenanceJob>();
        services.AddScoped<FullTimeSlotGenerationJob>();
        services.AddScoped<MonthlyLeaveFundJob>();

        // Configure PayOS Settings
        services.Configure<PayOSSettings>(configuration.GetSection(PayOSSettings.SectionName));
        services.AddScoped<IPayOSService, PayOSService>();


        return services;
    }
}
