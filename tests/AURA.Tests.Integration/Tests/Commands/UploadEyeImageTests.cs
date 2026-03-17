using AURA.Tests.Integration.Builders;
using AURA.Tests.Integration.Fixtures;
using AURA.Tests.Integration.Seed;

namespace AURA.Tests.Integration.Tests.Commands;

[Collection("Integration")]
public sealed class UploadEyeImageTests
{
    private readonly IntegrationTestFixture _fixture;

    public UploadEyeImageTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task UploadEyeImage_ShouldPersistMetadata_AndTriggerAiRequest()
    {
        await _fixture.ResetDatabaseAsync();

        using var scope = _fixture.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Infrastructure.Persistence.ApplicationDbContext>();
        var patient = await TestDataSeeder.GetSeededPatientAsync(dbContext);

        var builder = new ScreeningSessionBuilder().WithPatientId(patient.Id);
        var (screening, image, result) = builder.Build();

        await dbContext.AiScreenings.AddAsync(screening);
        await dbContext.RetinalImages.AddAsync(image);
        await dbContext.ScreeningResults.AddAsync(result);
        await dbContext.SaveChangesAsync();

        using var externalClient = new HttpClient { BaseAddress = new Uri(_fixture.ApiFactory.AiMockServer.Urls[0]) };
        var aiResponse = await externalClient.PostAsJsonAsync("/mock/ai/screenings", new
        {
            screeningId = screening.Id,
            imageId = image.Id,
            imageUrl = image.ImageUrl
        });

        aiResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);

        var persistedImage = await dbContext.RetinalImages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == image.Id);
        persistedImage.Should().NotBeNull();
        persistedImage!.AiScreeningId.Should().Be(screening.Id);

        _fixture.ApiFactory.AiMockServer.LogEntries.Should().NotBeEmpty();
    }
}
