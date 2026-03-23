using AURA.Tests.Integration.Fixtures;
using AURA.Tests.Integration.Helpers;
using AURA.Tests.Integration.Seed;
using Domain.Entities.Scheduling;
using Infrastructure.Persistence;

namespace AURA.Tests.Integration.Tests.API;

[Collection("Integration")]
public sealed class AppointmentSlotsControllerTests
{
    private readonly IntegrationTestFixture _fixture;

    public AppointmentSlotsControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AppointmentSlot_CreateManage_Delete_ShouldSucceedForOphthalmologist()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        Guid templateId;
        Guid ophthalmologistId;

        using (var scope = _fixture.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var ophthalmologist = await TestDataSeeder.GetSeededOphthalmologistAsync(dbContext);
            ophthalmologistId = ophthalmologist.Id;

            var template = new ScheduleTemplate(
                dayOfWeek: DateTime.UtcNow.DayOfWeek,
                startTime: new TimeOnly(9, 0),
                endTime: new TimeOnly(12, 0),
                slotDuration: 30,
                maxCapacity: 1,
                ophthalId: ophthalmologistId,
                cost: 200000m);

            await dbContext.ScheduleTemplates.AddAsync(template);
            await dbContext.SaveChangesAsync();
            templateId = template.Id;
        }

        var doctorToken = await AuthHelper.LoginOphthalmologistAsync(client);
        AuthHelper.SetBearerToken(client, doctorToken);

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        var createResponse = await client.PostAsJsonAsync("/api/appointment-slots", new
        {
            scheduleTemplateId = templateId,
            date,
            startTime = "09:00:00",
            endTime = "09:30:00",
            cost = 220000
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        using var createBody = await HttpClientHelper.ReadJsonDocumentAsync(createResponse);
        var slotId = createBody.RootElement.GetProperty("data").GetGuid();

        var getResponse = await client.GetAsync($"/api/appointment-slots/{slotId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updateResponse = await client.PutAsJsonAsync($"/api/appointment-slots/{slotId}", new
        {
            date,
            startTime = "09:30:00",
            endTime = "10:00:00",
            cost = 230000
        });
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var costResponse = await client.PatchAsJsonAsync($"/api/appointment-slots/{slotId}/cost", new
        {
            cost = 240000
        });
        costResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var blockResponse = await client.PostAsJsonAsync($"/api/appointment-slots/{slotId}/block", new
        {
            ophthalmologistId,
            reason = "In surgery"
        });
        blockResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var unblockResponse = await client.PostAsJsonAsync($"/api/appointment-slots/{slotId}/unblock", new
        {
            ophthalmologistId
        });
        unblockResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var deleteResponse = await client.DeleteAsync($"/api/appointment-slots/{slotId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AppointmentSlot_ReserveAndConfirm_ShouldCreateConsultationSession()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        Guid templateId;
        Guid patientId;

        using (var scope = _fixture.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var ophthalmologist = await TestDataSeeder.GetSeededOphthalmologistAsync(dbContext);
            var patient = await TestDataSeeder.GetSeededPatientAsync(dbContext);
            patientId = patient.Id;

            var template = new ScheduleTemplate(
                dayOfWeek: DateTime.UtcNow.DayOfWeek,
                startTime: new TimeOnly(10, 0),
                endTime: new TimeOnly(12, 0),
                slotDuration: 30,
                maxCapacity: 1,
                ophthalId: ophthalmologist.Id,
                cost: 300000m);

            await dbContext.ScheduleTemplates.AddAsync(template);
            await dbContext.SaveChangesAsync();
            templateId = template.Id;
        }

        var doctorToken = await AuthHelper.LoginOphthalmologistAsync(client);
        AuthHelper.SetBearerToken(client, doctorToken);

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2));
        var createSlotResponse = await client.PostAsJsonAsync("/api/appointment-slots", new
        {
            scheduleTemplateId = templateId,
            date,
            startTime = "10:00:00",
            endTime = "10:30:00",
            cost = 300000
        });
        createSlotResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        using var createSlotBody = await HttpClientHelper.ReadJsonDocumentAsync(createSlotResponse);
        var slotId = createSlotBody.RootElement.GetProperty("data").GetGuid();

        var patientToken = await AuthHelper.LoginPatientAsync(client);
        AuthHelper.SetBearerToken(client, patientToken);

        var reserveResponse = await client.PostAsJsonAsync($"/api/appointment-slots/{slotId}/reserve", new
        {
            patientId,
            reservationMinutes = 5
        });
        reserveResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var confirmResponse = await client.PostAsJsonAsync($"/api/appointment-slots/{slotId}/confirm", new
        {
            patientId,
            aiScreeningId = (Guid?)null,
            shareRetinalImages = true,
            shareAiResults = true
        });

        confirmResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var confirmBody = await HttpClientHelper.ReadJsonDocumentAsync(confirmResponse);
        var consultationSessionId = confirmBody.RootElement.GetProperty("data").GetProperty("consultationSessionId").GetGuid();
        consultationSessionId.Should().NotBe(Guid.Empty);
    }
}
