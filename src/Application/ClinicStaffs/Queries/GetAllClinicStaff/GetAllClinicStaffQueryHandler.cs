using Application.ClinicStaffs.Common;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities.Users;
using Domain.Repositories;

namespace Application.ClinicStaffs.Queries.GetAllClinicStaff;

/// <summary>
/// Returns all active clinic staff, joined with Identity user info.
/// </summary>
public class GetAllClinicStaffQueryHandler
    : IQueryHandler<GetAllClinicStaffQuery, IReadOnlyList<ClinicStaffResponse>>
{
    private readonly IClinicStaffRepository _clinicStaffRepository;
    private readonly IIdentityService _identityService;

    public GetAllClinicStaffQueryHandler(
        IClinicStaffRepository clinicStaffRepository,
        IIdentityService identityService)
    {
        _clinicStaffRepository = clinicStaffRepository;
        _identityService = identityService;
    }

    public async Task<Result<IReadOnlyList<ClinicStaffResponse>>> Handle(
        GetAllClinicStaffQuery request,
        CancellationToken cancellationToken)
    {
        var staffList = request.SubRole.HasValue
            ? await _clinicStaffRepository.GetBySubRoleAsync(request.SubRole.Value, cancellationToken)
            : await _clinicStaffRepository.GetAllActiveAsync(cancellationToken);

        // Batch-fetch user info for all staff members
        var userIds = staffList.Select(s => s.UserId).Distinct().ToList();
        var userMap = new Dictionary<Guid, (string FullName, string Email, string? AvatarUrl)>();

        foreach (var userId in userIds)
        {
            var user = await _identityService.GetUserByIdAsync(userId, cancellationToken);
            if (user is not null)
                userMap[userId] = (user.FullName, user.Email, user.AvatarUrl);
        }

        var responses = staffList.Select(staff =>
        {
            userMap.TryGetValue(staff.UserId, out var userInfo);
            return new ClinicStaffResponse(
                staff.Id,
                staff.UserId,
                userInfo.FullName ?? string.Empty,
                userInfo.Email ?? string.Empty,
                userInfo.AvatarUrl,
                staff.GetSubRoles().Select(r => r.ToString()).ToList().AsReadOnly(),
                staff.Department,
                staff.EmployeeCode,
                staff.Phone,
                staff.IsActive,
                staff.CreatedAt,
                staff.UpdatedAt);
        }).ToList().AsReadOnly();

        return Result<IReadOnlyList<ClinicStaffResponse>>.Success(responses);
    }
}
