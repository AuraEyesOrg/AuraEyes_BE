using Application.AiQuota.Interfaces;
using FluentAssertions;
using Infrastructure.Persistence;
using Infrastructure.Services;
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
        var sut = new DashboardMetricsService(ctx, aiQuota);

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
        var sut = new DashboardMetricsService(ctx, aiQuota);

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
        var sut = new DashboardMetricsService(ctx, aiQuota);

        var trends = await sut.GetScreeningVolumeTrendsAsync("monthly", 6);

        trends.Should().NotBeNull();
    }

    [Fact]
    public async Task GetScreeningVolumeTrendsAsync_InvalidRange_ShouldDefaultToMonthly()
    {
        await using var ctx = CreateContext();
        var aiQuota = Substitute.For<IAiQuotaService>();
        var sut = new DashboardMetricsService(ctx, aiQuota);

        var trends = await sut.GetScreeningVolumeTrendsAsync("not-a-valid-range", 3);

        trends.Should().NotBeNull();
        trends!.TimeRange.Should().Be("monthly");
    }

    [Fact]
    public async Task GetPopulationRiskAnalysisAsync_EmptyDb_ShouldReturnDto()
    {
        await using var ctx = CreateContext();
        var aiQuota = Substitute.For<IAiQuotaService>();
        var sut = new DashboardMetricsService(ctx, aiQuota);

        var analysis = await sut.GetPopulationRiskAnalysisAsync();

        analysis.Should().NotBeNull();
    }

    [Fact]
    public async Task GetSystemHealthAsync_ShouldReturnStatus()
    {
        await using var ctx = CreateContext();
        var aiQuota = Substitute.For<IAiQuotaService>();
        var sut = new DashboardMetricsService(ctx, aiQuota);

        var health = await sut.GetSystemHealthAsync();

        health.Should().NotBeNull();
    }

    [Fact]
    public async Task GetOphthalmologistMetricsAsync_WithOphthalId_ShouldReturnDto()
    {
        await using var ctx = CreateContext();
        var aiQuota = Substitute.For<IAiQuotaService>();
        var sut = new DashboardMetricsService(ctx, aiQuota);

        var metrics = await sut.GetOphthalmologistMetricsAsync(Guid.NewGuid());

        metrics.Should().NotBeNull();
    }
}
