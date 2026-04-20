using Application.AiQuota.Interfaces;
using Domain.Entities.Consultation;
using Domain.Entities.Platform;
using Domain.Entities.Users;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Infrastructure.UnitTests.Services;

public class DashboardMetricsServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetSystemAdminMetricsAsync_EmptyDb_ShouldReturnZeroCounts()
    {
        await using var ctx = CreateContext();
        var aiQuota = Substitute.For<IAiQuotaService>();
        var heartbeat = Substitute.For<IBetterStackHeartbeatService>();
        var sut = new DashboardMetricsService(ctx, aiQuota, heartbeat);

        var result = await sut.GetSystemAdminMetricsAsync();

        result.Should().NotBeNull();
        result.Doctors.Total.Should().Be(0);
        result.Organisations.Total.Should().Be(0);
        result.Patients.Total.Should().Be(0);
    }

    [Fact]
    public async Task GetRecentScreeningsAsync_ShouldReturnPagedResults()
    {
        await using var ctx = CreateContext();
        var aiQuota = Substitute.For<IAiQuotaService>();
        var heartbeat = Substitute.For<IBetterStackHeartbeatService>();
        var sut = new DashboardMetricsService(ctx, aiQuota, heartbeat);

        var page = await sut.GetRecentScreeningsAsync(1, 10);

        page.Should().NotBeNull();
        page.Items.Should().NotBeNull();
        page.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetScreeningVolumeTrendsAsync_ShouldReturnSeries()
    {
        await using var ctx = CreateContext();
        var aiQuota = Substitute.For<IAiQuotaService>();
        var heartbeat = Substitute.For<IBetterStackHeartbeatService>();
        var sut = new DashboardMetricsService(ctx, aiQuota, heartbeat);

        var trends = await sut.GetScreeningVolumeTrendsAsync("monthly", 6);

        trends.Should().NotBeNull();
    }

    [Fact]
    public async Task GetScreeningVolumeTrendsAsync_InvalidRange_ShouldDefaultToMonthly()
    {
        await using var ctx = CreateContext();
        var aiQuota = Substitute.For<IAiQuotaService>();
        var heartbeat = Substitute.For<IBetterStackHeartbeatService>();
        var sut = new DashboardMetricsService(ctx, aiQuota, heartbeat);

        var trends = await sut.GetScreeningVolumeTrendsAsync("not-a-valid-range", 3);

        trends.Should().NotBeNull();
        trends!.TimeRange.Should().Be("monthly");
    }

    [Fact]
    public async Task GetPopulationRiskAnalysisAsync_EmptyDb_ShouldReturnDto()
    {
        await using var ctx = CreateContext();
        var aiQuota = Substitute.For<IAiQuotaService>();
        var heartbeat = Substitute.For<IBetterStackHeartbeatService>();
        var sut = new DashboardMetricsService(ctx, aiQuota, heartbeat);

        var analysis = await sut.GetPopulationRiskAnalysisAsync();

        analysis.Should().NotBeNull();
    }

    [Fact]
    public async Task GetSystemHealthAsync_ShouldReturnStatus()
    {
        await using var ctx = CreateContext();
        var aiQuota = Substitute.For<IAiQuotaService>();
        var heartbeat = Substitute.For<IBetterStackHeartbeatService>();
        var sut = new DashboardMetricsService(ctx, aiQuota, heartbeat);

        var health = await sut.GetSystemHealthAsync();

        health.Should().NotBeNull();
    }

    [Fact]
    public async Task GetOphthalmologistMetricsAsync_WithOphthalId_ShouldReturnDto()
    {
        await using var ctx = CreateContext();
        var aiQuota = Substitute.For<IAiQuotaService>();
        var heartbeat = Substitute.For<IBetterStackHeartbeatService>();
        var sut = new DashboardMetricsService(ctx, aiQuota, heartbeat);

        var metrics = await sut.GetOphthalmologistMetricsAsync(Guid.NewGuid());

        metrics.Should().NotBeNull();
    }

    [Fact]
    public async Task GetDoctorWorkloadAsync_NoSessions_ShouldReturnZeroActualHours()
    {
        await using var ctx = CreateContext();
        var doctorId = await SeedDoctorAsync(ctx, OphthalmologistEmploymentType.FullTime, "Doctor Zero Sessions");
        ctx.WorkloadRequirements.Add(new WorkloadRequirement(
            OphthalmologistEmploymentType.FullTime,
            WorkloadPeriodType.Week,
            40m));
        await ctx.SaveChangesAsync();

        var sut = new DashboardMetricsService(
            ctx,
            Substitute.For<IAiQuotaService>(),
            Substitute.For<IBetterStackHeartbeatService>());

        var result = await sut.GetDoctorWorkloadAsync(
            doctorId,
            WorkloadPeriodType.Week,
            new DateOnly(2026, 4, 20));

        result.Should().NotBeNull();
        result!.ActualHours.Should().Be(0m);
        result.RequiredHours.Should().Be(40m);
        result.Status.Should().Be("UNDER");
    }

    [Fact]
    public async Task GetDoctorWorkloadAsync_ExactThreshold_ShouldReturnOkStatus()
    {
        await using var ctx = CreateContext();
        var doctorId = await SeedDoctorAsync(ctx, OphthalmologistEmploymentType.FullTime, "Doctor Exact Threshold");
        ctx.WorkloadRequirements.Add(new WorkloadRequirement(
            OphthalmologistEmploymentType.FullTime,
            WorkloadPeriodType.Week,
            40m));

        AddCompletedSession(ctx, doctorId, Utc(2026, 4, 20, 1, 0), Utc(2026, 4, 20, 11, 0));
        AddCompletedSession(ctx, doctorId, Utc(2026, 4, 21, 1, 0), Utc(2026, 4, 21, 11, 0));
        AddCompletedSession(ctx, doctorId, Utc(2026, 4, 22, 1, 0), Utc(2026, 4, 22, 11, 0));
        AddCompletedSession(ctx, doctorId, Utc(2026, 4, 23, 1, 0), Utc(2026, 4, 23, 11, 0));
        await ctx.SaveChangesAsync();

        var sut = new DashboardMetricsService(
            ctx,
            Substitute.For<IAiQuotaService>(),
            Substitute.For<IBetterStackHeartbeatService>());

        var result = await sut.GetDoctorWorkloadAsync(
            doctorId,
            WorkloadPeriodType.Week,
            new DateOnly(2026, 4, 20));

        result.Should().NotBeNull();
        result!.ActualHours.Should().Be(40m);
        result.RequiredHours.Should().Be(40m);
        result.CompletionRate.Should().Be(1m);
        result.Status.Should().Be("OK");
    }

    [Fact]
    public async Task GetDoctorWorkloadAsync_FullTimeOverrideSetting_ShouldUseConfiguredRequiredHours()
    {
        await using var ctx = CreateContext();
        var doctorId = await SeedDoctorAsync(ctx, OphthalmologistEmploymentType.FullTime, "Doctor Override Threshold");
        ctx.WorkloadRequirements.Add(new WorkloadRequirement(
            OphthalmologistEmploymentType.FullTime,
            WorkloadPeriodType.Week,
            40m));
        ctx.SystemSettings.Add(new SystemSetting("FULLTIME_REQUIRED_HOURS_WEEK", "6"));

        AddCompletedSession(ctx, doctorId, Utc(2026, 4, 20, 2, 0), Utc(2026, 4, 20, 10, 0));
        await ctx.SaveChangesAsync();

        var sut = new DashboardMetricsService(
            ctx,
            Substitute.For<IAiQuotaService>(),
            Substitute.For<IBetterStackHeartbeatService>());

        var result = await sut.GetDoctorWorkloadAsync(
            doctorId,
            WorkloadPeriodType.Week,
            new DateOnly(2026, 4, 20));

        result.Should().NotBeNull();
        result!.ActualHours.Should().Be(8m);
        result.RequiredHours.Should().Be(6m);
        result.Status.Should().Be("OK");
    }

    [Fact]
    public async Task GetDoctorWorkloadsAsync_FullTimeOverrideSetting_ShouldUseConfiguredRequiredHours()
    {
        await using var ctx = CreateContext();
        var doctorId = await SeedDoctorAsync(ctx, OphthalmologistEmploymentType.FullTime, "Doctor Override List");
        ctx.WorkloadRequirements.Add(new WorkloadRequirement(
            OphthalmologistEmploymentType.FullTime,
            WorkloadPeriodType.Week,
            40m));
        ctx.SystemSettings.Add(new SystemSetting("FULLTIME_REQUIRED_HOURS_WEEK", "6"));

        AddCompletedSession(ctx, doctorId, Utc(2026, 4, 20, 2, 0), Utc(2026, 4, 20, 10, 0));
        await ctx.SaveChangesAsync();

        var sut = new DashboardMetricsService(
            ctx,
            Substitute.For<IAiQuotaService>(),
            Substitute.For<IBetterStackHeartbeatService>());

        var result = await sut.GetDoctorWorkloadsAsync(
            WorkloadPeriodType.Week,
            new DateOnly(2026, 4, 20),
            null,
            null,
            null,
            false,
            1,
            10);

        result.Items.Should().ContainSingle();
        var item = result.Items.Single();
        item.DoctorId.Should().Be(doctorId);
        item.RequiredHours.Should().Be(6m);
        item.ActualHours.Should().Be(8m);
        item.Status.Should().Be("OK");
    }

    [Fact]
    public async Task GetDoctorWorkloadAsync_BelowThreshold_ShouldReturnUnderStatus()
    {
        await using var ctx = CreateContext();
        var doctorId = await SeedDoctorAsync(ctx, OphthalmologistEmploymentType.FullTime, "Doctor Under Threshold");
        ctx.WorkloadRequirements.Add(new WorkloadRequirement(
            OphthalmologistEmploymentType.FullTime,
            WorkloadPeriodType.Week,
            40m));

        AddCompletedSession(ctx, doctorId, Utc(2026, 4, 20, 2, 0), Utc(2026, 4, 20, 10, 0));
        await ctx.SaveChangesAsync();

        var sut = new DashboardMetricsService(
            ctx,
            Substitute.For<IAiQuotaService>(),
            Substitute.For<IBetterStackHeartbeatService>());

        var result = await sut.GetDoctorWorkloadAsync(
            doctorId,
            WorkloadPeriodType.Week,
            new DateOnly(2026, 4, 20));

        result.Should().NotBeNull();
        result!.ActualHours.Should().Be(8m);
        result.Status.Should().Be("UNDER");
    }

    [Fact]
    public async Task GetDoctorWorkloadAsync_SessionsOutsidePeriod_ShouldNotBeCounted()
    {
        await using var ctx = CreateContext();
        var doctorId = await SeedDoctorAsync(ctx, OphthalmologistEmploymentType.FullTime, "Doctor Outside Period");
        ctx.WorkloadRequirements.Add(new WorkloadRequirement(
            OphthalmologistEmploymentType.FullTime,
            WorkloadPeriodType.Week,
            10m));

        AddCompletedSession(ctx, doctorId, Utc(2026, 4, 10, 1, 0), Utc(2026, 4, 10, 3, 0));
        AddCompletedSession(ctx, doctorId, Utc(2026, 4, 21, 1, 0), Utc(2026, 4, 21, 5, 0));
        await ctx.SaveChangesAsync();

        var sut = new DashboardMetricsService(
            ctx,
            Substitute.For<IAiQuotaService>(),
            Substitute.For<IBetterStackHeartbeatService>());

        var result = await sut.GetDoctorWorkloadAsync(
            doctorId,
            WorkloadPeriodType.Week,
            new DateOnly(2026, 4, 20));

        result.Should().NotBeNull();
        result!.ActualHours.Should().Be(4m);
    }

    [Fact]
    public async Task GetDoctorWorkloadAsync_OverlappingSessions_ShouldNotDoubleCount()
    {
        await using var ctx = CreateContext();
        var doctorId = await SeedDoctorAsync(ctx, OphthalmologistEmploymentType.FullTime, "Doctor Overlap");
        ctx.WorkloadRequirements.Add(new WorkloadRequirement(
            OphthalmologistEmploymentType.FullTime,
            WorkloadPeriodType.Week,
            10m));

        AddCompletedSession(ctx, doctorId, Utc(2026, 4, 20, 1, 0), Utc(2026, 4, 20, 3, 0));
        AddCompletedSession(ctx, doctorId, Utc(2026, 4, 20, 2, 0), Utc(2026, 4, 20, 4, 0));
        AddCompletedSession(ctx, doctorId, Utc(2026, 4, 20, 4, 0), Utc(2026, 4, 20, 5, 30));
        await ctx.SaveChangesAsync();

        var sut = new DashboardMetricsService(
            ctx,
            Substitute.For<IAiQuotaService>(),
            Substitute.For<IBetterStackHeartbeatService>());

        var result = await sut.GetDoctorWorkloadAsync(
            doctorId,
            WorkloadPeriodType.Week,
            new DateOnly(2026, 4, 20));

        result.Should().NotBeNull();
        result!.ActualHours.Should().Be(4.5m);
    }

    private static async Task<Guid> SeedDoctorAsync(
        ApplicationDbContext context,
        OphthalmologistEmploymentType employmentType,
        string fullName)
    {
        var userId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = userId,
            FullName = fullName,
            Email = $"{userId:N}@auraeyes.test",
            UserName = $"{userId:N}@auraeyes.test",
            NormalizedEmail = $"{userId:N}@AURAEYES.TEST",
            NormalizedUserName = $"{userId:N}@AURAEYES.TEST",
            SecurityStamp = Guid.NewGuid().ToString("N"),
            IsDeleted = false,
            IsActive = true
        };

        var doctor = new Ophthalmologist(
            userId,
            employmentType: employmentType,
            workingHoursPerWeek: 40,
            yearsOfExperience: 5);

        context.Users.Add(user);
        context.Ophthalmologists.Add(doctor);
        await context.SaveChangesAsync();

        return doctor.Id;
    }

    private static void AddCompletedSession(
        ApplicationDbContext context,
        Guid doctorId,
        DateTime startUtc,
        DateTime endUtc)
    {
        var session = ConsultationSession.CreateVerification(
            Guid.NewGuid(),
            Guid.NewGuid(),
            0m,
            doctorId);

        context.ConsultationSessions.Add(session);

        var entry = context.Entry(session);
        entry.Property(x => x.Status).CurrentValue = SessionStatus.Completed;
        entry.Property(x => x.StartTime).CurrentValue = startUtc;
        entry.Property(x => x.EndTime).CurrentValue = endUtc;
    }

    private static DateTime Utc(int year, int month, int day, int hour, int minute)
        => new(year, month, day, hour, minute, 0, DateTimeKind.Utc);
}
