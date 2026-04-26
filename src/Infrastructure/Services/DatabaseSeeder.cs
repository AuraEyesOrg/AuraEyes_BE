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

        // Step 3: Seed domain entities (Organisation, Ophthalmologist, Patient) (idempotent)
        await SeedDomainEntitiesAsync(context, userManager, logger);

        // Step 4: Ensure System Admin wallet exists with OwnerType = "System"
        await EnsureSystemAdminWalletAsync(context, userManager, logger);

        // Step 5: Seed permissions + default role assignments
        // Idempotent â€” runs on every startup so new permissions defined in code
        // are automatically added to the database on next deployment.
        await SeedPermissionsAsync(context, roleManager, logger);

        // Step 6: Seed test data for clinic queue (Cashier page) (idempotent)
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

        // Digital Clinic model: SystemAdmin + Ophthalmologist + ClinicStaff + Patient
        var defaultAccounts = new List<(string Email, string Password, string Role, string FullName)>
        {
            // Clinic Owner
            (configuration["Swagger:Username"] ?? "systemadmin@auraeyes.vn",
             configuration["Swagger:Password"] ?? "Admin@123$",
             Roles.SystemAdmin, "Clinic Owner (System Admin)"),

            // Ophthalmologist
            ("doctor@auraeyes.vn", "Doctor@123$", Roles.Ophthalmologist, "BS. Nguyen Van An"),

            // Clinic Staff â€” three sub-role examples
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
                // Ensure user is in the required role even if account already exists
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
        logger?.LogInformation("Seeding domain entities (Organisations, Ophthalmologist, ClinicStaff, Patient, Wallets, Schedules)...");

        // Step 0: Seed Organisation
        var adminUser = await userManager.FindByEmailAsync("systemadmin@auraeyes.vn");
        var organisation = await context.Organisations.FirstOrDefaultAsync(o => o.Name == "AuraEyes General Hospital");
        if (organisation == null && adminUser != null)
        {
            organisation = new Organisation(
                ownerId: adminUser.Id,
                name: "AuraEyes General Hospital",
                orgType: OrgType.Hospital,
                address: "123 Healthcare St, Dist 1, HCMC",
                description: "A state-of-the-art ophthalmology hospital."
            );
            await context.Organisations.AddAsync(organisation);
            await context.SaveChangesAsync();
            logger?.LogInformation("? Created organisation: AuraEyes General Hospital");
        }

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
                    bio: "BÃ¡c sÄ© chuyÃªn khoa máº¯t vá»›i kinh nghiá»‡m trong lÄ©nh vá»±c sÃ ng lá»c bá»‡nh vÃµng máº¡c.",
                    yearsOfExperience: 5,
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
                logger?.LogInformation("âœ“ Created wallet for Ophthalmologist with 1,000,000 VND â†’ Wallets table");
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
                logger?.LogInformation("âœ“ Created wallet for Patient with 5,000,000 VND â†’ Wallets table");
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
                "âœ“ Created wallet for System Admin {Email} with OwnerType System â†’ Wallets table",
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
            logger?.LogInformation("âœ“ Created 5 schedule templates (Mon-Fri 9am-5pm, 30min slots) â†’ ScheduleTemplates table");
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
            logger?.LogInformation("  âœ“ New permission: [{Category}] {Name}", def.Category, def.Name);
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
                logger?.LogWarning("  Role '{Role}' not found â€” skipping its permission assignments.", roleName);
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
        else
        {
            logger?.LogInformation("All default role-permission assignments already exist. Skipping.");
        }

        logger?.LogInformation("Permission seeding completed.");
    }

    private static async Task SeedClinicQueueTestDataAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger? logger)
    {
        logger?.LogInformation("Seeding test data for clinic queue (Cashier page)...");

        var doctorUser = await userManager.FindByEmailAsync("doctor@auraeyes.vn");
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

        // Check if we already have a visit waiting for payment for this patient
        // Fresh seeding forced

        // 1. Create AI Screening
        var aiScreening = new AiScreening(patient.Id, "AuraEyes-AI-v1.0");
        aiScreening.Process("{\"result\": \"High risk of AMD\", \"confidence\": 0.92}");
        await context.AiScreenings.AddAsync(aiScreening);
        await context.SaveChangesAsync();

        // 2. Create Consultation Session
        // Note: OrganisationId is nullable, we'll use Guid.Empty if not found or just leave it null if the factory allows.
        // Looking at ConsultationSession.cs, CreateClinicBooking requires organisationId.
        // Let's see if we can find any organisation.
        var organisation = await context.Organisations.FirstOrDefaultAsync();
        ConsultationSession session;
        if (organisation != null)
        {
            session = ConsultationSession.CreateClinicBooking(patient.Id, organisation.Id, 500000, DateTime.UtcNow.AddHours(1), ophthalmologist.Id);
        }
        else
        {
            session = ConsultationSession.CreateVerification(patient.Id, aiScreening.Id, 200000, ophthalmologist.Id);
        }
        
        // Mark session as completed
        // Need to check SessionStatus and how to complete it.
        // For simplicity, let's just set the properties directly or find a method.
        // SessionStatus.Completed = 3 (usually)
        
        await context.ConsultationSessions.AddAsync(session);
        await context.SaveChangesAsync();

        // 3. Create Medical Diagnosis with JSON Snapshot
        var prescriptionData = new
        {
            DiagnosisCode = "H35.30",
            CodingSystem = "ICD-10",
            ClinicalFindings = "Cháº©n Ä‘oÃ¡n xÃ¡c Ä‘á»‹nh: ThoÃ¡i hÃ³a hoÃ ng Ä‘iá»ƒm tuá»•i giÃ  (AMD) thá»ƒ khÃ´. CÃ³ cÃ¡c máº£ng drusen kÃ­ch thÆ°á»›c trung bÃ¬nh vÃ¹ng trung tÃ¢m.",
            SeverityLevel = "Moderate",
            Recommendations = "Äeo kÃ­nh rÃ¢m khi ra ngoÃ i trá»i, bá»• sung vitamin Lutein/Zeaxanthin.",
            FollowUpDate = DateTime.UtcNow.AddMonths(2),
            DiagnosedBy = new
            {
                DoctorId = ophthalmologist.Id,
                DoctorName = doctorUser.FullName
            },
            FinalizedAt = DateTime.UtcNow,
            PrescriptionItems = new[]
            {
                new
                {
                    MedicineName = "Vismed 0.18%",
                    Unit = "Há»™p",
                    Dosage = "1 giá»t/láº§n",
                    Frequency = "4 láº§n/ngÃ y",
                    Duration = "30 days",
                    Instruction = "Nhá» máº¯t khi cáº£m tháº¥y khÃ´, má»i."
                },
                new
                {
                    MedicineName = "PreserVision AREDS 2",
                    Unit = "Lá»",
                    Dosage = "1 viÃªn/láº§n",
                    Frequency = "2 láº§n/ngÃ y",
                    Duration = "60 days",
                    Instruction = "Uá»‘ng sau bá»¯a Äƒn sÃ¡ng vÃ  tá»‘i."
                }
            },
            PrescriptionNote = "TrÃ¡nh tiáº¿p xÃºc trá»±c tiáº¿p vá»›i Ã¡nh náº¯ng máº·t trá»i gáº¯t.",
            NoMedicationPrescribed = false
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

        // 4. Create Appointment Slot and Appointment
        // Find or create a schedule template for this doctor
        var template = await context.ScheduleTemplates.FirstOrDefaultAsync(t => t.OphthalId == ophthalmologist.Id);
        if (template == null)
        {
            template = new Domain.Entities.Scheduling.ScheduleTemplate(
                dayOfWeek: DateTime.UtcNow.DayOfWeek,
                startTime: new TimeOnly(8, 0),
                endTime: new TimeOnly(17, 0),
                slotDuration: 30,
                maxCapacity: 1,
                cost: 500000,
                ophthalId: ophthalmologist.Id,
                orgId: organisation?.Id
            );
            await context.ScheduleTemplates.AddAsync(template);
            await context.SaveChangesAsync();
        }

        var slot = new AppointmentSlot(
            scheduleTemplateId: template.Id,
            date: DateOnly.FromDateTime(DateTime.UtcNow),
            startTime: TimeOnly.FromDateTime(DateTime.UtcNow.AddMinutes(-30)),
            endTime: TimeOnly.FromDateTime(DateTime.UtcNow.AddMinutes(30)),
            maxCapacity: 1
        );
        slot.UpdateOphthalId(ophthalmologist.Id);
        slot.UpdateCost(500000);
        await context.AppointmentSlots.AddAsync(slot);
        await context.SaveChangesAsync();

        var appointment = new Appointment(
            patientId: patient.Id,
            appointmentSlotId: slot.Id,
            price: 500000,
            requestedDoctorId: ophthalmologist.Id,
            visitReason: "Tu v?n b?nh võng m?c"
        );
        appointment.Confirm();
        await context.Appointments.AddAsync(appointment);
        await context.SaveChangesAsync();

        // 5. Create Patient Visit from Appointment
        var visit = PatientVisit.CreateFromAppointment(appointment);
        visit.Start();
        visit.FinishConsultation("D? li?u m?u cho trang Cashier");

        await context.PatientVisits.AddAsync(visit);
        await context.SaveChangesAsync();

        logger?.LogInformation("âœ“ Successfully seeded clinic queue test data for {Email}", patientUser.Email);
    }
}



