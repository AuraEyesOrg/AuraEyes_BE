using Application.Common.Constants;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

/// <summary>
/// Database seeder for initial data.
/// Creates default roles and optionally a system admin user.
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        // Apply pending migrations
        if (context.Database.GetPendingMigrations().Any())
        {
            await context.Database.MigrateAsync();
        }

        // Seed roles
        await SeedRolesAsync(roleManager);

        // Seed system admin (optional - for development)
        await SeedSystemAdminAsync(userManager);
    }

    private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
        foreach (var roleName in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new ApplicationRole(roleName)
                {
                    Description = GetRoleDescription(roleName)
                };
                await roleManager.CreateAsync(role);
            }
        }
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

    private static async Task SeedSystemAdminAsync(UserManager<ApplicationUser> userManager)
    {
        const string adminEmail = "admin@aura.health";
        const string adminPassword = "Admin@123456"; // Change this in production!

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Administrator",
                EmailConfirmed = true, // Pre-confirmed for development
                IsActive = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, Roles.SystemAdmin);
            }
        }
    }
}
