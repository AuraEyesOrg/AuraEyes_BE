using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Dashboard.Queries.GetDashboardMetrics;

/// <summary>
/// Handler for GetDashboardMetricsQuery - Returns mock data
/// </summary>
public class GetDashboardMetricsQueryHandler : IQueryHandler<GetDashboardMetricsQuery, DashboardMetricsDto>
{
    public Task<Result<DashboardMetricsDto>> Handle(GetDashboardMetricsQuery request, CancellationToken cancellationToken)
    {
        // Mock data for dashboard metrics
        var dto = new DashboardMetricsDto
        {
            TotalScreeningsToday = 156,
            TotalScreeningsYesterday = 142,
            ScreeningsChangePercentage = 9.9m,
            AiAccuracy = 96.5m,
            AiAccuracyChangePercentage = 1.2m,
            PendingReviews = 23,
            CriticalCases = 5,
            ActionRequired = true,
            TotalActiveClinics = 45,
            TotalActiveDevices = 120,
            TotalUsers = 350,
            TotalPatients = 12500
        };

        return Task.FromResult(Result<DashboardMetricsDto>.Success(dto));
    }
}
