using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Users.Queries.GetUserMetrics;

/// <summary>
/// Handler for GetUserMetricsQuery - Returns mock data
/// </summary>
public class GetUserMetricsQueryHandler : IQueryHandler<GetUserMetricsQuery, UserMetricsDto>
{
    public Task<Result<UserMetricsDto>> Handle(GetUserMetricsQuery request, CancellationToken cancellationToken)
    {
        // Mock user metrics
        var dto = new UserMetricsDto
        {
            TotalUsers = 350,
            TotalUsersMonthlyChange = 8.5m,
            ActiveDoctors = 85,
            ActiveDoctorsChange = 3.2m,
            PatientsScreened = 12500,
            PatientsScreenedChange = 12.1m,
            PendingApprovals = 15
        };

        return Task.FromResult(Result<UserMetricsDto>.Success(dto));
    }
}
