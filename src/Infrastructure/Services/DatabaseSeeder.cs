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

        // Seed roles
        await SeedRolesAsync(roleManager, logger);

        // Seed default accounts
        await SeedDefaultAccountsAsync(userManager, logger);
    }

    private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager, ILogger? logger)
    {
        logger?.LogInformation("Seeding Identity roles...");

        foreach (var roleName in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new ApplicationRole(roleName)
                {
                    Description = GetRoleDescription(roleName)
                };
                
                var result = await roleManager.CreateAsync(role);
                
                if (result.Succeeded)
                {
                    logger?.LogInformation("Created role: {RoleName}", roleName);
                }
                else
                {
                    logger?.LogWarning("Failed to create role {RoleName}: {Errors}", 
                        roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                logger?.LogDebug("Role {RoleName} already exists, skipping", roleName);
            }
        }

        logger?.LogInformation("Role seeding completed. Total roles: {Count}", Roles.All.Length);
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
        logger?.LogInformation("Seeding default user accounts...");

        foreach (var (email, password, role, fullName) in DefaultAccounts)
        {
            var existingUser = await userManager.FindByEmailAsync(email);

            if (existingUser == null)
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
                    logger?.LogInformation("Created user: {Email} with role {Role}", email, role);

                    var roleResult = await userManager.AddToRoleAsync(user, role);

                    if (roleResult.Succeeded)
                    {
                        logger?.LogInformation("Assigned role {Role} to user {Email}", role, email);
                    }
                    else
                    {
                        logger?.LogWarning("Failed to assign role {Role} to user {Email}: {Errors}",
                            role, email, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    logger?.LogWarning("Failed to create user {Email}: {Errors}",
                        email, string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                logger?.LogDebug("User {Email} already exists, skipping", email);
            }
        }

        logger?.LogInformation("Default account seeding completed. Total accounts: {Count}", DefaultAccounts.Length);
    }
}
