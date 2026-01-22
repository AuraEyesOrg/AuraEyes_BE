using Application.Common.Constants;
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
        ("systemadmin@gmail.com", "SystemAdmin@123$", Roles.SystemAdmin, "System Administrator"),
        ("orgadmin@gmail.com", "OrgAdmin@123$", Roles.OrgAdmin, "Organization Administrator"),
        ("ophthalmologist@gmail.com", "Ophthalmologist@123$", Roles.Ophthalmologist, "Doctor Ophthalmologist"),
        ("patient@gmail.com", "Patient@123$", Roles.Patient, "Patient User")
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
            
            // Seed roles first
            await SeedRolesAsync(roleManager, logger);

            // Then seed default accounts
            await SeedDefaultAccountsAsync(userManager, logger);
        }
        else
        {
            logger?.LogInformation("Roles already exist. Skipping seed process.");
        }
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
}
