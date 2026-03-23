using AURA.Tests.Integration.Fixtures;
using AURA.Tests.Integration.Helpers;

namespace AURA.Tests.Integration.Tests.API;

[Collection("Integration")]
public sealed class SystemAdminDashboardControllerTests
{
    private readonly IntegrationTestFixture _fixture;

    public SystemAdminDashboardControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetMetrics_ShouldReturnDashboardMetrics()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var token = await AuthHelper.LoginSystemAdminAsync(client);
        AuthHelper.SetBearerToken(client, token);

        var response = await client.GetAsync("/api/system-admin/dashboard/metrics");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var body = await HttpClientHelper.ReadJsonDocumentAsync(response);
        body.RootElement.GetProperty("success").GetBoolean().Should().BeTrue();

        var data = body.RootElement.GetProperty("data");
        data.ValueKind.Should().Be(JsonValueKind.Object);
        data.GetProperty("totalScreeningsToday").GetInt32().Should().BeGreaterThanOrEqualTo(0);
        data.GetProperty("aiAccuracy").GetDecimal().Should().BeGreaterThanOrEqualTo(0m);
    }

    [Fact]
    public async Task GetRiskAnalysis_ShouldReturnPopulationRiskBreakdown()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var token = await AuthHelper.LoginSystemAdminAsync(client);
        AuthHelper.SetBearerToken(client, token);

        var response = await client.GetAsync("/api/system-admin/dashboard/risk-analysis");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var body = await HttpClientHelper.ReadJsonDocumentAsync(response);
        body.RootElement.GetProperty("success").GetBoolean().Should().BeTrue();

        var data = body.RootElement.GetProperty("data");
        data.ValueKind.Should().Be(JsonValueKind.Object);

        var categories = data.GetProperty("riskCategories");
        categories.ValueKind.Should().Be(JsonValueKind.Array);
        categories.GetArrayLength().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetSystemHealth_ShouldReturnComponentHealthStatus()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var token = await AuthHelper.LoginSystemAdminAsync(client);
        AuthHelper.SetBearerToken(client, token);

        var response = await client.GetAsync("/api/system-admin/dashboard/system-health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var body = await HttpClientHelper.ReadJsonDocumentAsync(response);
        body.RootElement.GetProperty("success").GetBoolean().Should().BeTrue();

        var data = body.RootElement.GetProperty("data");
        data.ValueKind.Should().Be(JsonValueKind.Object);

        var components = data.GetProperty("components");
        components.ValueKind.Should().Be(JsonValueKind.Array);
        components.GetArrayLength().Should().BeGreaterThan(0);
    }
}
