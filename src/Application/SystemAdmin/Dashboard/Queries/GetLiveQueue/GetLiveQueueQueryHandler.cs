using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Dashboard.Queries.GetLiveQueue;

public class GetLiveQueueQueryHandler
    : IQueryHandler<GetLiveQueueQuery, IReadOnlyList<LiveQueueItemDto>>
{
    private readonly IDashboardMetricsService _dashboardMetricsService;

    public GetLiveQueueQueryHandler(IDashboardMetricsService dashboardMetricsService)
    {
        _dashboardMetricsService = dashboardMetricsService;
    }

    public async Task<Result<IReadOnlyList<LiveQueueItemDto>>> Handle(
        GetLiveQueueQuery request,
        CancellationToken cancellationToken)
    {
        var dto = await _dashboardMetricsService.GetLiveQueueAsync(cancellationToken);
        return Result<IReadOnlyList<LiveQueueItemDto>>.Success(dto);
    }
}
