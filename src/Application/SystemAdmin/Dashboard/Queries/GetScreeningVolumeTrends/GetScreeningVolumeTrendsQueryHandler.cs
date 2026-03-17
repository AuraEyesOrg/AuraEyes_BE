using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Dashboard.Queries.GetScreeningVolumeTrends;

/// <summary>
/// Handler for GetScreeningVolumeTrendsQuery - Returns real data
/// </summary>
public class GetScreeningVolumeTrendsQueryHandler : IQueryHandler<GetScreeningVolumeTrendsQuery, ScreeningVolumeTrendsDto>
{
    private readonly IDashboardMetricsService _dashboardMetricsService;

    public GetScreeningVolumeTrendsQueryHandler(IDashboardMetricsService dashboardMetricsService)
    {
        _dashboardMetricsService = dashboardMetricsService;
    }

    public async Task<Result<ScreeningVolumeTrendsDto>> Handle(GetScreeningVolumeTrendsQuery request, CancellationToken cancellationToken)
    {
        var dto = await _dashboardMetricsService.GetScreeningVolumeTrendsAsync(request.TimeRange, request.Periods, cancellationToken);
        return Result<ScreeningVolumeTrendsDto>.Success(dto);
    }
}
