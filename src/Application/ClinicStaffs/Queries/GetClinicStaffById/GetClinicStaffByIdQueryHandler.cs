using Application.ClinicStaffs.Common;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Repositories;

namespace Application.ClinicStaffs.Queries.GetClinicStaffById;

/// <summary>
/// Returns a single ClinicStaff with joined Identity user details.
/// </summary>
public class GetClinicStaffByIdQueryHandler : IQueryHandler<GetClinicStaffByIdQuery, ClinicStaffResponse>
{
    private readonly IClinicStaffRepository _clinicStaffRepository;
    private readonly IIdentityService _identityService;

    public GetClinicStaffByIdQueryHandler(
        IClinicStaffRepository clinicStaffRepository,
        IIdentityService identityService)
    {
        _clinicStaffRepository = clinicStaffRepository;
        _identityService = identityService;
    }

    public async Task<Result<ClinicStaffResponse>> Handle(
        GetClinicStaffByIdQuery request,
        CancellationToken cancellationToken)
    {
        var staff = await _clinicStaffRepository.GetByIdAsync(request.StaffId, cancellationToken);
        if (staff is null)
            return Result<ClinicStaffResponse>.NotFound($"ClinicStaff '{request.StaffId}' was not found.");

        var user = await _identityService.GetUserByIdAsync(staff.UserId, cancellationToken);

        var response = new ClinicStaffResponse(
            staff.Id,
            staff.UserId,
            user?.FullName ?? string.Empty,
            user?.Email ?? string.Empty,
            user?.AvatarUrl,
            staff.GetSubRoles().Select(r => r.ToString()).ToList().AsReadOnly(),
            staff.Department,
            staff.EmployeeCode,
            staff.Phone,
            staff.IsActive,
            staff.CreatedAt,
            staff.UpdatedAt);

        return Result<ClinicStaffResponse>.Success(response);
    }
}
