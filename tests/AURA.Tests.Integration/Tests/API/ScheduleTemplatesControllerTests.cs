using AURA.Tests.Integration.Fixtures;
using AURA.Tests.Integration.Helpers;
using AURA.Tests.Integration.Seed;
using Infrastructure.Persistence;

namespace AURA.Tests.Integration.Tests.API;

[Collection("Integration")]
public sealed class ScheduleTemplatesControllerTests
{
    private readonly IntegrationTestFixture _fixture;

    public ScheduleTemplatesControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ScheduleTemplate_CreateGetUpdateListDelete_ShouldSucceed()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        Guid ophthalmologistId;
        using (var scope = _fixture.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var ophthalmologist = await TestDataSeeder.GetSeededOphthalmologistAsync(dbContext);
            ophthalmologistId = ophthalmologist.Id;
        }

        var doctorToken = await AuthHelper.LoginOphthalmologistAsync(client);
        AuthHelper.SetBearerToken(client, doctorToken);

        var createResponse = await client.PostAsJsonAsync("/api/schedule-templates", new
        {
            ophthalId = ophthalmologistId,
            dayOfWeek = 1,
            startTime = "09:00:00",
            endTime = "12:00:00",
            slotDuration = 30,
            maxCapacity = 1,
            cost = 250000
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        using var createBody = await HttpClientHelper.ReadJsonDocumentAsync(createResponse);
        var templateId = createBody.RootElement.GetProperty("data").GetGuid();

        var getResponse = await client.GetAsync($"/api/schedule-templates/{templateId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updateResponse = await client.PutAsJsonAsync($"/api/schedule-templates/{templateId}", new
        {
            dayOfWeek = 1,
            startTime = "09:30:00",
            endTime = "12:30:00",
            slotDuration = 30,
            maxCapacity = 1,
            cost = 300000
        });
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var listResponse = await client.GetAsync($"/api/schedule-templates?ophthalId={ophthalmologistId}&pageNumber=1&pageSize=10");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var deleteResponse = await client.DeleteAsync($"/api/schedule-templates/{templateId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
