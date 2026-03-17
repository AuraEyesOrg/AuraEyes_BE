using AURA.Tests.Integration.Builders;
using AURA.Tests.Integration.Fixtures;
using AURA.Tests.Integration.Seed;
using Application.ConsultationSessions.Commands.EndSession;
using Domain.Enums;
using Infrastructure.Persistence;

namespace AURA.Tests.Integration.Tests.Commands;

[Collection("Integration")]
public sealed class CompleteConsultationTests
{
    private readonly IntegrationTestFixture _fixture;

    public CompleteConsultationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CompleteConsultation_ShouldUpdateStatusCorrectly()
    {
        await _fixture.ResetDatabaseAsync();

        Guid sessionId;
        Guid ophthalmologistId;

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

            sessionId = session.Id;
            ophthalmologistId = ophthalmologist.Id;

            await dbContext.ConsultationSessions.AddAsync(session);
            await dbContext.SaveChangesAsync();
        }

        var result = await _fixture.MediatorFixture.SendAsync(new EndSessionCommand
        {
            SessionId = sessionId,
            DoctorId = ophthalmologistId
        });

        result.IsSuccess.Should().BeTrue();

        using var assertScope = _fixture.CreateScope();
        var dbAssert = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var updated = await dbAssert.ConsultationSessions.AsNoTracking().FirstAsync(x => x.Id == sessionId);

        updated.Status.Should().Be(SessionStatus.Completed);
        updated.ChatStatus.Should().Be(ChatStatus.Archived);
    }
}
