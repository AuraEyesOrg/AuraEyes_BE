using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Users.Queries.GetUserMetrics;

/// <summary>
/// Handler for GetUserMetricsQuery - delegates to IIdentityService for user metrics.
/// </summary>
public class GetUserMetricsQueryHandler : IQueryHandler<GetUserMetricsQuery, UserMetricsDto>
{
    private readonly IIdentityService _identityService;

    public GetUserMetricsQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<UserMetricsDto>> Handle(
        GetUserMetricsQuery request,
        CancellationToken cancellationToken)
    {
        var metrics = await _identityService.GetUserMetricsAsync(cancellationToken);

        var dto = new UserMetricsDto
        {
            TotalUsers = metrics.TotalUsers,
            TotalUsersMonthlyChange = metrics.TotalUsersMonthlyChange,
            ActiveDoctors = metrics.ActiveDoctors,
            ActiveDoctorsChange = metrics.ActiveDoctorsChange,
            PatientsScreened = metrics.PatientsScreened,
            PatientsScreenedChange = metrics.PatientsScreenedChange,
            PendingApprovals = metrics.PendingApprovals,
            ClinicStaffCount = metrics.ClinicStaffCount,
            OphthalmologistCount = metrics.OphthalmologistCount
        };

        return Result<UserMetricsDto>.Success(dto);
    }
}
