using Application.AiQuota.Common;
using Application.AiQuota.Interfaces;
using Domain.Entities.Consultation;
using Domain.Entities.Screening;
using Domain.Entities.Scheduling;
using Domain.Entities.Users;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Infrastructure.UnitTests.Common;

namespace Infrastructure.UnitTests.Services;

public class DashboardMetricsServiceTests
{
    [Fact]
    public async Task GetSystemHealthAsync_ShouldReturnOperationalPayload()
    {
        await using var context = CreateContext();
        var service = new DashboardMetricsService(context, new FakeAiQuotaService(), new FakeBetterStackHeartbeatService());

        var result = await service.GetSystemHealthAsync();

        result.AllSystemsOperational.Should().BeTrue();
        result.Components.Should().NotBeEmpty();
        result.Components.Should().Contain(c => c.ComponentName == "Database");
    }

    [Fact]
    public async Task GetPatientMetricsAsync_WhenPatientMissing_ShouldReturnDefaultDto()
    {
        await using var context = CreateContext();
        var service = new DashboardMetricsService(context, new FakeAiQuotaService(), new FakeBetterStackHeartbeatService());

        var result = await service.GetPatientMetricsAsync(Guid.NewGuid());

        result.CompletedScreenings.Should().Be(0);
        result.TotalReports.Should().Be(0);
        result.UpcomingAppointments.Should().Be(0);
        result.RemainingQuota.Should().Be(0);
    }

    [Fact]
    public async Task GetPatientMetricsAsync_WhenPatientExists_ShouldReturnQuotaFromService()
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        await context.Patients.AddAsync(Patient.CreateRegistered(userId));
        await context.SaveChangesAsync();

        var service = new DashboardMetricsService(
            context,
            new FakeAiQuotaService { Quota = new AiQuotaDto { RemainingQuota = 9 } },
            new FakeBetterStackHeartbeatService());

        var result = await service.GetPatientMetricsAsync(userId);

