using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.Patients.Queries.GetDashboardMetrics;

public class GetDashboardMetricsQueryHandler : IQueryHandler<GetDashboardMetricsQuery, PatientDashboardMetricsDto>
{
    private readonly IDashboardMetricsService _dashboardMetricsService;

    public GetDashboardMetricsQueryHandler(IDashboardMetricsService dashboardMetricsService)
    {
        _dashboardMetricsService = dashboardMetricsService;
    }

    public async Task<Result<PatientDashboardMetricsDto>> Handle(GetDashboardMetricsQuery request, CancellationToken cancellationToken)
    {
        var metrics = await _dashboardMetricsService.GetPatientMetricsAsync(request.UserId, cancellationToken);
        return Result<PatientDashboardMetricsDto>.Success(metrics);
    }
}