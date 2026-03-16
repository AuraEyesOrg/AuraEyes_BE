using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.Ophthalmologists.Queries.GetDashboardMetrics;

public class GetDashboardMetricsQueryHandler : IQueryHandler<GetDashboardMetricsQuery, OphthalmologistDashboardMetricsDto>
{
    private readonly IDashboardMetricsService _dashboardMetricsService;

    public GetDashboardMetricsQueryHandler(IDashboardMetricsService dashboardMetricsService)
    {
        _dashboardMetricsService = dashboardMetricsService;
    }

    public async Task<Result<OphthalmologistDashboardMetricsDto>> Handle(GetDashboardMetricsQuery request, CancellationToken cancellationToken)
    {
        var metrics = await _dashboardMetricsService.GetOphthalmologistMetricsAsync(request.UserId, cancellationToken);
        return Result<OphthalmologistDashboardMetricsDto>.Success(metrics);
    }
}