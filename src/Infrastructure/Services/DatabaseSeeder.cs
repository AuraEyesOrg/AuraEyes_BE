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

        // Check if any roles exist - only seed if database is completely empty
        var hasRoles = await roleManager.Roles.AnyAsync();

        if (!hasRoles)
        {
            logger?.LogInformation("No roles found in database. Starting initial seed...");

            // Step 1: Seed roles first
            await SeedRolesAsync(roleManager, logger);

            // Step 2: Seed default accounts (AspNetUsers + AspNetUserRoles)
            await SeedDefaultAccountsAsync(userManager, configuration, logger);

            // Step 3: Seed domain entities (Organisation, Ophthalmologist, Patient)
            await SeedDomainEntitiesAsync(context, userManager, logger);
        }
        else
        {
            logger?.LogInformation("Roles already exist. Skipping initial account seed.");
        }

        // Step 4: Ensure System Admin wallet exists with OwnerType = "System"
        await EnsureSystemAdminWalletAsync(context, userManager, logger);

        // Step 5: Seed permissions + default role assignments
        // Idempotent — runs on every startup so new permissions defined in code
        // are automatically added to the database on next deployment.
        await SeedPermissionsAsync(context, roleManager, logger);
    }

    private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager, ILogger? logger)
    {
        logger?.LogInformation("Seeding Identity roles into AspNetRoles table...");

        foreach (var roleName in Roles.All)
        {
            var role = new ApplicationRole(roleName)
            {
                Description = GetRoleDescription(roleName)
            };

            var result = await roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                logger?.LogInformation("✓ Created role: {RoleName} → AspNetRoles", roleName);
            }
            else
            {
                logger?.LogError("✗ Failed to create role {RoleName}: {Errors}",
                    roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        logger?.LogInformation("Role seeding completed. Total roles created: {Count}", Roles.All.Length);
    }

    private static string GetRoleDescription(string roleName)
    {
        return roleName switch
        {
            Roles.Patient => "End users who use the retinal screening service",
            Roles.Ophthalmologist => "Medical professionals who review screenings and provide diagnoses",
            Roles.ClinicStaff => "Clinic employees (Receptionist, Coordinator, Cashier) managing clinic operations",
            Roles.SystemAdmin => "Clinic Owner — full system access and administration",
            _ => roleName
        };
    }

    private static async Task SeedDefaultAccountsAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger? logger)
    {
        logger?.LogInformation("Seeding default user accounts into AspNetUsers and AspNetUserRoles...");

        // Digital Clinic model: SystemAdmin + Ophthalmologist + ClinicStaff + Patient
        var defaultAccounts = new List<(string Email, string Password, string Role, string FullName)>
        {
            // Clinic Owner
            (configuration["Swagger:Username"] ?? "systemadmin@auraeyes.vn",
             configuration["Swagger:Password"] ?? "Admin@123$",
             Roles.SystemAdmin, "Clinic Owner (System Admin)"),

            // Ophthalmologist
            ("doctor@auraeyes.vn", "Doctor@123$", Roles.Ophthalmologist, "BS. Nguyen Van An"),

            // Clinic Staff — three sub-role examples
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
                logger?.LogInformation("✓ Created user: {Email} → AspNetUsers", email);

                var roleResult = await userManager.AddToRoleAsync(user, role);

                if (roleResult.Succeeded)
                    logger?.LogInformation("✓ Assigned role {Role} to {Email} → AspNetUserRoles", role, email);
                else
                    logger?.LogError("✗ Failed to assign role {Role} to user {Email}: {Errors}",
                        role, email, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }
            else
            {
                logger?.LogError("✗ Failed to create user {Email}: {Errors}",
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
        logger?.LogInformation("Seeding domain entities (Ophthalmologist, ClinicStaff, Patient, Wallets, Schedules)...");

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
                    yearsOfExperience: 5,
                    phone: "+84123456789",
                    licenseUrl: null,
                    degreeUrl: null
                );

                await context.Ophthalmologists.AddAsync(ophthalmologist);
                await context.SaveChangesAsync();
                ophthalmologistId = ophthalmologist.Id;

                logger?.LogInformation("✓ Created ophthalmologist profile for {Email} → Ophthalmologists table",
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
                logger?.LogInformation("✓ Created ClinicStaff profile for {Email} [{SubRoles}] → ClinicStaffs table",
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
                var patient = new Patient(userId: patientUser.Id);
                await context.Patients.AddAsync(patient);
                await context.SaveChangesAsync();

                logger?.LogInformation("✓ Created patient profile for {Email} → Patients table",
                    patientUser.Email);
            }
            else
            {
                logger?.LogInformation("Patient profile already exists. Skipping.");
            }
        }

        // Step 4: Seed Wallets for Ophthalmologist and Patient
        await SeedWalletsAsync(context, ophthalmologistUser, patientUser, logger);

        // Step 5: Seed ScheduleTemplate
        await SeedScheduleTemplatesAsync(context, logger);

        logger?.LogInformation("Domain entity seeding completed.");
    }


    private static async Task SeedWalletsAsync(
        ApplicationDbContext context,
        ApplicationUser? ophthalmologistUser,
        ApplicationUser? patientUser,
        ILogger? logger)
    {
        logger?.LogInformation("Seeding wallets...");

        // Seed Ophthalmologist Wallet
        if (ophthalmologistUser != null)
        {
            var existingOphthWallet = await context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == ophthalmologistUser.Id && w.OwnerType == "Ophthalmologist");

            if (existingOphthWallet == null)
            {
                var ophthWallet = new Domain.Entities.Financial.Wallet(
                    userId: ophthalmologistUser.Id,
                    ownerType: "Ophthalmologist",
                    initialBalance: 1000000m // 1 million VND
                );

                await context.Wallets.AddAsync(ophthWallet);
                await context.SaveChangesAsync();
                logger?.LogInformation("✓ Created wallet for Ophthalmologist with 1,000,000 VND → Wallets table");
            }
            else
            {
                logger?.LogInformation("Ophthalmologist wallet already exists. Skipping.");
            }
        }

        // Seed Patient Wallet
        if (patientUser != null)
        {
            var existingPatientWallet = await context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == patientUser.Id && w.OwnerType == "Patient");

            if (existingPatientWallet == null)
            {
                var patientWallet = new Domain.Entities.Financial.Wallet(
                    userId: patientUser.Id,
                    ownerType: "Patient",
                    initialBalance: 5000000m // 5 million VND
                );

                await context.Wallets.AddAsync(patientWallet);
                await context.SaveChangesAsync();
                logger?.LogInformation("✓ Created wallet for Patient with 5,000,000 VND → Wallets table");
            }
            else
            {
                logger?.LogInformation("Patient wallet already exists. Skipping.");
            }
        }

        logger?.LogInformation("Wallet seeding completed.");
    }

    private static async Task EnsureSystemAdminWalletAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger? logger)
    {
        logger?.LogInformation("Ensuring System Admin wallet (OwnerType = System)...");

        var systemAdminUser = (await userManager.GetUsersInRoleAsync(Roles.SystemAdmin))
            .FirstOrDefault();

        if (systemAdminUser == null)
        {
            logger?.LogWarning("No user with role {Role} found. Skipping System wallet seeding.", Roles.SystemAdmin);
            return;
        }

        var existingWallet = await context.Wallets
            .FirstOrDefaultAsync(w => w.UserId == systemAdminUser.Id);

        if (existingWallet == null)
        {
            var systemWallet = new Domain.Entities.Financial.Wallet(
                userId: systemAdminUser.Id,
                ownerType: "System",
                initialBalance: 0m
            );

            await context.Wallets.AddAsync(systemWallet);
            await context.SaveChangesAsync();

            logger?.LogInformation(
                "✓ Created wallet for System Admin {Email} with OwnerType System → Wallets table",
                systemAdminUser.Email);
            return;
        }

        if (string.Equals(existingWallet.OwnerType, "System", StringComparison.OrdinalIgnoreCase))
        {
            logger?.LogInformation("System Admin wallet already exists with OwnerType System. Skipping.");
            return;
        }
    }

    private static async Task SeedScheduleTemplatesAsync(
        ApplicationDbContext context,
        ILogger? logger)
    {
        logger?.LogInformation("Seeding schedule templates...");

        // Check if any schedule template already exists
        var existingTemplate = await context.ScheduleTemplates.FirstOrDefaultAsync();

        if (existingTemplate == null)
        {
            var now = DateTime.UtcNow;
            // Create templates for Monday to Friday, 9 AM to 5 PM, 30-minute slots, capacity 2
            int[] weekdays = [1, 2, 3, 4, 5]; // Monday to Friday

            foreach (var day in weekdays)
            {
                var template = new Domain.Entities.Scheduling.ScheduleTemplate(
                    dayOfWeek: (DayOfWeek)day,
                    startTime: new TimeOnly(9, 0), // 9 AM
                    endTime: new TimeOnly(17, 0),  // 5 PM
                    slotDuration: 30,              // 30-minute slots
                    maxCapacity: 2                 // Max 2 patients per slot
                );

                await context.ScheduleTemplates.AddAsync(template);
            }

            await context.SaveChangesAsync();
            logger?.LogInformation("✓ Created 5 schedule templates (Mon-Fri 9am-5pm, 30min slots) → ScheduleTemplates table");
        }
        else
        {
            logger?.LogInformation("Schedule templates already exist. Skipping.");
        }

        logger?.LogInformation("Schedule template seeding completed.");
    }

    /// <summary>
    /// Idempotent permission + default role-permission seeder.
    /// Safe to run on every startup: inserts missing permissions, skips existing ones.
    /// New permissions added to <see cref="Permissions.All"/> will be created automatically
    /// on the next deployment without requiring a migration.
    /// </summary>
    private static async Task SeedPermissionsAsync(
        ApplicationDbContext context,
        RoleManager<ApplicationRole> roleManager,
        ILogger? logger)
    {
        logger?.LogInformation("Seeding permissions (idempotent)...");

        // 1. Upsert permissions ------------------------------------------------
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
            logger?.LogInformation("  ✓ New permission: [{Category}] {Name}", def.Category, def.Name);
        }

        if (newPermissions.Count > 0)
        {
            await context.Permissions.AddRangeAsync(newPermissions);
            await context.SaveChangesAsync();
            logger?.LogInformation("Seeded {Count} new permissions.", newPermissions.Count);
        }
        else
        {
            logger?.LogInformation("All permissions already exist. Skipping permission insert.");
        }

        // 2. Seed default role-permission assignments --------------------------
        // Load fresh from DB so we have IDs for both existing + newly inserted
        var allPermissions = await context.Permissions
            .ToDictionaryAsync(p => p.Name, StringComparer.OrdinalIgnoreCase);

        int assignmentsCreated = 0;

        foreach (var (roleName, permissionNames) in Permissions.DefaultRolePermissions)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                logger?.LogWarning("  Role '{Role}' not found — skipping its permission assignments.", roleName);
                continue;
            }

            // Use IgnoreQueryFilters to catch soft-deleted records and avoid unique constraint violations
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
                    logger?.LogWarning("  Permission '{Perm}' not found — skipping.", permName);
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
        else
        {
            logger?.LogInformation("All default role-permission assignments already exist. Skipping.");
        }

        logger?.LogInformation("Permission seeding completed.");
    }

}
