using AURA.Tests.Integration.Builders;
using AURA.Tests.Integration.Fixtures;
using AURA.Tests.Integration.Helpers;
using AURA.Tests.Integration.Seed;
using Infrastructure.Persistence;

namespace AURA.Tests.Integration.Tests.Queries;

[Collection("Integration")]
public sealed class GetConsultationTests
{
    private readonly IntegrationTestFixture _fixture;

    public GetConsultationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetConsultation_ShouldReturnSessionById()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        Guid sessionId;

        using (var scope = _fixture.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var patient = await TestDataSeeder.GetSeededPatientAsync(dbContext);
            var ophthalmologist = await TestDataSeeder.GetSeededOphthalmologistAsync(dbContext);
            var screening = await TestDataSeeder.CreateAiScreeningWithResultAsync(dbContext, patient.Id);

            var session = new ConsultationBuilder()
                .WithPatientId(patient.Id)
                .WithOphthalmologistId(ophthalmologist.Id)
                .WithAiScreeningId(screening.Screening.Id)
                .BuildVerification();

            await dbContext.ConsultationSessions.AddAsync(session);
            await dbContext.SaveChangesAsync();
            sessionId = session.Id;
        }

        var token = await AuthHelper.LoginPatientAsync(client);
        AuthHelper.SetBearerToken(client, token);

        var response = await client.GetAsync($"/api/consultation-sessions/{sessionId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var json = await HttpClientHelper.ReadJsonDocumentAsync(response);
        json.RootElement.GetProperty("success").GetBoolean().Should().BeTrue();

        var id = json.RootElement.GetProperty("data").GetProperty("id").GetGuid();
        id.Should().Be(sessionId);
    }
}
