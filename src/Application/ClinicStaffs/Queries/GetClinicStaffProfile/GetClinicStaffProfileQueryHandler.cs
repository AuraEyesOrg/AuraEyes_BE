using Application.ClinicStaffs.Common;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Repositories;

namespace Application.ClinicStaffs.Queries.GetClinicStaffProfile;

public class GetClinicStaffProfileQueryHandler : IQueryHandler<GetClinicStaffProfileQuery, ClinicStaffProfileDto>
{
    private readonly IClinicStaffRepository _clinicStaffRepository;
    private readonly IIdentityService _identityService;

    public GetClinicStaffProfileQueryHandler(
        IClinicStaffRepository clinicStaffRepository,
        IIdentityService identityService)
    {
        _clinicStaffRepository = clinicStaffRepository;
        _identityService = identityService;
    }

    public async Task<Result<ClinicStaffProfileDto>> Handle(
        GetClinicStaffProfileQuery request,
        CancellationToken cancellationToken)
    {
        var staff = await _clinicStaffRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (staff is null)
            return Result<ClinicStaffProfileDto>.NotFound("Clinic staff profile not found.");

        var user = await _identityService.GetUserByIdAsync(request.UserId, cancellationToken);
        var userDetails = await _identityService.GetUserDetailsAsync(request.UserId, cancellationToken);
        var twoFactorEnabled = await _identityService.IsTwoFactorEnabledAsync(request.UserId);

        if (user is null || userDetails is null)
            return Result<ClinicStaffProfileDto>.NotFound("User not found.");

        return Result<ClinicStaffProfileDto>.Success(new ClinicStaffProfileDto
        {
            Id = staff.Id,
            UserId = staff.UserId,
            Email = user.Email,
            FullName = userDetails.FullName,
            Phone = userDetails.PhoneNumber,
            DateOfBirth = userDetails.DateOfBirth,
            Gender = userDetails.Gender?.ToString().ToLowerInvariant(),
            Address = userDetails.Address,
            CitizenId = userDetails.CitizenId,
            AvatarUrl = userDetails.AvatarUrl,
            Department = staff.Department,
            EmployeeCode = staff.EmployeeCode,
            SubRoles = staff.GetSubRoles().Select(x => x.ToString()).ToList().AsReadOnly(),
            IsEmailVerified = user.EmailConfirmed,
            IsTwoFactorEnabled = twoFactorEnabled,
            CreatedAt = staff.CreatedAt,
            UpdatedAt = staff.UpdatedAt,
        });
    }
}

