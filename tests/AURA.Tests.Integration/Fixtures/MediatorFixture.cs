namespace AURA.Tests.Integration.Fixtures;

public sealed class MediatorFixture
{
    private readonly ApiFactory _apiFactory;

    public MediatorFixture(ApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
    }

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        using var scope = _apiFactory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        return await mediator.Send(request, cancellationToken);
    }
}
