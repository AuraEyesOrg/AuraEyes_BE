using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.Ophthalmologists.Queries.GetReviewQueue;

public class GetReviewQueueQueryHandler : IQueryHandler<GetReviewQueueQuery, List<ReviewQueueItemDto>>
{
    private readonly IDashboardMetricsService _dashboardMetricsService;

    public GetReviewQueueQueryHandler(IDashboardMetricsService dashboardMetricsService)
    {
        _dashboardMetricsService = dashboardMetricsService;
    }

    public async Task<Result<List<ReviewQueueItemDto>>> Handle(GetReviewQueueQuery request, CancellationToken cancellationToken)
    {
        var items = await _dashboardMetricsService.GetReviewQueueAsync(request.UserId, cancellationToken);
        return Result<List<ReviewQueueItemDto>>.Success(items);
    }
}
