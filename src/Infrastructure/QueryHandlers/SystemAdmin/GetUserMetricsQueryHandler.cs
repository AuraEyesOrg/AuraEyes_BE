using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Users.Queries.GetUserMetrics;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using UserMetrics = Application.SystemAdmin.Users.Queries.GetUserMetrics.UserMetricsDto;

namespace Infrastructure.QueryHandlers.SystemAdmin;

/// <summary>
/// Handler for GetUserMetricsQuery - queries real user data.
/// </summary>
public class GetUserMetricsQueryHandler : IQueryHandler<GetUserMetricsQuery, UserMetrics>
{
    private readonly ApplicationDbContext _context;

    public GetUserMetricsQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<UserMetrics>> Handle(
        GetUserMetricsQuery request,
        CancellationToken cancellationToken)
    {
        var totalUsers = await _context.Users.CountAsync(u => !u.IsDeleted, cancellationToken);
        var activeDoctors = await _context.Ophthalmologists
            .CountAsync(o => o.IsVerified, cancellationToken);
        var patientsScreened = await _context.Patients.CountAsync(cancellationToken);
        var pendingApprovals = await _context.Ophthalmologists
            .CountAsync(o => o.VerificationStatus == VerificationStatus.PendingVerification, cancellationToken);

        var dto = new UserMetrics
        {
            TotalUsers = totalUsers,
            TotalUsersMonthlyChange = 0,
            ActiveDoctors = activeDoctors,
            ActiveDoctorsChange = 0,
            PatientsScreened = patientsScreened,
            PatientsScreenedChange = 0,
            PendingApprovals = pendingApprovals
        };

        return Result<UserMetrics>.Success(dto);
    }
}
