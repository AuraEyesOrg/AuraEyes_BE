using AURA.Tests.Integration.Fixtures;
using AURA.Tests.Integration.Helpers;
using AURA.Tests.Integration.Seed;
using Infrastructure.Persistence;

namespace AURA.Tests.Integration.Tests.API;

[Collection("Integration")]
public sealed class ConsultationSessionsControllerTests
{
    private readonly IntegrationTestFixture _fixture;

    public ConsultationSessionsControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task VerificationLifecycle_CreateReportMessageEnd_ShouldSucceed()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        Guid patientId;
        Guid ophthalmologistId;
        Guid aiScreeningId;

        using (var scope = _fixture.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var patient = await TestDataSeeder.GetSeededPatientAsync(dbContext);
            var ophthalmologist = await dbContext.Ophthalmologists.FirstAsync();
            ophthalmologist.Verify();

            var screening = await TestDataSeeder.CreateAiScreeningWithResultAsync(dbContext, patient.Id);
            await dbContext.SaveChangesAsync();

            patientId = patient.Id;
            ophthalmologistId = ophthalmologist.Id;
            aiScreeningId = screening.Screening.Id;
        }

        var patientToken = await AuthHelper.LoginPatientAsync(client);
        AuthHelper.SetBearerToken(client, patientToken);

        var createResponse = await client.PostAsJsonAsync("/api/consultation-sessions/verification", new
        {
            patientId,
            aiScreeningId,
            price = 150000,
            ophthalmologistId
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        using var createBody = await HttpClientHelper.ReadJsonDocumentAsync(createResponse);
        var sessionId = createBody.RootElement.GetProperty("data").GetGuid();

        var getSessionResponse = await client.GetAsync($"/api/consultation-sessions/{sessionId}");
        getSessionResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var listResponse = await client.GetAsync("/api/consultation-sessions?pageNumber=1&pageSize=10");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var doctorToken = await AuthHelper.LoginOphthalmologistAsync(client);
        AuthHelper.SetBearerToken(client, doctorToken);

        var reportResponse = await client.PostAsJsonAsync($"/api/consultation-sessions/{sessionId}/verification-report", new
        {
            doctorId = ophthalmologistId,
            diagnosesCode = "H35.0",
            diagnosesText = "Diabetic retinopathy suspected",
            treatmentPlan = "Recommend follow-up screening in 3 months"
        });
        reportResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        AuthHelper.SetBearerToken(client, patientToken);
        var messageResponse = await client.PostAsJsonAsync($"/api/consultation-sessions/{sessionId}/messages", new
        {
            message = "Cam on bac si da tu van"
        });
        messageResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        AuthHelper.SetBearerToken(client, doctorToken);
        var endResponse = await client.PostAsJsonAsync($"/api/consultation-sessions/{sessionId}/end", new
        {
            doctorId = ophthalmologistId
        });
        endResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CancelSession_WithValidPatientRequest_ShouldSucceed()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        Guid patientId;
        Guid ophthalmologistId;
        Guid aiScreeningId;

        using (var scope = _fixture.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var patient = await TestDataSeeder.GetSeededPatientAsync(dbContext);
            var ophthalmologist = await TestDataSeeder.GetSeededOphthalmologistAsync(dbContext);
            var screening = await TestDataSeeder.CreateAiScreeningWithResultAsync(dbContext, patient.Id);

            patientId = patient.Id;
            ophthalmologistId = ophthalmologist.Id;
            aiScreeningId = screening.Screening.Id;
        }

        var patientToken = await AuthHelper.LoginPatientAsync(client);
        AuthHelper.SetBearerToken(client, patientToken);

        var createResponse = await client.PostAsJsonAsync("/api/consultation-sessions/verification", new
        {
            patientId,
            aiScreeningId,
            price = 120000,
            ophthalmologistId
        });
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        using var createBody = await HttpClientHelper.ReadJsonDocumentAsync(createResponse);
        var sessionId = createBody.RootElement.GetProperty("data").GetGuid();

        var cancelResponse = await client.PostAsJsonAsync($"/api/consultation-sessions/{sessionId}/cancel", new
        {
            cancelledByUserId = patientId,
            reason = "Cannot attend"
        });

        cancelResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
