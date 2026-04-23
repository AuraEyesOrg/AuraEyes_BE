using Application.SystemSettings.Interfaces;
using Domain.Entities.Users;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Infrastructure.UnitTests.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitTests.Services;

public class AiQuotaServiceTests
{
    [Fact]
    public async Task GetQuotaAsync_ForPatient_ShouldReturnComputedQuota()
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        var patient = new Patient(userId);
        patient.AddPurchasedQuota(5);
        patient.ConsumeQuota(3); // used=1
        await context.Patients.AddAsync(patient);
        await context.SaveChangesAsync();

        var service = CreateService(context, new Dictionary<string, string>
        {
            ["FREE_AI_QUOTA"] = "3",
            ["AI_QUOTA_UNIT_PRICE"] = "12000"
        });

        var quota = await service.GetQuotaAsync(userId, "Patient");

        quota.TotalQuota.Should().Be(8);
        quota.UsedQuota.Should().Be(1);
        quota.RemainingQuota.Should().Be(7);
        quota.QuotaSource.Should().Be("Free");
        quota.UnitPrice.Should().Be(12000m);
    }

    [Fact]
    public async Task GetQuotaAsync_ForUnknownRole_ShouldReturnNoneQuota()
    {
        await using var context = CreateContext();
        var service = CreateService(context, new Dictionary<string, string>());

        var quota = await service.GetQuotaAsync(Guid.NewGuid(), "UnknownRole");

        quota.TotalQuota.Should().Be(0);
        quota.RemainingQuota.Should().Be(0);
        quota.QuotaSource.Should().Be("None");
    }

    [Fact]
    public async Task GetQuotaAsync_ForOrgRole_ShouldReadFromOrganisation()
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var org = new Organisation(ownerId, "Org A", OrgType.Hospital);
        org.AddPurchasedQuota(6);
        org.ConsumeQuota(3); // used=1
        await context.Organisations.AddAsync(org);
        await context.SaveChangesAsync();

        var user = new ApplicationUser
        {
            Id = userId,
            UserName = "orgadmin@test.com",
            Email = "orgadmin@test.com",
            FullName = "Org Admin",
            OrganizationId = org.Id
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var service = CreateService(context, new Dictionary<string, string>
        {
            ["FREE_AI_QUOTA"] = "3",
            ["AI_QUOTA_UNIT_PRICE"] = "10000"
        });

        var quota = await service.GetQuotaAsync(userId, "OrgAdmin");

        quota.TotalQuota.Should().Be(9);
        quota.UsedQuota.Should().Be(1);
        quota.RemainingQuota.Should().Be(8);
        quota.QuotaSource.Should().Be("Free");
    }

    [Fact]
    public async Task HasAvailableQuotaAsync_ShouldReflectRemainingQuota()
    {
        await using var context = CreateContext();
        var service = CreateService(context, new Dictionary<string, string> { ["FREE_AI_QUOTA"] = "0" });

        var hasQuota = await service.HasAvailableQuotaAsync(Guid.NewGuid(), "Patient");

        hasQuota.Should().BeFalse();
    }

    [Fact]
    public async Task DeductQuotaAsync_ForPatient_ShouldIncreaseUsedQuota()
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        var patient = new Patient(userId);
        await context.Patients.AddAsync(patient);
        await context.SaveChangesAsync();

        var service = CreateService(context, new Dictionary<string, string> { ["FREE_AI_QUOTA"] = "3" });
        await service.DeductQuotaAsync(userId, "Patient");

        var updated = await context.Patients.FirstAsync(x => x.UserId == userId);
        updated.UsedAiQuota.Should().Be(1);
    }

    [Fact]
    public async Task AddPurchasedQuotaAsync_ForPatient_ShouldIncreasePurchasedQuota()
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        await context.Patients.AddAsync(new Patient(userId));
        await context.SaveChangesAsync();

        var service = CreateService(context, new Dictionary<string, string>());
        await service.AddPurchasedQuotaAsync(userId, "Patient", 10);

        var updated = await context.Patients.FirstAsync(x => x.UserId == userId);
        updated.PurchasedAiQuota.Should().Be(10);
    }

    [Fact]
    public async Task DeductQuotaAsync_WhenPatientMissing_ShouldThrow()
    {
        await using var context = CreateContext();
        var service = CreateService(context, new Dictionary<string, string>());

        var act = async () => await service.DeductQuotaAsync(Guid.NewGuid(), "Patient");

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Patient not found.*");
    }

    [Fact]
    public async Task AddPurchasedQuotaAsync_WhenOrgRoleWithoutOrg_ShouldThrow()
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        await context.Users.AddAsync(new ApplicationUser
        {
            Id = userId,
            UserName = "noorg@test.com",
            Email = "noorg@test.com",
            FullName = "No Org User"
        });
        await context.SaveChangesAsync();

        var service = CreateService(context, new Dictionary<string, string>());
        var act = async () => await service.AddPurchasedQuotaAsync(userId, "OrgAdmin", 5);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Organisation not found for this user.*");
    }

    [Fact]
    public async Task GetQuotaAsync_WhenUnitPriceSettingInvalidOrNegative_ShouldUseDefaultUnitPrice()
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        await context.Patients.AddAsync(new Patient(userId));
        await context.SaveChangesAsync();

        var service = CreateService(context, new Dictionary<string, string>
        {
            ["FREE_AI_QUOTA"] = "3",
            ["AI_QUOTA_UNIT_PRICE"] = "-1"
        });

        var quota = await service.GetQuotaAsync(userId, "Patient");

        quota.UnitPrice.Should().Be(10000m);
    }

    [Theory]
    [InlineData("UnknownRole")]
    [InlineData("SystemAdmin")]
    [InlineData("")]
    public async Task GetQuotaAsync_WithUnsupportedRole_ShouldReturnNone(string role)
    {
        await using var context = CreateContext();
        var service = CreateService(context, new Dictionary<string, string>());

        var quota = await service.GetQuotaAsync(Guid.NewGuid(), role);

        quota.QuotaSource.Should().Be("None");
        quota.RemainingQuota.Should().Be(0);
    }

    [Theory]
    [InlineData("Patient", 0, false)]
    [InlineData("Patient", 3, true)]
    [InlineData("Patient", 10, true)]
    public async Task HasAvailableQuotaAsync_ShouldMatchConfiguredFreeQuota(string role, int freeQuota, bool expected)
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        await context.Patients.AddAsync(new Patient(userId));
        await context.SaveChangesAsync();

        var service = CreateService(context, new Dictionary<string, string>
        {
            ["FREE_AI_QUOTA"] = freeQuota.ToString()
        });

        var hasQuota = await service.HasAvailableQuotaAsync(userId, role);

        hasQuota.Should().Be(expected);
    }

    [Fact]
    public async Task GetQuotaAsync_WhenPatientMissing_ShouldReturnFreeFallbackQuota()
    {
        await using var context = CreateContext();
        var service = CreateService(context, new Dictionary<string, string>
        {
            ["FREE_AI_QUOTA"] = "4",
            ["AI_QUOTA_UNIT_PRICE"] = "15000"
        });

        var quota = await service.GetQuotaAsync(Guid.NewGuid(), "Patient");

        quota.TotalQuota.Should().Be(4);
        quota.UsedQuota.Should().Be(0);
        quota.RemainingQuota.Should().Be(4);
        quota.QuotaSource.Should().Be("Free");
        quota.UnitPrice.Should().Be(15000m);
    }

    [Fact]
    public async Task GetQuotaAsync_ForOrgRole_WhenUserMissing_ShouldReturnNone()
    {
        await using var context = CreateContext();
        var service = CreateService(context, new Dictionary<string, string>());

        var quota = await service.GetQuotaAsync(Guid.NewGuid(), "OrgAdmin");

        quota.QuotaSource.Should().Be("None");
        quota.RemainingQuota.Should().Be(0);
    }

    [Fact]
    public async Task AddPurchasedQuotaAsync_ForOrgAdmin_ShouldIncreaseOrganisationPurchasedQuota()
    {
        await using var context = CreateContext();
        var ownerId = Guid.NewGuid();
        var org = new Organisation(ownerId, "Grow Org", OrgType.Clinic);
        await context.Organisations.AddAsync(org);
        var userId = Guid.NewGuid();
        await context.Users.AddAsync(new ApplicationUser
        {
            Id = userId,
            UserName = "grow-org-admin@test.local",
            Email = "grow-org-admin@test.local",
            FullName = "Grow Org Admin",
            OrganizationId = org.Id
        });
        await context.SaveChangesAsync();
        var service = CreateService(context, new Dictionary<string, string>());

        await service.AddPurchasedQuotaAsync(userId, "OrgAdmin", 12);

        var updatedOrg = await context.Organisations.FirstAsync(x => x.Id == org.Id);
        updatedOrg.PurchasedAiQuota.Should().Be(12);
    }

    [Fact]
    public async Task GetQuotaAsync_ForOphthalmologistRole_ShouldUseOrganisationQuota()
    {
        await using var context = CreateContext();
        var ownerId = Guid.NewGuid();
        var org = new Organisation(ownerId, "Eye Org", OrgType.Hospital);
        org.AddPurchasedQuota(4);
        await context.Organisations.AddAsync(org);
        var userId = Guid.NewGuid();
        await context.Users.AddAsync(new ApplicationUser
        {
            Id = userId,
            UserName = "oph-admin@test.local",
            Email = "oph-admin@test.local",
            FullName = "Oph User",
            OrganizationId = org.Id
        });
        await context.SaveChangesAsync();
        var service = CreateService(context, new Dictionary<string, string> { ["FREE_AI_QUOTA"] = "3" });

        var quota = await service.GetQuotaAsync(userId, "Ophthalmologist");

        quota.TotalQuota.Should().Be(7);
        quota.RemainingQuota.Should().Be(7);
    }

    [Fact]
    public async Task DeductQuotaAsync_ForOrgAdmin_ShouldIncreaseOrganisationUsedQuota()
    {
        await using var context = CreateContext();
        var ownerId = Guid.NewGuid();
        var org = new Organisation(ownerId, "Deduct Org", OrgType.Clinic);
        await context.Organisations.AddAsync(org);
        var userId = Guid.NewGuid();
        await context.Users.AddAsync(new ApplicationUser
        {
            Id = userId,
            UserName = "deduct-org@test.local",
            Email = "deduct-org@test.local",
            FullName = "Deduct Org Admin",
            OrganizationId = org.Id
        });
        await context.SaveChangesAsync();
        var service = CreateService(context, new Dictionary<string, string> { ["FREE_AI_QUOTA"] = "3" });

        await service.DeductQuotaAsync(userId, "OrgAdmin");

        var updatedOrg = await context.Organisations.FirstAsync(x => x.Id == org.Id);
        updatedOrg.UsedAiQuota.Should().Be(1);
    }

    [Theory]
    [InlineData("OrgAdmin")]
    [InlineData("orgadmin")]
    [InlineData("Ophthalmologist")]
    [InlineData("ophthalmologist")]
    [InlineData("ORGADMIN")]
    [InlineData("OPHTHALMOLOGIST")]
    public async Task GetQuotaAsync_ForOrganisationRoles_ShouldResolveOrgQuota(string role)
    {
        await using var context = CreateContext();
        var ownerId = Guid.NewGuid();
        var org = new Organisation(ownerId, "Role Org", OrgType.Clinic);
        org.AddPurchasedQuota(2);
        await context.Organisations.AddAsync(org);
        var userId = Guid.NewGuid();
        await context.Users.AddAsync(new ApplicationUser
        {
            Id = userId,
            UserName = $"role-{role}@test.local",
            Email = $"role-{role}@test.local",
            FullName = "Role User",
            OrganizationId = org.Id
        });
        await context.SaveChangesAsync();
        var service = CreateService(context, new Dictionary<string, string> { ["FREE_AI_QUOTA"] = "3" });

        var quota = await service.GetQuotaAsync(userId, role);

        quota.TotalQuota.Should().Be(5);
        quota.RemainingQuota.Should().Be(5);
        quota.QuotaSource.Should().Be("Free");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    public async Task AddPurchasedQuotaAsync_ForPatient_WithVariousAmounts_ShouldAccumulateQuota(int amount)
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        await context.Patients.AddAsync(new Patient(userId));
        await context.SaveChangesAsync();
        var service = CreateService(context, new Dictionary<string, string>());

        await service.AddPurchasedQuotaAsync(userId, "Patient", amount);

        var patient = await context.Patients.FirstAsync(x => x.UserId == userId);
        patient.PurchasedAiQuota.Should().Be(amount);
    }

    [Theory]
    [InlineData("OrgAdmin", 0, false)]
    [InlineData("OrgAdmin", 1, true)]
    [InlineData("OrgAdmin", 3, true)]
    [InlineData("Ophthalmologist", 0, false)]
    [InlineData("Ophthalmologist", 1, true)]
    [InlineData("Ophthalmologist", 3, true)]
    [InlineData("orgadmin", 0, false)]
    [InlineData("ophthalmologist", 2, true)]
    public async Task HasAvailableQuotaAsync_ForOrgRoles_ShouldDependOnConfiguredFreeQuota(string role, int freeQuota, bool expected)
    {
        await using var context = CreateContext();
        var ownerId = Guid.NewGuid();
        var org = new Organisation(ownerId, "Availability Org", OrgType.Clinic);
        await context.Organisations.AddAsync(org);
        var userId = Guid.NewGuid();
        await context.Users.AddAsync(new ApplicationUser
        {
            Id = userId,
            UserName = $"org-role-{role}@test.local",
            Email = $"org-role-{role}@test.local",
            FullName = "Org Role User",
            OrganizationId = org.Id
        });
        await context.SaveChangesAsync();
        var service = CreateService(context, new Dictionary<string, string> { ["FREE_AI_QUOTA"] = freeQuota.ToString() });

        var hasQuota = await service.HasAvailableQuotaAsync(userId, role);

        hasQuota.Should().Be(expected);
    }

    private static AiQuotaService CreateService(ApplicationDbContext context, Dictionary<string, string> settings)
    {
        var settingService = new FakeSettingService(settings);
        return new AiQuotaService(context, new TestLogger<AiQuotaService>(), settingService);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private sealed class FakeSettingService(Dictionary<string, string> settings) : ISystemSettingService
    {
        public Task<string?> GetSettingAsync(string key, CancellationToken cancellationToken = default)
            => Task.FromResult(settings.TryGetValue(key, out var v) ? v : null);

        public Task<Dictionary<string, string>> GetAllSettingsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new Dictionary<string, string>(settings));

        public Task UpdateSettingsAsync(Dictionary<string, string> settings, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<(bool Success, int UsedSlots, int Quota, int RemainingSlots)> TryReservePartTimeSlotsAsync(DateOnly date, int quantity, int dailyLimit, CancellationToken cancellationToken = default) => Task.FromResult((true, 0, dailyLimit, dailyLimit));

        public Task<IReadOnlyDictionary<DateOnly, int>> GetPartTimeReservedSlotsByDateRangeAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyDictionary<DateOnly, int>>(new Dictionary<DateOnly, int>());
    }
}
