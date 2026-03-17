using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Dashboard.Queries.GetRecentScreenings;

/// <summary>
/// Handler for GetRecentScreeningsQuery - Returns real data
/// </summary>
public class GetRecentScreeningsQueryHandler : IQueryHandler<GetRecentScreeningsQuery, PagedResult<RecentScreeningDto>>
{
    private readonly IDashboardMetricsService _dashboardMetricsService;

    public GetRecentScreeningsQueryHandler(IDashboardMetricsService dashboardMetricsService)
    {
        _dashboardMetricsService = dashboardMetricsService;
    }

    public async Task<Result<PagedResult<RecentScreeningDto>>> Handle(GetRecentScreeningsQuery request, CancellationToken cancellationToken)
    {
        var pagedResult = await _dashboardMetricsService.GetRecentScreeningsAsync(request.PageNumber, request.PageSize, cancellationToken);
        return Result<PagedResult<RecentScreeningDto>>.Success(pagedResult);
    }
}
