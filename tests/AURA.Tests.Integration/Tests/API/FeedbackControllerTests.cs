using AURA.Tests.Integration.Fixtures;
using AURA.Tests.Integration.Helpers;
using AURA.Tests.Integration.Seed;
using Domain.Entities.Scheduling;
using Infrastructure.Persistence;

namespace AURA.Tests.Integration.Tests.API;

[Collection("Integration")]
public sealed class FeedbackControllerTests
{
    private readonly IntegrationTestFixture _fixture;

    public FeedbackControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task WebsiteFeedback_CreateAndGet_ShouldSucceedAfterAiUsage()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var token = await AuthHelper.LoginPatientAsync(client);
        AuthHelper.SetBearerToken(client, token);

        using (var scope = _fixture.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var patient = await TestDataSeeder.GetSeededPatientAsync(dbContext);
            await TestDataSeeder.CreateAiScreeningWithResultAsync(dbContext, patient.Id);
        }

        var createResponse = await client.PostAsJsonAsync("/api/feedback/website", new
        {
            rating = 5,
            category = 2,
            comment = "Great UX on screening flow"
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        using var createBody = await HttpClientHelper.ReadJsonDocumentAsync(createResponse);
        var feedbackId = createBody.RootElement.GetProperty("data").GetGuid();

        var getResponse = await client.GetAsync($"/api/feedback/website/{feedbackId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var getBody = await HttpClientHelper.ReadJsonDocumentAsync(getResponse);
        getBody.RootElement.GetProperty("data").GetProperty("rating").GetInt32().Should().Be(5);
    }

    [Fact]
    public async Task OrganisationFeedback_CreateListGetRating_ShouldSucceed()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var token = await AuthHelper.LoginPatientAsync(client);
        AuthHelper.SetBearerToken(client, token);

        Guid organisationId;
        Guid appointmentId;

        using (var scope = _fixture.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var patient = await TestDataSeeder.GetSeededPatientAsync(dbContext);
            var org = await dbContext.Organisations.AsNoTracking().FirstAsync();

            var template = new ScheduleTemplate(
                dayOfWeek: DateTime.UtcNow.DayOfWeek,
                startTime: new TimeOnly(9, 0),
                endTime: new TimeOnly(10, 0),
                slotDuration: 30,
                maxCapacity: 1,
                orgId: org.Id,
                cost: 200000m);

            await dbContext.ScheduleTemplates.AddAsync(template);
            await dbContext.SaveChangesAsync();

            var slot = new AppointmentSlot(
                template.Id,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                new TimeOnly(9, 0),
                new TimeOnly(9, 30),
                maxCapacity: 1,
                cost: 200000m);

            slot.BookWithCapacity();
            slot.UpdateStatus(Domain.Enums.ScheduleStatus.Booked);

            await dbContext.AppointmentSlots.AddAsync(slot);
            await dbContext.SaveChangesAsync();

            var appointment = Appointment.CreateClinicVisit(patient.Id, slot.Id, org.Id, "Eye discomfort");
            appointment.CheckIn();
            appointment.Start();
            appointment.Complete("Completed for feedback test");

            await dbContext.Appointments.AddAsync(appointment);
            await dbContext.SaveChangesAsync();

            organisationId = org.Id;
            appointmentId = appointment.Id;
        }

        var createResponse = await client.PostAsJsonAsync($"/api/feedback/organisations/{organisationId}", new
        {
            appointmentId,
            rating = 4,
            comment = "Professional clinic staff"
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        using var createBody = await HttpClientHelper.ReadJsonDocumentAsync(createResponse);
        var feedbackId = createBody.RootElement.GetProperty("data").GetGuid();

        var listResponse = await client.GetAsync($"/api/feedback/organisations/{organisationId}/items?pageNumber=1&pageSize=10");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        using var listBody = await HttpClientHelper.ReadJsonDocumentAsync(listResponse);
        listBody.RootElement.GetProperty("data").GetProperty("items").GetArrayLength().Should().BeGreaterThan(0);

        var getResponse = await client.GetAsync($"/api/feedback/organisations/{organisationId}/items/{feedbackId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var ratingResponse = await client.GetAsync($"/api/feedback/organisations/{organisationId}/rating");
        ratingResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        using var ratingBody = await HttpClientHelper.ReadJsonDocumentAsync(ratingResponse);
        ratingBody.RootElement.GetProperty("data").GetProperty("ratingCount").GetInt32().Should().BeGreaterThan(0);
    }
}
