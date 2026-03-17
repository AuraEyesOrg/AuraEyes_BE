using AURA.Tests.Integration.Fixtures;
using AURA.Tests.Integration.Helpers;
using AURA.Tests.Integration.Seed;
using Domain.Enums;
using Infrastructure.Persistence;

namespace AURA.Tests.Integration.Tests.Queries;

[Collection("Integration")]
public sealed class GetScreeningResultTests
{
    private readonly IntegrationTestFixture _fixture;

    public GetScreeningResultTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetRecentScreenings_ShouldReturnStoredAiAnalysis()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        using (var scope = _fixture.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var patient = await TestDataSeeder.GetSeededPatientAsync(dbContext);
            await TestDataSeeder.CreateAiScreeningWithResultAsync(dbContext, patient.Id, RiskLevel.High);
        }

        var adminToken = await AuthHelper.LoginSystemAdminAsync(client);
        AuthHelper.SetBearerToken(client, adminToken);

        var response = await client.GetAsync("/api/system-admin/dashboard/recent-screenings?pageNumber=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var json = await HttpClientHelper.ReadJsonDocumentAsync(response);
        json.RootElement.GetProperty("success").GetBoolean().Should().BeTrue();

        var data = json.RootElement.GetProperty("data");
        data.ValueKind.Should().Be(JsonValueKind.Object);

        var items = data.GetProperty("items");
        items.ValueKind.Should().Be(JsonValueKind.Array);
        items.GetArrayLength().Should().BeGreaterThan(0);
    }
}