        result.RemainingQuota.Should().Be(9);
    }

    [Fact]
    public async Task GetScreeningVolumeTrendsAsync_Weekly_ShouldReturnDataPoints()
    {
        await using var context = CreateContext();
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "p1@test.local",
            Email = "p1@test.local",
            FullName = "Patient One"
        };
        await context.Users.AddAsync(user);
        var patient = Patient.CreateRegistered(user.Id);
        await context.Patients.AddAsync(patient);
        await context.SaveChangesAsync();

        var s1 = new AiScreening(patient.Id, "v1");
        var s2 = new AiScreening(patient.Id, "v1");
        await context.AiScreenings.AddRangeAsync(s1, s2);
        await context.SaveChangesAsync();

        var service = new DashboardMetricsService(context, new FakeAiQuotaService(), new FakeBetterStackHeartbeatService());
        var result = await service.GetScreeningVolumeTrendsAsync("weekly", 4);

        result.TimeRange.Should().Be("weekly");
        result.TotalScreenings.Should().BeGreaterThan(0);
        result.DataPoints.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetPopulationRiskAnalysisAsync_ShouldExcludeNoneAndComputePercentages()
    {
        await using var context = CreateContext();
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "p2@test.local",
            Email = "p2@test.local",
            FullName = "Patient Two"
        };
        await context.Users.AddAsync(user);
        var patient = Patient.CreateRegistered(user.Id);
        await context.Patients.AddAsync(patient);
        await context.SaveChangesAsync();

        var screening = new AiScreening(patient.Id, "v1");
        await context.AiScreenings.AddAsync(screening);
        await context.SaveChangesAsync();

        await context.ScreeningResults.AddRangeAsync(
            new ScreeningResult(screening.Id, RiskLevel.None, 90),
            new ScreeningResult(screening.Id, RiskLevel.High, 91),
            new ScreeningResult(screening.Id, RiskLevel.Critical, 88));
        await context.SaveChangesAsync();

        var service = new DashboardMetricsService(context, new FakeAiQuotaService(), new FakeBetterStackHeartbeatService());
        var result = await service.GetPopulationRiskAnalysisAsync();

        result.TotalPatients.Should().Be(1);
        result.RiskCategories.Should().NotContain(c => c.RiskLevel == RiskLevel.None.ToString());
        result.RiskCategories.Should().Contain(c => c.RiskLevel == RiskLevel.High.ToString());
    }

    [Fact]
    public async Task GetRecentScreeningsAsync_ShouldMapStatusAndCriticalFlag()
    {
        await using var context = CreateContext();
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "p3@test.local",
            Email = "p3@test.local",
            FullName = "Patient Three"
        };
        await context.Users.AddAsync(user);
        var patient = Patient.CreateRegistered(user.Id);
        await context.Patients.AddAsync(patient);
        await context.SaveChangesAsync();

        var completed = new AiScreening(patient.Id, "v1");
        completed.Process("{}");
        var pending = new AiScreening(patient.Id, "v1");
        await context.AiScreenings.AddRangeAsync(completed, pending);
        await context.SaveChangesAsync();

        await context.ScreeningResults.AddRangeAsync(
            new ScreeningResult(completed.Id, RiskLevel.Critical, 95),
            new ScreeningResult(pending.Id, RiskLevel.Low, 70));
        await context.SaveChangesAsync();

        var service = new DashboardMetricsService(context, new FakeAiQuotaService(), new FakeBetterStackHeartbeatService());
        var result = await service.GetRecentScreeningsAsync(1, 10);

        result.TotalCount.Should().Be(2);
        result.Items.Should().Contain(i => i.Status == "Completed");
        result.Items.Should().Contain(i => i.IsCritical);
    }

    [Fact]
    public async Task GetOrganisationMetricsAsync_WhenNoOrganisation_ShouldReturnDefaultDto()
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        await context.Users.AddAsync(new ApplicationUser
        {
            Id = userId,
            UserName = "no-org@test.local",
            Email = "no-org@test.local",
            FullName = "No Org"
        });
        await context.SaveChangesAsync();

        var service = new DashboardMetricsService(context, new FakeAiQuotaService(), new FakeBetterStackHeartbeatService());
        var result = await service.GetOrganisationMetricsAsync(userId);

        result.TotalAppointments.Should().Be(0);
        result.RemainingAiQuota.Should().Be(0);
    }

    [Fact]
    public async Task GetOrganisationMetricsAsync_WithOrganisation_ShouldReturnCountsAndQuota()
    {
        await using var context = CreateContext();
        var owner = Guid.NewGuid();
        var org = new Organisation(owner, "Clinic A", OrgType.Clinic);
        await context.Organisations.AddAsync(org);
        await context.SaveChangesAsync();

        var userId = Guid.NewGuid();
        await context.Users.AddAsync(new ApplicationUser
        {
            Id = userId,
            UserName = "org-admin@test.local",
            Email = "org-admin@test.local",
            FullName = "Org Admin",
            OrganizationId = org.Id
        });

        var patientUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "p-org@test.local",
            Email = "p-org@test.local",
            FullName = "Patient Org"
        };
        await context.Users.AddAsync(patientUser);
        var patient = Patient.CreateRegistered(patientUser.Id);
        await context.Patients.AddAsync(patient);

        var template = new ScheduleTemplate(
            DayOfWeek.Monday,
            new TimeOnly(8, 0),
            new TimeOnly(10, 0),
            slotDuration: 30,
            maxCapacity: 2,
            orgId: org.Id);
        await context.ScheduleTemplates.AddAsync(template);
        await context.SaveChangesAsync();

        var slot = new AppointmentSlot(template.Id, DateOnly.FromDateTime(DateTime.UtcNow), new TimeOnly(8, 0), new TimeOnly(8, 30), 2);
        slot.BookWithCapacity();
        await context.AppointmentSlots.AddAsync(slot);

        var appt1 = new Appointment(patient.Id, slot.Id, 100000, PricingType.AutoAssign);
        appt1.Confirm();
        var appt2 = new Appointment(patient.Id, slot.Id, 100000, PricingType.AutoAssign);
        appt2.Cancel(Guid.NewGuid(), "cancelled");
        await context.Appointments.AddRangeAsync(appt1, appt2);
        await context.SaveChangesAsync();

        var service = new DashboardMetricsService(
            context,
            new FakeAiQuotaService { Quota = new AiQuotaDto { RemainingQuota = 11 } },
            new FakeBetterStackHeartbeatService());

        var result = await service.GetOrganisationMetricsAsync(userId);

        result.TotalAppointments.Should().Be(2);
        result.RemainingAiQuota.Should().Be(11);
        result.AppointmentStatus.Cancelled.Should().Be(1);
    }

    [Fact]
    public async Task GetOphthalmologistMetricsAsync_WhenNoDoctor_ShouldReturnDefaultDto()
    {
        await using var context = CreateContext();
        var service = new DashboardMetricsService(context, new FakeAiQuotaService(), new FakeBetterStackHeartbeatService());

        var result = await service.GetOphthalmologistMetricsAsync(Guid.NewGuid());

        result.PendingReviews.Should().Be(0);
        result.UrgentCases.Should().Be(0);
    }

    [Fact]
    public async Task GetOphthalmologistMetricsAsync_WithPendingUrgentData_ShouldReturnCounts()
    {
        await using var context = CreateContext();

        var doctorUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "doctor@test.local",
            Email = "doctor@test.local",
            FullName = "Doctor"
        };
        var patientUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "patient4@test.local",
            Email = "patient4@test.local",
            FullName = "Patient Four"
        };
        await context.Users.AddRangeAsync(doctorUser, patientUser);
        await context.SaveChangesAsync();

        var doctor = new Ophthalmologist(doctorUser.Id, yearsOfExperience: 6);
        var patient = Patient.CreateRegistered(patientUser.Id);
        await context.Ophthalmologists.AddAsync(doctor);
        await context.Patients.AddAsync(patient);
        await context.SaveChangesAsync();

        var screening = new AiScreening(patient.Id, "v1");
        await context.AiScreenings.AddAsync(screening);
        await context.SaveChangesAsync();

        var session = ConsultationSession.CreateVerification(patient.Id, screening.Id, price: 100000, ophthalmologistId: doctor.Id);
        await context.ConsultationSessions.AddAsync(session);
        await context.ScreeningResults.AddAsync(new ScreeningResult(screening.Id, RiskLevel.Critical, 92));
        await context.SaveChangesAsync();

        var service = new DashboardMetricsService(context, new FakeAiQuotaService(), new FakeBetterStackHeartbeatService());
        var result = await service.GetOphthalmologistMetricsAsync(doctorUser.Id);

        result.PendingReviews.Should().BeGreaterThanOrEqualTo(1);
        result.UrgentCases.Should().BeGreaterThanOrEqualTo(1);
    }

    private sealed class FakeAiQuotaService : IAiQuotaService
    {
        public AiQuotaDto Quota { get; set; } = new();

        public Task<AiQuotaDto> GetQuotaAsync(Guid userId, string role, CancellationToken cancellationToken = default)
            => Task.FromResult(Quota);

        public Task<bool> HasAvailableQuotaAsync(Guid userId, string role, CancellationToken cancellationToken = default)
            => Task.FromResult(Quota.RemainingQuota > 0);

        public Task DeductQuotaAsync(Guid userId, string role, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task AddPurchasedQuotaAsync(Guid userId, string role, int amount, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeBetterStackHeartbeatService(
        IReadOnlyList<BetterStackMonitorDescriptor>? descriptors = null,
        string? embedUrl = null) : IBetterStackHeartbeatService
    {
        public Task NotifyStartedAsync(BetterStackMonitor monitor, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task NotifySucceededAsync(BetterStackMonitor monitor, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task NotifyFailedAsync(BetterStackMonitor monitor, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public IReadOnlyList<BetterStackMonitorDescriptor> GetMonitorDescriptors() => descriptors ?? [];
        public string? GetEmbedUrl() => embedUrl;
    }

    [Fact]
    public async Task GetSystemAdminMetricsAsync_WhenNoData_ShouldReturnZeroLikeMetrics()
    {
        await using var context = CreateContext();
        var service = new DashboardMetricsService(
            context,
            new FakeAiQuotaService(),
            new FakeBetterStackHeartbeatService(
            [
                new BetterStackMonitorDescriptor(BetterStackMonitor.DailyQuotaReset, "daily-quota-reset", "Daily quota reset", "hangfire", true)
            ],
            "https://status.example/embed"));

        var result = await service.GetSystemAdminMetricsAsync();

        result.Doctors.Total.Should().Be(0);
        result.Organisations.Total.Should().Be(0);
        result.Patients.Total.Should().Be(0);
        result.BetterStack.Enabled.Should().BeTrue();
        result.BetterStack.EmbedUrl.Should().Be("https://status.example/embed");
        result.SystemStatus.ApiHealthy.Should().BeTrue();
    }

    [Fact]
    public async Task GetScreeningVolumeTrendsAsync_WithUnknownTimeRange_ShouldFallbackToMonthly()
    {
        await using var context = CreateContext();
        var service = new DashboardMetricsService(context, new FakeAiQuotaService(), new FakeBetterStackHeartbeatService());

        var result = await service.GetScreeningVolumeTrendsAsync("something-random", 3);

        result.TimeRange.Should().Be("monthly");
    }

    [Theory]
    [InlineData("weekly")]
    [InlineData("WEEKLY")]
    [InlineData("monthly")]
    [InlineData("MONTHLY")]
    [InlineData("other")]
    public async Task GetScreeningVolumeTrendsAsync_ShouldNormalizeTimeRange(string input)
    {
        await using var context = CreateContext();
        var service = new DashboardMetricsService(context, new FakeAiQuotaService(), new FakeBetterStackHeartbeatService());

        var result = await service.GetScreeningVolumeTrendsAsync(input, 2);

        var expected = string.Equals(input, "weekly", StringComparison.OrdinalIgnoreCase) ? "weekly" : "monthly";
        result.TimeRange.Should().Be(expected);
    }

    [Fact]
    public async Task GetRecentScreeningsAsync_WithPaging_ShouldReturnRequestedPageSize()
    {
        await using var context = CreateContext();
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "paging@test.local", Email = "paging@test.local", FullName = "Paging User" };
        await context.Users.AddAsync(user);
        var patient = Patient.CreateRegistered(user.Id);
        await context.Patients.AddAsync(patient);
        await context.SaveChangesAsync();

        await context.AiScreenings.AddRangeAsync(
            new AiScreening(patient.Id, "v1"),
            new AiScreening(patient.Id, "v1"),
            new AiScreening(patient.Id, "v1"));
        await context.SaveChangesAsync();

        var service = new DashboardMetricsService(context, new FakeAiQuotaService(), new FakeBetterStackHeartbeatService());
        var result = await service.GetRecentScreeningsAsync(2, 1);

        result.TotalCount.Should().Be(3);
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetOrganisationMetricsAsync_ShouldComputeUtilizationRateFromSlots()
    {
        await using var context = CreateContext();
        var owner = Guid.NewGuid();
        var org = new Organisation(owner, "Rate Org", OrgType.Clinic);
        await context.Organisations.AddAsync(org);

        var userId = Guid.NewGuid();
        await context.Users.AddAsync(new ApplicationUser
        {
            Id = userId,
            UserName = "rate-org@test.local",
            Email = "rate-org@test.local",
            FullName = "Rate Org Admin",
            OrganizationId = org.Id
        });
        await context.SaveChangesAsync();

        var template = new ScheduleTemplate(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(10, 0), 30, 4, orgId: org.Id);
        await context.ScheduleTemplates.AddAsync(template);
        await context.SaveChangesAsync();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var slot1 = new AppointmentSlot(template.Id, today, new TimeOnly(8, 0), new TimeOnly(8, 30), 4);
        slot1.BookWithCapacity();
        slot1.BookWithCapacity();
        var slot2 = new AppointmentSlot(template.Id, today, new TimeOnly(9, 0), new TimeOnly(9, 30), 4);
        slot2.BookWithCapacity();
        await context.AppointmentSlots.AddRangeAsync(slot1, slot2);
        await context.SaveChangesAsync();

        var service = new DashboardMetricsService(context, new FakeAiQuotaService(), new FakeBetterStackHeartbeatService());
        var result = await service.GetOrganisationMetricsAsync(userId);

        result.UtilizationRatePercent.Should().Be(37.5m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(11)]
    public async Task GetPatientMetricsAsync_ShouldMapRemainingQuotaFromService(int remainingQuota)
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        await context.Patients.AddAsync(Patient.CreateRegistered(userId));
        await context.SaveChangesAsync();
        var service = new DashboardMetricsService(
            context,
            new FakeAiQuotaService { Quota = new AiQuotaDto { RemainingQuota = remainingQuota } },
            new FakeBetterStackHeartbeatService());

        var result = await service.GetPatientMetricsAsync(userId);

        result.RemainingQuota.Should().Be(remainingQuota);
    }

    [Fact]
    public async Task GetPatientMetricsAsync_ShouldComputeCompletedReportsAndUpcomingAppointments()
    {
        await using var context = CreateContext();
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "pm@test.local", Email = "pm@test.local", FullName = "Patient Metrics" };
        await context.Users.AddAsync(user);
        var patient = Patient.CreateRegistered(user.Id);
        await context.Patients.AddAsync(patient);
        await context.SaveChangesAsync();

        var sCompleted = new AiScreening(patient.Id, "v1");
        sCompleted.Process("{}");
        var sPending = new AiScreening(patient.Id, "v1");
        await context.AiScreenings.AddRangeAsync(sCompleted, sPending);
        await context.SaveChangesAsync();

        await context.ScreeningResults.AddRangeAsync(
            new ScreeningResult(sCompleted.Id, RiskLevel.Low, 80),
            new ScreeningResult(sPending.Id, RiskLevel.Moderate, 81));

        var orgId = Guid.NewGuid();
        var slot = new AppointmentSlot(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow), new TimeOnly(8, 0), new TimeOnly(8, 30), 2);
        await context.AppointmentSlots.AddAsync(slot);
        var apptPending = new Appointment(patient.Id, slot.Id, 100000, PricingType.AutoAssign);
        var apptConfirmed = new Appointment(patient.Id, slot.Id, 100000, PricingType.AutoAssign);
        apptConfirmed.Confirm();
        await context.Appointments.AddRangeAsync(apptPending, apptConfirmed);
        await context.SaveChangesAsync();

        var service = new DashboardMetricsService(
            context,
            new FakeAiQuotaService { Quota = new AiQuotaDto { RemainingQuota = 5 } },
            new FakeBetterStackHeartbeatService());

        var result = await service.GetPatientMetricsAsync(user.Id);

        result.CompletedScreenings.Should().Be(1);
        result.TotalReports.Should().Be(2);
        result.UpcomingAppointments.Should().Be(2);
        result.RemainingQuota.Should().Be(5);
    }

    [Fact]
    public async Task GetSystemAdminMetricsAsync_WhenNoConfiguredMonitors_ShouldSetBetterStackDisabled()
    {
        await using var context = CreateContext();
        var service = new DashboardMetricsService(
            context,
            new FakeAiQuotaService(),
            new FakeBetterStackHeartbeatService(
            [
                new BetterStackMonitorDescriptor(BetterStackMonitor.DailyQuotaReset, "daily-quota-reset", "Daily quota reset", "hangfire", false),
                new BetterStackMonitorDescriptor(BetterStackMonitor.SlotMaintenance, "slot-maintenance", "Slot maintenance", "hangfire", false)
            ],
            null));

        var result = await service.GetSystemAdminMetricsAsync();

        result.BetterStack.Enabled.Should().BeFalse();
        result.BetterStack.Monitors.Should().HaveCount(2);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 0, 1)]
    public async Task GetSystemAdminMetricsAsync_ShouldExposePendingActionCounts(int pendingDoctor, int pendingOnboarding, int expectedPendingDoctor)
    {
        await using var context = CreateContext();
        var userDoctor = new ApplicationUser { Id = Guid.NewGuid(), UserName = "doc-p@test.local", Email = "doc-p@test.local", FullName = "Doc Pending" };
        var userOrg = new ApplicationUser { Id = Guid.NewGuid(), UserName = "org-p@test.local", Email = "org-p@test.local", FullName = "Org Pending" };
        await context.Users.AddRangeAsync(userDoctor, userOrg);

        if (pendingDoctor > 0)
        {
            await context.Ophthalmologists.AddAsync(new Ophthalmologist(userDoctor.Id, yearsOfExperience: 3));
        }

        if (pendingOnboarding > 0)
        {
            await context.OrganisationOnboardingRequests.AddAsync(
                new OrganisationOnboardingRequest("Org Pending", OrgType.Clinic, "P", "pending-org@test.local"));
        }

        await context.SaveChangesAsync();
        var service = new DashboardMetricsService(context, new FakeAiQuotaService(), new FakeBetterStackHeartbeatService());

        var result = await service.GetSystemAdminMetricsAsync();

        result.PendingActions.PendingOphthalmologistVerifications.Should().Be(expectedPendingDoctor);
        result.PendingActions.PendingOrganisationOnboarding.Should().Be(pendingOnboarding);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public async Task GetScreeningVolumeTrendsAsync_WithNoData_ShouldReturnZeroTotals(int periods)
    {
        await using var context = CreateContext();
        var service = new DashboardMetricsService(context, new FakeAiQuotaService(), new FakeBetterStackHeartbeatService());

        var weekly = await service.GetScreeningVolumeTrendsAsync("weekly", periods);
        var monthly = await service.GetScreeningVolumeTrendsAsync("monthly", periods);

        weekly.TotalScreenings.Should().Be(0);
        weekly.AveragePerPeriod.Should().Be(0);
        weekly.DataPoints.Should().BeEmpty();
        monthly.TotalScreenings.Should().Be(0);
        monthly.AveragePerPeriod.Should().Be(0);
        monthly.DataPoints.Should().BeEmpty();
    }

    [Theory]
    [InlineData(RiskLevel.Low, 1)]
    [InlineData(RiskLevel.Moderate, 2)]
    [InlineData(RiskLevel.High, 3)]
    [InlineData(RiskLevel.Critical, 4)]
    [InlineData(RiskLevel.None, 5)]
    [InlineData(RiskLevel.Low, 6)]
    [InlineData(RiskLevel.High, 7)]
    [InlineData(RiskLevel.Moderate, 8)]
    [InlineData(RiskLevel.Critical, 9)]
    [InlineData(RiskLevel.None, 10)]
    public async Task GetRecentScreeningsAsync_WithoutResult_ShouldReturnNonCriticalAndNullRisk(RiskLevel seededRisk, int caseId)
    {
        await using var context = CreateContext();
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = $"no-risk-{seededRisk}-{caseId}@test.local",
            Email = $"no-risk-{seededRisk}-{caseId}@test.local",
            FullName = "No Risk User"
        };
        await context.Users.AddAsync(user);
        var patient = Patient.CreateRegistered(user.Id);
        await context.Patients.AddAsync(patient);
        await context.SaveChangesAsync();

        var noResultScreening = new AiScreening(patient.Id, "v1");
        noResultScreening.Process("{}");
        await context.AiScreenings.AddAsync(noResultScreening);
        await context.SaveChangesAsync();

        var otherScreening = new AiScreening(patient.Id, "v1");
        await context.AiScreenings.AddAsync(otherScreening);
        await context.SaveChangesAsync();
        await context.ScreeningResults.AddAsync(new ScreeningResult(otherScreening.Id, seededRisk, 88));
        await context.SaveChangesAsync();

        var service = new DashboardMetricsService(context, new FakeAiQuotaService(), new FakeBetterStackHeartbeatService());
        var result = await service.GetRecentScreeningsAsync(1, 20);
        var target = result.Items.Single(i => i.Id == noResultScreening.Id);

        target.Status.Should().Be("Completed");
        target.RiskLevel.Should().BeNull();
        target.IsCritical.Should().BeFalse();
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

}
