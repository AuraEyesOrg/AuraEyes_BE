using AURA.Tests.Integration.Fixtures;
using AURA.Tests.Integration.Helpers;
using AURA.Tests.Integration.Seed;
using Domain.Entities.Scheduling;
using Infrastructure.Persistence;

namespace AURA.Tests.Integration.Tests.API;

[Collection("Integration")]
public sealed class ClinicAppointmentsControllerTests
{
    private readonly IntegrationTestFixture _fixture;

    public ClinicAppointmentsControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ClinicAppointment_CreateCheckInStartComplete_ShouldSucceed()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var (organisationId, slotId) = await SeedOrganisationSlotAsync();

        var patientToken = await AuthHelper.LoginPatientAsync(client);
        AuthHelper.SetBearerToken(client, patientToken);

        var createResponse = await client.PostAsJsonAsync("/api/clinic-appointments", new
        {
            organisationId,
            slotId,
            visitReason = "Routine eye check"
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        using var createBody = await HttpClientHelper.ReadJsonDocumentAsync(createResponse);
        var appointmentId = createBody.RootElement.GetProperty("data").GetProperty("appointmentId").GetGuid();

        var orgAdminToken = await AuthHelper.LoginOrgAdminAsync(client);
        AuthHelper.SetBearerToken(client, orgAdminToken);

        var checkInResponse = await client.PutAsync($"/api/clinic-appointments/{appointmentId}/check-in", null);
        checkInResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var startResponse = await client.PutAsync($"/api/clinic-appointments/{appointmentId}/start", null);
        startResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var completeResponse = await client.PutAsJsonAsync($"/api/clinic-appointments/{appointmentId}/complete", new
        {
            notes = "Completed normally"
        });
        completeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ClinicAppointment_CancelAndNoShow_ShouldSucceed()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var patientToken = await AuthHelper.LoginPatientAsync(client);
        AuthHelper.SetBearerToken(client, patientToken);

        var (organisationIdForCancel, slotIdForCancel) = await SeedOrganisationSlotAsync();
        var createForCancel = await client.PostAsJsonAsync("/api/clinic-appointments", new
        {
            organisationId = organisationIdForCancel,
            slotId = slotIdForCancel,
            visitReason = "Cancel case"
        });
        createForCancel.StatusCode.Should().Be(HttpStatusCode.OK);
        using var createForCancelBody = await HttpClientHelper.ReadJsonDocumentAsync(createForCancel);
        var cancelAppointmentId = createForCancelBody.RootElement.GetProperty("data").GetProperty("appointmentId").GetGuid();

        using var cancelRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/clinic-appointments/{cancelAppointmentId}")
        {
            Content = JsonContent.Create(new
            {
                reason = "Personal reason"
            })
        };
        var cancelResponse = await client.SendAsync(cancelRequest);
        cancelResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var (organisationIdForNoShow, slotIdForNoShow) = await SeedOrganisationSlotAsync();
        var createForNoShow = await client.PostAsJsonAsync("/api/clinic-appointments", new
        {
            organisationId = organisationIdForNoShow,
            slotId = slotIdForNoShow,
            visitReason = "No show case"
        });
        createForNoShow.StatusCode.Should().Be(HttpStatusCode.OK);
        using var createForNoShowBody = await HttpClientHelper.ReadJsonDocumentAsync(createForNoShow);
        var noShowAppointmentId = createForNoShowBody.RootElement.GetProperty("data").GetProperty("appointmentId").GetGuid();

        var orgAdminToken = await AuthHelper.LoginOrgAdminAsync(client);
        AuthHelper.SetBearerToken(client, orgAdminToken);

        var noShowResponse = await client.PutAsync($"/api/clinic-appointments/{noShowAppointmentId}/no-show", null);
        noShowResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<(Guid OrganisationId, Guid SlotId)> SeedOrganisationSlotAsync()
    {
        using var scope = _fixture.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var organisation = await dbContext.Organisations.AsNoTracking().FirstAsync();
        var template = new ScheduleTemplate(
            dayOfWeek: DateTime.UtcNow.DayOfWeek,
            startTime: new TimeOnly(13, 0),
            endTime: new TimeOnly(15, 0),
            slotDuration: 30,
            maxCapacity: 2,
            orgId: organisation.Id,
            cost: 180000m);

        await dbContext.ScheduleTemplates.AddAsync(template);
        await dbContext.SaveChangesAsync();

        var slot = new AppointmentSlot(
            template.Id,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            new TimeOnly(13, 0),
            new TimeOnly(13, 30),
            maxCapacity: 2,
            cost: 180000m);

        await dbContext.AppointmentSlots.AddAsync(slot);
        await dbContext.SaveChangesAsync();

        return (organisation.Id, slot.Id);
    }
}
