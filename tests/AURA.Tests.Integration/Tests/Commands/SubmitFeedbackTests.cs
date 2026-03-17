using AURA.Tests.Integration.Builders;
using AURA.Tests.Integration.Fixtures;
using AURA.Tests.Integration.Helpers;
using AURA.Tests.Integration.Seed;
using Infrastructure.Persistence;

namespace AURA.Tests.Integration.Tests.Commands;

[Collection("Integration")]
public sealed class SubmitFeedbackTests
{
    private readonly IntegrationTestFixture _fixture;

    public SubmitFeedbackTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task SubmitFeedback_ShouldLinkToConsultation()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var token = await AuthHelper.LoginPatientAsync(client);
        AuthHelper.SetBearerToken(client, token);

        Guid consultationId;
        Guid ophthalmologistId;

        using (var scope = _fixture.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var patient = await TestDataSeeder.GetSeededPatientAsync(dbContext);
            var ophthalmologist = await TestDataSeeder.GetSeededOphthalmologistAsync(dbContext);
            var screening = await TestDataSeeder.CreateAiScreeningWithResultAsync(dbContext, patient.Id);
            var consultation = await TestDataSeeder.CreateCompletedConsultationAsync(
                dbContext,
                patient.Id,
                ophthalmologist.Id,
                screening.Screening.Id);

            consultationId = consultation.Id;
            ophthalmologistId = ophthalmologist.Id;
        }

        var response = await client.PostAsJsonAsync($"/api/feedback/ophthalmologists/{ophthalmologistId}", new
        {
            consultationSessionId = consultationId,
            rating = 5,
            comment = "Great consultation quality"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        using var assertScope = _fixture.CreateScope();
        var dbAssert = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var feedback = await dbAssert.OphthalmologistFeedbacks.AsNoTracking().FirstOrDefaultAsync(x => x.ConsultationSessionId == consultationId);

        feedback.Should().NotBeNull();
        feedback!.OphthalmologistId.Should().Be(ophthalmologistId);
    }

    [Fact]
    public async Task SubmitFeedback_WithInvalidRating_ShouldBeRejected()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var token = await AuthHelper.LoginPatientAsync(client);
        AuthHelper.SetBearerToken(client, token);

        var randomDoctorId = Guid.NewGuid();
        var randomSessionId = Guid.NewGuid();

        var response = await client.PostAsJsonAsync($"/api/feedback/ophthalmologists/{randomDoctorId}", new
        {
            consultationSessionId = randomSessionId,
            rating = 0,
            comment = "Invalid rating"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SubmitFeedback_BeforeConsultationCompletion_ShouldFail()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var token = await AuthHelper.LoginPatientAsync(client);
        AuthHelper.SetBearerToken(client, token);

        Guid consultationId;
        Guid ophthalmologistId;

        using (var scope = _fixture.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var patient = await TestDataSeeder.GetSeededPatientAsync(dbContext);
            var ophthalmologist = await TestDataSeeder.GetSeededOphthalmologistAsync(dbContext);
            var screening = await TestDataSeeder.CreateAiScreeningWithResultAsync(dbContext, patient.Id);

            var consultation = new ConsultationBuilder()
                .WithPatientId(patient.Id)
                .WithOphthalmologistId(ophthalmologist.Id)
                .WithAiScreeningId(screening.Screening.Id)
                .BuildVerification();

            await dbContext.ConsultationSessions.AddAsync(consultation);
            await dbContext.SaveChangesAsync();

            consultationId = consultation.Id;
            ophthalmologistId = ophthalmologist.Id;
        }

        var response = await client.PostAsJsonAsync($"/api/feedback/ophthalmologists/{ophthalmologistId}", new
        {
            consultationSessionId = consultationId,
            rating = 4,
            comment = "Should fail before completion"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        using var doc = await HttpClientHelper.ReadJsonDocumentAsync(response);
        var message = doc.RootElement.GetProperty("message").GetString();
        message.Should().NotBeNullOrWhiteSpace();
        message!.ToLowerInvariant().Should().Contain("completed");
    }
}
