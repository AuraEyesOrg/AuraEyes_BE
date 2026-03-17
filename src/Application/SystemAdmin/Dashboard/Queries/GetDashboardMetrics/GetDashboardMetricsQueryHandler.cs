using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Dashboard.Queries.GetDashboardMetrics;

/// <summary>
/// Handler for GetDashboardMetricsQuery - Returns real dashboard metrics
/// </summary>
public class GetDashboardMetricsQueryHandler : IQueryHandler<GetDashboardMetricsQuery, DashboardMetricsDto>
{
    private readonly IDashboardMetricsService _dashboardMetricsService;

    public GetDashboardMetricsQueryHandler(IDashboardMetricsService dashboardMetricsService)
    {
        _dashboardMetricsService = dashboardMetricsService;
    }

    public async Task<Result<DashboardMetricsDto>> Handle(GetDashboardMetricsQuery request, CancellationToken cancellationToken)
    {
        var dto = await _dashboardMetricsService.GetSystemAdminMetricsAsync(cancellationToken);
        return Result<DashboardMetricsDto>.Success(dto);
    }
}
