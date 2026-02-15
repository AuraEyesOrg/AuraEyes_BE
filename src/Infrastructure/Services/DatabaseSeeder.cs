using Application.Common.Constants;
using Domain.Entities;
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
            
            // Step 1: Seed roles first
            await SeedRolesAsync(roleManager, logger);

            // Step 2: Seed default accounts (AspNetUsers + AspNetUserRoles)
            await SeedDefaultAccountsAsync(userManager, logger);

            // Step 3: Seed domain entities (Organisation, Ophthalmologist, Patient)
            await SeedDomainEntitiesAsync(context, userManager, logger);
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
                    userId: patientUser.Id,
                    medicalHistorySummary: "No significant medical history. First-time screening for diabetic retinopathy."
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
}
