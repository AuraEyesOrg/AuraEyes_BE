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

        // Step 5: Seed default contract templates with variables
        // Idempotent — skips if templates already exist.
        await SeedContractTemplatesAsync(context, logger);
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

    private static async Task SeedContractTemplatesAsync(
        ApplicationDbContext context,
        ILogger? logger)
    {
        logger?.LogInformation("Checking contract templates and variables...");

        int templatesCreated = 0;
        int variablesSeeded = 0;

        /* ── Ophthalmologist Contract Template ── */
        const string ophthHtml = @"<div style=""font-family:'Times New Roman',serif;color:#1e293b;line-height:1.7;font-size:13pt;text-align:justify;"">
  <div style=""text-align:center;font-weight:bold;text-transform:uppercase;letter-spacing:1px;margin-bottom:4px;"">CỘNG HOÀ XÃ HỘI CHỦ NGHĨA VIỆT NAM</div>
  <div style=""text-align:center;font-weight:bold;border-bottom:1.5px solid #1e293b;display:inline-block;width:100%;padding-bottom:4px;margin-bottom:24px;"">Độc lập – Tự do – Hạnh phúc</div>
  <h1 style=""text-align:center;font-size:15pt;font-weight:bold;text-transform:uppercase;margin:0 0 6px;"">HỢP ĐỒNG HỢP TÁC CHUYÊN MÔN Y KHOA</h1>
  <p style=""text-align:center;font-style:italic;color:#64748b;margin-bottom:24px;"">Số: {{contractNumber}}/AURA-HĐ</p>
  <p>Hôm nay, ngày <strong>{{day}}</strong> tháng <strong>{{month}}</strong> năm <strong>{{year}}</strong>, tại TP. Hồ Chí Minh, chúng tôi gồm:</p>
  <div style=""margin:20px 0;padding:16px 20px;border-left:4px solid #7c3aed;background:#f5f3ff;border-radius:0 8px 8px 0;"">
    <p style=""font-weight:bold;margin:0 0 8px;color:#7c3aed;"">BÊN A (Đơn vị cung cấp dịch vụ)</p>
    <p style=""margin:2px 0;""><b>Tên tổ chức:</b> Công ty TNHH AURA EYES VIETNAM</p>
    <p style=""margin:2px 0;""><b>Địa chỉ:</b> 123 Đường Công nghệ, Khu CNC, TP. Thủ Đức</p>
    <p style=""margin:2px 0;""><b>Đại diện:</b> Ban Giám Đốc Công ty</p>
  </div>
  <div style=""margin:20px 0;padding:16px 20px;border-left:4px solid #0891b2;background:#ecfeff;border-radius:0 8px 8px 0;"">
    <p style=""font-weight:bold;margin:0 0 8px;color:#0891b2;"">BÊN B (Chuyên gia Y tế)</p>
    <p style=""margin:2px 0;""><b>Họ và tên:</b> {{doctorName}}</p>
    <p style=""margin:2px 0;""><b>Ngày sinh:</b> {{doctorDOB}}</p>
    <p style=""margin:2px 0;""><b>Số CCCD:</b> {{doctorLicenseNumber}}</p>
    <p style=""margin:2px 0;""><b>Địa chỉ:</b> {{doctorAddress}}</p>
    <p style=""margin:2px 0;""><b>Điện thoại:</b> {{doctorPhone}} &nbsp;|&nbsp; <b>Email:</b> {{doctorEmail}}</p>
    <p style=""margin:2px 0;""><b>Chuyên khoa:</b> {{doctorSpecialization}}</p>
    <p style=""margin:2px 0;""><b>Chứng chỉ hành nghề số:</b> {{doctorPracticeNumber}}</p>
  </div>
  <h2 style=""font-size:12pt;font-weight:bold;text-transform:uppercase;margin:24px 0 8px;padding-bottom:4px;border-bottom:1px solid #e2e8f0;"">ĐIỀU 1. MỤC ĐÍCH VÀ NỘI DUNG HỢP TÁC</h2>
  <ol style=""margin:0;padding-left:22px;"">
    <li style=""margin-bottom:6px;"">Bên B thực hiện dịch vụ đọc và phân tích hình ảnh đáy mắt cho các bệnh nhân của Bên A thông qua nền tảng AURA platform.</li>
    <li style=""margin-bottom:6px;"">Thời hạn hợp đồng: <strong>{{contractDuration}}</strong> tháng kể từ ngày ký.</li>
    <li>Phạm vi hoạt động trên toàn quốc, thông qua hệ thống telemedicine của Bên A.</li>
  </ol>
  <h2 style=""font-size:12pt;font-weight:bold;text-transform:uppercase;margin:24px 0 8px;padding-bottom:4px;border-bottom:1px solid #e2e8f0;"">ĐIỀU 2. QUYỀN LỢI VÀ NGHĨA VỤ</h2>
  <ul style=""margin:0;padding-left:22px;"">
    <li style=""margin-bottom:6px;"">Bên B được hưởng <strong>{{revenueShare}}%</strong> doanh thu từ mỗi ca khám được phân công.</li>
    <li style=""margin-bottom:6px;"">Thanh toán định kỳ vào ngày <strong>{{paymentDay}}</strong> hằng tháng.</li>
    <li>Phạt vi phạm hợp đồng: <strong>{{penaltyAmount}}</strong> VNĐ.</li>
  </ul>
  <h2 style=""font-size:12pt;font-weight:bold;text-transform:uppercase;margin:24px 0 8px;padding-bottom:4px;border-bottom:1px solid #e2e8f0;"">ĐIỀU 3. BẢO MẬT VÀ HIỆU LỰC</h2>
  <p>Hợp đồng có hiệu lực từ ngày ký. Lập thành 02 bản có giá trị pháp lý ngang nhau.</p>
  <table style=""width:100%;margin-top:48px;border-collapse:collapse;"">
    <tr>
      <td style=""width:50%;text-align:center;padding:12px;vertical-align:top;""><strong>ĐẠI DIỆN BÊN A</strong><br/><em style=""font-size:11pt;"">(Ký, ghi rõ họ tên, đóng dấu)</em><div style=""margin-top:60px;border-top:1px solid #94a3b8;padding-top:8px;"">Ban Giám Đốc Công ty</div></td>
      <td style=""width:50%;text-align:center;padding:12px;vertical-align:top;""><strong>ĐẠI DIỆN BÊN B</strong><br/><em style=""font-size:11pt;"">(Ký, ghi rõ họ tên)</em><div style=""margin-top:60px;border-top:1px solid #94a3b8;padding-top:8px;"">{{doctorName}}</div></td>
    </tr>
  </table>
</div>";

        var ophthTemplate = await context.ContractTemplates
            .Where(t => t.Type == ContractType.OphthalmologistContract && !t.IsDeleted)
            .FirstOrDefaultAsync();

        if (ophthTemplate == null)
        {
            ophthTemplate = new ContractTemplate(
                "Mẫu Hợp Đồng Bác Sĩ Nhãn Khoa",
                ContractType.OphthalmologistContract,
                "v1.0",
                ophthHtml);
            var vars = BuildOphthVariables(ophthTemplate.Id);
            ophthTemplate.SetVariables(vars);
            await context.ContractTemplates.AddAsync(ophthTemplate);
            templatesCreated++;
            variablesSeeded += vars.Count;
            logger?.LogInformation("✓ Created ophthalmologist contract template with {Count} variables.", vars.Count);
        }
        else
        {
            var varCount = await context.ContractTemplateVariables
                .CountAsync(v => v.TemplateId == ophthTemplate.Id);
            if (varCount == 0)
            {
                var vars = BuildOphthVariables(ophthTemplate.Id);
                await context.ContractTemplateVariables.AddRangeAsync(vars);
                variablesSeeded += vars.Count;
                logger?.LogInformation("✓ Seeded {Count} variables into existing ophthalmologist template.", vars.Count);
            }
        }

        /* ── Organization Contract Template ── */
        const string orgHtml = @"<div style=""font-family:'Times New Roman',serif;color:#1e293b;line-height:1.7;font-size:13pt;text-align:justify;"">
  <div style=""text-align:center;font-weight:bold;text-transform:uppercase;letter-spacing:1px;margin-bottom:4px;"">CỘNG HOÀ XÃ HỘI CHỦ NGHĨA VIỆT NAM</div>
  <div style=""text-align:center;font-weight:bold;border-bottom:1.5px solid #1e293b;display:inline-block;width:100%;padding-bottom:4px;margin-bottom:24px;"">Độc lập – Tự do – Hạnh phúc</div>
  <h1 style=""text-align:center;font-size:15pt;font-weight:bold;text-transform:uppercase;margin:0 0 6px;"">HỢP ĐỒNG LIÊN KẾT CUNG CẤP DỊCH VỤ Y TẾ</h1>
  <p style=""text-align:center;font-style:italic;color:#64748b;margin-bottom:24px;"">Số: {{contractNumber}}/AURA-HĐ</p>
  <p>Hôm nay, ngày <strong>{{day}}</strong> tháng <strong>{{month}}</strong> năm <strong>{{year}}</strong>, tại TP. Hồ Chí Minh, chúng tôi gồm:</p>
  <div style=""margin:20px 0;padding:16px 20px;border-left:4px solid #7c3aed;background:#f5f3ff;border-radius:0 8px 8px 0;"">
    <p style=""font-weight:bold;margin:0 0 8px;color:#7c3aed;"">BÊN A (AURA EYES VIETNAM)</p>
    <p style=""margin:2px 0;""><b>Tên tổ chức:</b> Công ty TNHH AURA EYES VIETNAM</p>
    <p style=""margin:2px 0;""><b>Địa chỉ:</b> 123 Đường Công nghệ, Khu CNC, TP. Thủ Đức</p>
    <p style=""margin:2px 0;""><b>Đại diện:</b> Ban Giám Đốc Công ty</p>
  </div>
  <div style=""margin:20px 0;padding:16px 20px;border-left:4px solid #d97706;background:#fffbeb;border-radius:0 8px 8px 0;"">
    <p style=""font-weight:bold;margin:0 0 8px;color:#d97706;"">BÊN B (Đơn vị Y tế)</p>
    <p style=""margin:2px 0;""><b>Tên tổ chức:</b> {{orgName}}</p>
    <p style=""margin:2px 0;""><b>Số đăng ký kinh doanh:</b> {{orgLicenseNumber}}</p>
    <p style=""margin:2px 0;""><b>Địa chỉ:</b> {{orgAddress}}</p>
    <p style=""margin:2px 0;""><b>Điện thoại:</b> {{orgPhone}} &nbsp;|&nbsp; <b>Email:</b> {{orgEmail}}</p>
    <p style=""margin:2px 0;""><b>Đại diện:</b> {{orgRepresentative}} – <em>{{orgPosition}}</em></p>
  </div>
  <h2 style=""font-size:12pt;font-weight:bold;text-transform:uppercase;margin:24px 0 8px;padding-bottom:4px;border-bottom:1px solid #e2e8f0;"">ĐIỀU 1. MỤC TIÊU VÀ NỘI DUNG LIÊN KẾT</h2>
  <p>Hai bên cùng hợp tác triển khai chương trình tầm soát bệnh lý đáy mắt thông qua thiết bị chụp đáy mắt và hệ thống AI AURA.</p>
  <p>Thời hạn hợp đồng: <strong>{{contractDuration}}</strong> tháng kể từ ngày ký.</p>
  <h2 style=""font-size:12pt;font-weight:bold;text-transform:uppercase;margin:24px 0 8px;padding-bottom:4px;border-bottom:1px solid #e2e8f0;"">ĐIỀU 3. CƠ CHẾ TÀI CHÍNH</h2>
  <ul style=""margin:0;padding-left:22px;"">
    <li style=""margin-bottom:6px;"">Bên B được hưởng <strong>{{revenueShare}}%</strong> doanh thu dịch vụ phát sinh tại cơ sở.</li>
    <li style=""margin-bottom:6px;"">Thanh toán vào ngày <strong>{{paymentDay}}</strong> hằng tháng.</li>
    <li>Tiền phạt vi phạm: <strong>{{penaltyAmount}}</strong> VNĐ.</li>
  </ul>
  <h2 style=""font-size:12pt;font-weight:bold;text-transform:uppercase;margin:24px 0 8px;padding-bottom:4px;border-bottom:1px solid #e2e8f0;"">ĐIỀU 6. ĐIỀU KHOẢN THI HÀNH</h2>
  <p>Hợp đồng có hiệu lực từ ngày ký. Lập thành 02 bản có giá trị pháp lý ngang nhau.</p>
  <table style=""width:100%;margin-top:48px;border-collapse:collapse;"">
    <tr>
      <td style=""width:50%;text-align:center;padding:12px;vertical-align:top;""><strong>ĐẠI DIỆN BÊN A</strong><br/><em style=""font-size:11pt;"">(Ký, ghi rõ họ tên, đóng dấu)</em><div style=""margin-top:60px;border-top:1px solid #94a3b8;padding-top:8px;"">Ban Giám Đốc Công ty</div></td>
      <td style=""width:50%;text-align:center;padding:12px;vertical-align:top;""><strong>ĐẠI DIỆN BÊN B</strong><br/><em style=""font-size:11pt;"">(Ký, ghi rõ họ tên, đóng dấu)</em><div style=""margin-top:60px;border-top:1px solid #94a3b8;padding-top:8px;"">{{orgRepresentative}}</div></td>
    </tr>
  </table>
</div>";

        var orgTemplate = await context.ContractTemplates
            .Where(t => t.Type == ContractType.MedicalOrganizationContract && !t.IsDeleted)
            .FirstOrDefaultAsync();

        if (orgTemplate == null)
        {
            orgTemplate = new ContractTemplate(
                "Mẫu Hợp Đồng Tổ Chức Y Tế",
                ContractType.MedicalOrganizationContract,
                "v1.0",
                orgHtml);
            var vars = BuildOrgVariables(orgTemplate.Id);
            orgTemplate.SetVariables(vars);
            await context.ContractTemplates.AddAsync(orgTemplate);
            templatesCreated++;
            variablesSeeded += vars.Count;
            logger?.LogInformation("✓ Created organization contract template with {Count} variables.", vars.Count);
        }
        else
        {
            var varCount = await context.ContractTemplateVariables
                .CountAsync(v => v.TemplateId == orgTemplate.Id);
            if (varCount == 0)
            {
                var vars = BuildOrgVariables(orgTemplate.Id);
                await context.ContractTemplateVariables.AddRangeAsync(vars);
                variablesSeeded += vars.Count;
                logger?.LogInformation("✓ Seeded {Count} variables into existing organization template.", vars.Count);
            }
        }

        if (templatesCreated > 0 || variablesSeeded > 0)
        {
            await context.SaveChangesAsync();
            logger?.LogInformation(
                "Contract template seeding complete: {T} template(s) created, {V} variable(s) seeded.",
                templatesCreated, variablesSeeded);
        }
        else
        {
            logger?.LogInformation("Contract templates and variables already fully seeded. Skipping.");
        }
    }

    private static List<ContractTemplateVariable> BuildOphthVariables(Guid templateId) =>
    [
        // Date & Time
        new(templateId, "day", "Ngày", VariableType.Number, null, null, null, null, true, 0),
        new(templateId, "month", "Tháng", VariableType.Number, null, null, null, null, true, 1),
        new(templateId, "year", "Năm", VariableType.Number, null, null, null, null, true, 2),
        new(templateId, "paymentDay", "Ngày thanh toán hàng tháng", VariableType.Number, "Ngày trong tháng dùng để thanh toán lương", null, null, null, false, 3),
        // Doctor info
        new(templateId, "doctorName", "Họ và tên bác sĩ", VariableType.Text, null, null, null, null, true, 4),
        new(templateId, "doctorDOB", "Ngày sinh", VariableType.Date, null, null, null, null, false, 5),
        new(templateId, "doctorLicenseNumber", "Số CCCD / Hộ chiếu", VariableType.Text, null, null, null, null, true, 6),
        new(templateId, "doctorAddress", "Địa chỉ thường trú", VariableType.Text, null, null, null, null, false, 7),
        new(templateId, "doctorPhone", "Số điện thoại", VariableType.Text, null, null, null, null, false, 8),
        new(templateId, "doctorEmail", "Email", VariableType.Text, null, null, null, null, false, 9),
        new(templateId, "doctorSpecialization", "Chuyên khoa", VariableType.Text, null, null, null, null, false, 10),
        new(templateId, "doctorPracticeNumber", "Số chứng chỉ hành nghề", VariableType.Text, null, null, null, null, false, 11),
        // Contract terms
        new(templateId, "contractNumber", "Số hợp đồng", VariableType.Text, null, null, null, null, false, 12),
        new(templateId, "contractDuration", "Thời hạn hợp đồng", VariableType.Number, null, null, null, "tháng", false, 13),
        new(templateId, "revenueShare", "Tỷ lệ chia sẻ doanh thu", VariableType.Number, null, null, null, "%", false, 14),
        new(templateId, "penaltyAmount", "Tiền phạt vi phạm", VariableType.Currency, null, null, null, "VNĐ", false, 15),
    ];

    private static List<ContractTemplateVariable> BuildOrgVariables(Guid templateId) =>
    [
        // Date & Time
        new(templateId, "day", "Ngày", VariableType.Number, null, null, null, null, true, 0),
        new(templateId, "month", "Tháng", VariableType.Number, null, null, null, null, true, 1),
        new(templateId, "year", "Năm", VariableType.Number, null, null, null, null, true, 2),
        new(templateId, "paymentDay", "Ngày thanh toán hàng tháng", VariableType.Number, "Ngày trong tháng dùng để thanh toán", null, null, null, false, 3),
        // Organization info
        new(templateId, "orgName", "Tên tổ chức / phòng khám", VariableType.Text, null, null, null, null, true, 4),
        new(templateId, "orgLicenseNumber", "Số đăng ký kinh doanh", VariableType.Text, null, null, null, null, false, 5),
        new(templateId, "orgAddress", "Địa chỉ tổ chức", VariableType.Text, null, null, null, null, false, 6),
        new(templateId, "orgPhone", "Số điện thoại", VariableType.Text, null, null, null, null, false, 7),
        new(templateId, "orgEmail", "Email liên hệ", VariableType.Text, null, null, null, null, false, 8),
        new(templateId, "orgRepresentative", "Họ tên người đại diện pháp lý", VariableType.Text, null, null, null, null, true, 9),
        new(templateId, "orgPosition", "Chức vụ người đại diện", VariableType.Text, null, null, null, null, false, 10),
        // Contract terms
        new(templateId, "contractNumber", "Số hợp đồng", VariableType.Text, null, null, null, null, false, 11),
        new(templateId, "contractDuration", "Thời hạn hợp đồng", VariableType.Number, null, null, null, "tháng", false, 12),
        new(templateId, "revenueShare", "Tỷ lệ chia sẻ doanh thu", VariableType.Number, null, null, null, "%", false, 13),
        new(templateId, "penaltyAmount", "Tiền phạt vi phạm", VariableType.Currency, null, null, null, "VNĐ", false, 14),
    ];
}
