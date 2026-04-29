using Application.Common.Constants;
using Domain.Entities.Authorization;
using Domain.Entities.Users;
using Domain.Enums;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Domain.Entities.Scheduling;
using Domain.Entities.Screening;
using Domain.Entities.Consultation;
using System.Text.Json;

namespace Infrastructure.Services;

/// <summary>
/// Database seeder for initial data.
/// Creates default roles and default user accounts.
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IConfiguration configuration,
        ILogger? logger = null)
    {
        // Apply pending migrations (async to avoid thread-pool starvation)
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            logger?.LogInformation("Applying {Count} pending migration(s)...", pendingMigrations.Count());
            var sw = System.Diagnostics.Stopwatch.StartNew();
            await context.Database.MigrateAsync();
            sw.Stop();
            logger?.LogInformation("Migrations applied successfully in {ElapsedMs}ms", sw.ElapsedMilliseconds);
        }
        else
        {
            logger?.LogInformation("No pending migrations. Database schema is up to date.");
        }

        logger?.LogInformation("Ensuring initial data exists...");

        // Step 1: Seed roles first (idempotent)
        await SeedRolesAsync(roleManager, logger);

        // Step 2: Seed default accounts (AspNetUsers + AspNetUserRoles) (idempotent)
        await SeedDefaultAccountsAsync(userManager, configuration, logger);

        // Step 3: Seed domain entities (Ophthalmologist, Patient, etc.) (idempotent)
        await SeedDomainEntitiesAsync(context, userManager, logger);

        // Step 4: Seed permissions + default role assignments
        await SeedPermissionsAsync(context, roleManager, logger);

        // Step 4.5: Seed specific permissions for staff sub-roles (Receptionist, etc.)
        await SeedStaffSubRolePermissionsAsync(context, userManager, logger);

        // Step 5: Seed test data for clinic queue (Cashier page) (idempotent)
        await SeedClinicQueueTestDataAsync(context, userManager, logger);

        logger?.LogInformation("Seeding completed successfully.");
    }

    private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager, ILogger? logger)
    {
        logger?.LogInformation("Ensuring Identity roles exist in AspNetRoles table...");

        foreach (var roleName in Roles.All)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var role = new ApplicationRole(roleName)
            {
                Description = GetRoleDescription(roleName)
            };

            var result = await roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                logger?.LogInformation("âœ“ Created role: {RoleName} â†’ AspNetRoles", roleName);
            }
            else
            {
                logger?.LogError("âœ— Failed to create role {RoleName}: {Errors}",
                    roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        logger?.LogInformation("Role verification completed.");
    }

    private static string GetRoleDescription(string roleName)
    {
        return roleName switch
        {
            Roles.Patient => "End users who use the retinal screening service",
            Roles.Ophthalmologist => "Medical professionals who review screenings and provide diagnoses",
            Roles.ClinicStaff => "Clinic employees (Receptionist, Coordinator, Cashier) managing clinic operations",
            Roles.SystemAdmin => "Clinic Owner â€” full system access and administration",
            _ => roleName
        };
    }

    private static async Task SeedDefaultAccountsAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger? logger)
    {
        logger?.LogInformation("Seeding default user accounts into AspNetUsers and AspNetUserRoles...");

        var defaultAccounts = new List<(string Email, string Password, string Role, string FullName)>
        {
            // Clinic Owner
            (configuration["Swagger:Username"] ?? "systemadmin@auraeyes.vn",
             configuration["Swagger:Password"] ?? "Admin@123$",
             Roles.SystemAdmin, "Clinic Owner (System Admin)"),

            // Ophthalmologist
            ("doctor@auraeyes.vn", "Doctor@123$", Roles.Ophthalmologist, "BS. Nguyen Van An"),

            // Clinic Staff
            ("receptionist@auraeyes.vn", "Staff@123$", Roles.ClinicStaff, "Tran Thi Binh - Receptionist"),
            ("coordinator@auraeyes.vn",  "Staff@123$", Roles.ClinicStaff, "Le Van Ca - Coordinator"),
            ("cashier@auraeyes.vn",      "Staff@123$", Roles.ClinicStaff, "Pham Thi Dung - Cashier"),

            // Patient
            ("patient@auraeyes.vn", "Patient@123$", Roles.Patient, "Nguyen Van Em")
        };

        if (string.IsNullOrWhiteSpace(defaultAccounts[0].Email) || string.IsNullOrWhiteSpace(defaultAccounts[0].Password))
        {
            logger?.LogWarning("Swagger credentials are missing. System Admin account will not be seeded.");
            defaultAccounts.RemoveAt(0);
        }

        foreach (var (email, password, role, fullName) in defaultAccounts)
        {
            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                if (!await userManager.IsInRoleAsync(existingUser, role))
                {
                    var roleResult = await userManager.AddToRoleAsync(existingUser, role);
                    if (roleResult.Succeeded)
                        logger?.LogInformation("âœ“ Assigned missing role {Role} to existing user {Email} â†’ AspNetUserRoles", role, email);
                    else
                        logger?.LogError("âœ— Failed to assign missing role {Role} to existing user {Email}: {Errors}",
                            role, email, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
                continue;
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await userManager.CreateAsync(user, password);

            if (createResult.Succeeded)
            {
                logger?.LogInformation("âœ“ Created user: {Email} â†’ AspNetUsers", email);

                var roleResult = await userManager.AddToRoleAsync(user, role);

                if (roleResult.Succeeded)
                    logger?.LogInformation("âœ“ Assigned role {Role} to {Email} â†’ AspNetUserRoles", role, email);
                else
                    logger?.LogError("âœ— Failed to assign role {Role} to user {Email}: {Errors}",
                        role, email, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }
            else
            {
                logger?.LogError("âœ— Failed to create user {Email}: {Errors}",
                    email, string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }
        }

        logger?.LogInformation("Default account seeding completed. Total accounts created: {Count}", defaultAccounts.Count);
    }

    private static async Task SeedDomainEntitiesAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger? logger)
    {
        logger?.LogInformation("Seeding domain entities (Ophthalmologist, ClinicStaff, Patient, Schedules)...");

        // Step 1: Seed Ophthalmologist entity
        var ophthalmologistUser = await userManager.FindByEmailAsync("doctor@auraeyes.vn");
        Guid ophthalmologistId = Guid.Empty;

        if (ophthalmologistUser == null)
        {
            logger?.LogWarning("Ophthalmologist user not found. Skipping ophthalmologist entity seeding.");
        }
        else
        {
            var existingOphth = await context.Ophthalmologists
                .FirstOrDefaultAsync(o => o.UserId == ophthalmologistUser.Id);

            if (existingOphth == null)
            {
                var ophthalmologist = new Ophthalmologist(
                    userId: ophthalmologistUser.Id,
                    bio: "Bác sĩ chuyên khoa mắt với kinh nghiệm trong lĩnh vực sàng lọc bệnh võng mạc.",
                    phone: "+84123456789",
                    licenseUrl: null,
                    degreeUrl: null
                );

                await context.Ophthalmologists.AddAsync(ophthalmologist);
                await context.SaveChangesAsync();
                ophthalmologistId = ophthalmologist.Id;

                logger?.LogInformation("âœ“ Created ophthalmologist profile for {Email} â†’ Ophthalmologists table",
                    ophthalmologistUser.Email);
            }
            else
            {
                ophthalmologistId = existingOphth.Id;
                logger?.LogInformation("Ophthalmologist profile already exists. Skipping.");
            }
        }

        // Step 2: Seed ClinicStaff profiles
        var staffAccounts = new[]
        {
            ("receptionist@auraeyes.vn", new[] { ClinicStaffRole.Receptionist }),
            ("coordinator@auraeyes.vn",  new[] { ClinicStaffRole.Coordinator }),
            ("cashier@auraeyes.vn",      new[] { ClinicStaffRole.Cashier }),
        };

        foreach (var (email, subRoles) in staffAccounts)
        {
            var staffUser = await userManager.FindByEmailAsync(email);
            if (staffUser == null)
            {
                logger?.LogWarning("ClinicStaff user {Email} not found. Skipping.", email);
                continue;
            }

            var existingStaff = await context.ClinicStaffs
                .FirstOrDefaultAsync(s => s.UserId == staffUser.Id);

            if (existingStaff == null)
            {
                var clinicStaff = new ClinicStaff(staffUser.Id, subRoles, department: "Clinic Operations");
                await context.ClinicStaffs.AddAsync(clinicStaff);
                logger?.LogInformation("âœ“ Created ClinicStaff profile for {Email} [{SubRoles}] â†’ ClinicStaffs table",
                    email, string.Join(",", subRoles.Select(r => r.ToString())));
            }
            else
            {
                logger?.LogInformation("ClinicStaff profile for {Email} already exists. Skipping.", email);
            }
        }

        await context.SaveChangesAsync();

        // Step 3: Seed Patient entity
        var patientUser = await userManager.FindByEmailAsync("patient@auraeyes.vn");
        if (patientUser != null)
        {
            var p = await context.Patients.FirstOrDefaultAsync(pat => pat.UserId == patientUser.Id);
            if (p != null)
            {
                var existingVisits = await context.PatientVisits.Where(v => v.PatientId == p.Id).ToListAsync();
                context.PatientVisits.RemoveRange(existingVisits);
                await context.SaveChangesAsync();
                logger?.LogInformation("Cleared existing visits for patient@auraeyes.vn to ensure fresh test data.");
            }
        }

        if (patientUser == null)
        {
            logger?.LogWarning("Patient user not found. Skipping patient entity seeding.");
        }
        else
        {
            var existingPatient = await context.Patients
                .FirstOrDefaultAsync(p => p.UserId == patientUser.Id);

            if (existingPatient == null)
            {
                var patient = Patient.CreateRegistered(userId: patientUser.Id);
                await context.Patients.AddAsync(patient);
                await context.SaveChangesAsync();

                logger?.LogInformation("âœ“ Created patient profile for {Email} â†’ Patients table",
                    patientUser.Email);
            }
            else
            {
                logger?.LogInformation("Patient profile already exists. Skipping.");
            }
        }

        logger?.LogInformation("Domain entity seeding completed.");
    }



    private static async Task SeedPermissionsAsync(
        ApplicationDbContext context,
        RoleManager<ApplicationRole> roleManager,
        ILogger? logger)
    {
        logger?.LogInformation("Seeding permissions (idempotent)...");

        var existingNames = await context.Permissions
            .Select(p => p.Name)
            .ToListAsync();

        var existingSet = new HashSet<string>(existingNames, StringComparer.OrdinalIgnoreCase);
        var newPermissions = new List<Permission>();

        foreach (var def in Permissions.All)
        {
            if (existingSet.Contains(def.Name))
                continue;

            newPermissions.Add(new Permission(def.Name, def.DisplayName, def.Description, def.Category));
            logger?.LogInformation("  âœ“ New permission: [{Category}] {Name}", def.Category, def.Name);
        }

        if (newPermissions.Count > 0)
        {
            await context.Permissions.AddRangeAsync(newPermissions);
            await context.SaveChangesAsync();
            logger?.LogInformation("Seeded {Count} new permissions.", newPermissions.Count);
        }

        var allPermissions = await context.Permissions
            .ToDictionaryAsync(p => p.Name, StringComparer.OrdinalIgnoreCase);

        int assignmentsCreated = 0;

        foreach (var (roleName, permissionNames) in Permissions.DefaultRolePermissions)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                logger?.LogWarning("  Role '{Role}' not found â€” skipping its permission assignments.", roleName);
                continue;
            }

            var existingRolePermissionIds = await context.RolePermissions
                .IgnoreQueryFilters()
                .Where(rp => rp.RoleId == role.Id)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            var existingRolePermSet = new HashSet<Guid>(existingRolePermissionIds);

            foreach (var permName in permissionNames)
            {
                if (!allPermissions.TryGetValue(permName, out var permission))
                {
                    logger?.LogWarning("  Permission '{Perm}' not found â€” skipping.", permName);
                    continue;
                }

                if (existingRolePermSet.Contains(permission.Id))
                    continue;

                await context.RolePermissions.AddAsync(new RolePermission(role.Id, permission.Id));
                assignmentsCreated++;
            }
        }

        if (assignmentsCreated > 0)
        {
            await context.SaveChangesAsync();
            logger?.LogInformation("Seeded {Count} new role-permission assignments.", assignmentsCreated);
        }

        logger?.LogInformation("Permission seeding completed.");
    }

    private static async Task SeedStaffSubRolePermissionsAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger? logger)
    {
        logger?.LogInformation("Seeding sub-role specific permissions for clinic staff users...");

        var staffAssignments = new[]
        {
            ("receptionist@auraeyes.vn", Permissions.ReceptionistExtras),
            ("coordinator@auraeyes.vn",  Permissions.CoordinatorExtras),
            ("cashier@auraeyes.vn",      Permissions.CashierExtras)
        };

        var allPermissions = await context.Permissions.ToDictionaryAsync(p => p.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var (email, extras) in staffAssignments)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) continue;

            var existingUserPerms = await context.UserPermissions
                .Where(up => up.UserId == user.Id)
                .Select(up => up.PermissionId)
                .ToListAsync();

            var existingSet = new HashSet<Guid>(existingUserPerms);

            foreach (var permName in extras)
            {
                if (!allPermissions.TryGetValue(permName, out var permission))
                {
                    logger?.LogWarning("Permission '{Perm}' not found for user {Email}.", permName, email);
                    continue;
                }

                if (existingSet.Contains(permission.Id))
                    continue;

                await context.UserPermissions.AddAsync(new UserPermission(user.Id, permission.Id, isGranted: true));
            }
        }

        await context.SaveChangesAsync();
        logger?.LogInformation("Clinic staff sub-role permission seeding completed.");
    }

    private static async Task SeedClinicQueueTestDataAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger? logger)
    {
        logger?.LogInformation("Seeding test data for clinic queue (Cashier page)...");

        var doctorUser = await userManager.FindByEmailAsync("doctor@auraeyes.vn");
        var patientUser = await userManager.FindByEmailAsync("patient@auraeyes.vn");

        if (doctorUser == null || patientUser == null)
        {
            logger?.LogWarning("Doctor or Patient user not found. Skipping clinic queue seeding.");
            return;
        }

        var ophthalmologist = await context.Ophthalmologists.FirstOrDefaultAsync(o => o.UserId == doctorUser.Id);
        var patient = await context.Patients.FirstOrDefaultAsync(p => p.UserId == patientUser.Id);

        if (ophthalmologist == null || patient == null)
        {
            logger?.LogWarning("Ophthalmologist or Patient profile not found. Skipping clinic queue seeding.");
            return;
        }

        // Idempotency check: If we already have a slot for this doctor at this time today, skip
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var startTime = new TimeOnly(14, 0);
        var endTime = new TimeOnly(14, 30);
        
        var existingSlot = await context.AppointmentSlots
            .AnyAsync(s => s.OphthalId == ophthalmologist.Id && s.Date == today && s.StartTime == startTime);

        if (existingSlot)
        {
            logger?.LogInformation("Clinic queue test data (AppointmentSlot) already exists for today. Skipping.");
            return;
        }

        // 1. Create AI Screening
        var aiScreening = new AiScreening(patient.Id, "AuraEyes-AI-v1.0");
        aiScreening.Process("{\"result\": \"High risk of AMD\", \"confidence\": 0.92}");
        await context.AiScreenings.AddAsync(aiScreening);
        await context.SaveChangesAsync();

        // 2. Create Consultation Session
        var session = ConsultationSession.CreateVerification(patient.Id, aiScreening.Id, 200000, ophthalmologist.Id);
        
        await context.ConsultationSessions.AddAsync(session);
        await context.SaveChangesAsync();

        // 3. Create Medical Diagnosis
        var prescriptionData = new
        {
            DiagnosisCode = "H35.30",
            CodingSystem = "ICD-10",
            ClinicalFindings = "Cháº©n Ä‘oÃ¡n xÃ¡c Ä‘á»‹nh: ThoÃ¡i hÃ³a hoÃ ng Ä‘iá»ƒm tuá»•i giÃ  (AMD) thá»ƒ khÃ´.",
            SeverityLevel = "Moderate",
            Recommendations = "Ä eo kÃ­nh rÃ¢m khi ra ngoÃ i trá» i.",
            FollowUpDate = DateTime.UtcNow.AddMonths(2)
        };

        string jsonSnapshot = "DIAGNOSIS_SNAPSHOT_JSON::" + JsonSerializer.Serialize(prescriptionData);

        var diagnosis = new MedicalDiagnosis(
            aiScreeningId: aiScreening.Id,
            doctorId: ophthalmologist.Id,
            consultationSessionId: session.Id,
            diagnosisCode: "H35.30",
            codingSystem: "ICD-10",
            clinicalFindings: prescriptionData.ClinicalFindings,
            severityLevel: "Moderate",
            lifestyleAdvice: jsonSnapshot,
            finalizedAt: DateTime.UtcNow
        );
        diagnosis.Confirm();

        await context.MedicalDiagnoses.AddAsync(diagnosis);
        await context.SaveChangesAsync();

        logger?.LogInformation("âœ“ Successfully seeded clinic queue test data for {Email}", patientUser.Email);
    }
}
