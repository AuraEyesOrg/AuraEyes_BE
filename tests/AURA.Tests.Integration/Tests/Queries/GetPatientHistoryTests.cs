using AURA.Tests.Integration.Fixtures;
using AURA.Tests.Integration.Helpers;
using AURA.Tests.Integration.Seed;
using Infrastructure.Persistence;

namespace AURA.Tests.Integration.Tests.Queries;

[Collection("Integration")]
public sealed class GetPatientHistoryTests
{
    private readonly IntegrationTestFixture _fixture;

    public GetPatientHistoryTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetPatientHistory_ShouldReturnClinicAppointmentHistory()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        Guid patientId;

        using (var scope = _fixture.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var patient = await TestDataSeeder.GetSeededPatientAsync(dbContext);
            patientId = patient.Id;
        }

        var token = await AuthHelper.LoginPatientAsync(client);
        AuthHelper.SetBearerToken(client, token);

        var response = await client.GetAsync($"/api/patients/{patientId}/clinic-appointments");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var json = await HttpClientHelper.ReadJsonDocumentAsync(response);
        json.RootElement.GetProperty("success").GetBoolean().Should().BeTrue();
        json.RootElement.GetProperty("data").ValueKind.Should().Be(JsonValueKind.Array);
    }
}
