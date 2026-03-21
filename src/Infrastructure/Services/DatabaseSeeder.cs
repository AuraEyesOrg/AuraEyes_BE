using Application.Common.Constants;
using Domain.Entities.Authorization;
using Domain.Entities.Contracts;
using Domain.Entities.Users;
using Domain.Enums;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Database seeder for initial data.
/// Creates default roles and default user accounts.
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Default accounts to seed.
    /// Each account has email, password, role, and full name.
    /// </summary>
    private static readonly (string Email, string Password, string Role, string FullName)[] DefaultAccounts =
    [
        ("systemadmin@gmail.com", "Password123!", Roles.SystemAdmin, "System Administrator"),
        ("orgadmin@gmail.com", "Password123!", Roles.OrgAdmin, "Organization Administrator"),
        ("ophthalmologist@gmail.com", "Password123!", Roles.Ophthalmologist, "Doctor Ophthalmologist"),
        ("patient@gmail.com", "Password123!", Roles.Patient, "Patient User")
    ];

    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ILogger? logger = null)
    {
        // Apply pending migrations
        if (context.Database.GetPendingMigrations().Any())
        {
            logger?.LogInformation("Applying pending migrations...");
            await context.Database.MigrateAsync();
            logger?.LogInformation("Migrations applied successfully");
        }

        // Check if any roles exist - only seed if database is completely empty
        var hasRoles = await roleManager.Roles.AnyAsync();

        if (!hasRoles)
        {
            logger?.LogInformation("No roles found in database. Starting initial seed...");

            // Step 1: Seed roles first
            await SeedRolesAsync(roleManager, logger);

            // Step 2: Seed default accounts (AspNetUsers + AspNetUserRoles)
            await SeedDefaultAccountsAsync(userManager, logger);
        }
        else
        {
            logger?.LogInformation("Roles already exist. Skipping initial account seed.");
        }

        // Step 3: Seed domain entities (Organisation, Ophthalmologist, Patient)
        // Always runs — idempotent, skips if entities already exist.
        // This ensures domain profiles are created even if the server was restarted
        // after roles were seeded but before domain entities were created.
        await SeedDomainEntitiesAsync(context, userManager, logger);

        // Step 4: Seed permissions + default role assignments
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
            Roles.OrgAdmin => "Organization administrators who manage their organization's users and settings",
            Roles.SystemAdmin => "System administrators with full access to all features",
            _ => roleName
        };
    }

    private static async Task SeedDefaultAccountsAsync(UserManager<ApplicationUser> userManager, ILogger? logger)
    {
        logger?.LogInformation("Seeding default user accounts into AspNetUsers and AspNetUserRoles...");

        foreach (var (email, password, role, fullName) in DefaultAccounts)
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

            // Create user → inserts into AspNetUsers table
            var createResult = await userManager.CreateAsync(user, password);

            if (createResult.Succeeded)
            {
                logger?.LogInformation("✓ Created user: {Email} → AspNetUsers", email);

                // Assign role → inserts into AspNetUserRoles table
                var roleResult = await userManager.AddToRoleAsync(user, role);

                if (roleResult.Succeeded)
                {
                    logger?.LogInformation("✓ Assigned role {Role} to {Email} → AspNetUserRoles", role, email);
                }
                else
                {
                    logger?.LogError("✗ Failed to assign role {Role} to user {Email}: {Errors}",
                        role, email, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                logger?.LogError("✗ Failed to create user {Email}: {Errors}",
                    email, string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }
        }

        logger?.LogInformation("Default account seeding completed. Total accounts created: {Count}", DefaultAccounts.Length);
    }

    private static async Task SeedDomainEntitiesAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger? logger)
    {
        logger?.LogInformation("Seeding domain entities (Organisation, Ophthalmologist, Patient)...");

        // Step 1: Seed Organisation
        var orgAdminUser = await userManager.FindByEmailAsync("orgadmin@gmail.com");
        if (orgAdminUser == null)
        {
            logger?.LogWarning("OrgAdmin user not found. Skipping organisation seeding.");
        }
        else
        {
            var existingOrg = await context.Organisations
                .FirstOrDefaultAsync(o => o.OwnerId == orgAdminUser.Id);

            if (existingOrg == null)
            {
                var organisation = new Organisation(
                    ownerId: orgAdminUser.Id,
                    name: "Auski Hospital",
                    orgType: OrgType.Hospital,
                    address: "S1006 Vinhomes Grand Park, Ho Chi Minh City, Viet Nam",
                    licenseNumber: "MED-HCM-2024-001"
                );

                await context.Organisations.AddAsync(organisation);
                await context.SaveChangesAsync();

                logger?.LogInformation("✓ Created organisation: {OrgName} → Organisations table", organisation.Name);

                // Update OrgAdmin's OrganizationId
                orgAdminUser.OrganizationId = organisation.Id;
                await userManager.UpdateAsync(orgAdminUser);
                logger?.LogInformation("✓ Linked OrgAdmin to organisation");
            }
            else
            {
                logger?.LogInformation("Organisation already exists. Skipping.");
            }
        }

        // Step 2: Seed Ophthalmologist entity
        var ophthalmologistUser = await userManager.FindByEmailAsync("ophthalmologist@gmail.com");
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
                    bio: "Experienced ophthalmologist specializing in retinal diseases and diabetic retinopathy screening.",
                    yearsOfExperience: 5,
                    phone: "+84123456789",
                    licenseUrl: null, // Will be uploaded later
                    degreeUrl: null   // Will be uploaded later
                );

                await context.Ophthalmologists.AddAsync(ophthalmologist);
                await context.SaveChangesAsync();

                logger?.LogInformation("✓ Created ophthalmologist profile for {Email} → Ophthalmologists table",
                    ophthalmologistUser.Email);
            }
            else
            {
                logger?.LogInformation("Ophthalmologist profile already exists. Skipping.");
            }
        }

        // Step 3: Seed Patient entity
        var patientUser = await userManager.FindByEmailAsync("patient@gmail.com");
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
                var patient = new Patient(
                    userId: patientUser.Id
                );

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

        logger?.LogInformation("Domain entity seeding completed.");
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

            // Only fetch assignments for this role to avoid N+1
            var existingRolePermissionIds = await context.RolePermissions
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
