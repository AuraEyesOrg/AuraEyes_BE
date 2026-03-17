using Infrastructure.Persistence;

namespace AURA.Tests.Integration.Fixtures;

public sealed class IntegrationTestFixture : IAsyncLifetime
{
    private readonly DatabaseFixture _databaseFixture;

    public IntegrationTestFixture()
    {
        _databaseFixture = new DatabaseFixture();
        ApiFactory = new ApiFactory(_databaseFixture);
        MediatorFixture = new MediatorFixture(ApiFactory);
    }

    public ApiFactory ApiFactory { get; }

    public MediatorFixture MediatorFixture { get; }

    public async Task InitializeAsync()
    {
        await _databaseFixture.InitializeAsync();
        await ApiFactory.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        await ApiFactory.DisposeAsync();
        await _databaseFixture.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await ApiFactory.ResetDatabaseAsync();
    }

    public HttpClient CreateClient()
    {
        return ApiFactory.CreateClientWithHttps();
    }

    public IServiceScope CreateScope()
    {
        return ApiFactory.Services.CreateScope();
    }

    public async Task<ApplicationDbContext> CreateDbContextAsync()
    {
        var scope = CreateScope();
        return await Task.FromResult(scope.ServiceProvider.GetRequiredService<ApplicationDbContext>());
    }
}
