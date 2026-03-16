using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.Organisations.Queries.GetDashboardMetrics;

public class GetDashboardMetricsQueryHandler : IQueryHandler<GetDashboardMetricsQuery, OrganisationDashboardMetricsDto>
{
    private readonly IDashboardMetricsService _dashboardMetricsService;

    public GetDashboardMetricsQueryHandler(IDashboardMetricsService dashboardMetricsService)
    {
        _dashboardMetricsService = dashboardMetricsService;
    }

    public async Task<Result<OrganisationDashboardMetricsDto>> Handle(GetDashboardMetricsQuery request, CancellationToken cancellationToken)
    {
        var metrics = await _dashboardMetricsService.GetOrganisationMetricsAsync(request.UserId, cancellationToken);
        return Result<OrganisationDashboardMetricsDto>.Success(metrics);
    }
}