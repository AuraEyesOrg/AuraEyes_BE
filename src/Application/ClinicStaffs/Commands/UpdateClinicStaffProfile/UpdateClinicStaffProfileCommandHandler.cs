using Application.ClinicStaffs.Common;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.ClinicStaffs.Commands.UpdateClinicStaffProfile;

public class UpdateClinicStaffProfileCommandHandler : ICommandHandler<UpdateClinicStaffProfileCommand, ClinicStaffProfileDto>
{
    private readonly IClinicStaffRepository _clinicStaffRepository;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateClinicStaffProfileCommandHandler(
        IClinicStaffRepository clinicStaffRepository,
        IIdentityService identityService,
        IUnitOfWork unitOfWork)
    {
        _clinicStaffRepository = clinicStaffRepository;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ClinicStaffProfileDto>> Handle(
        UpdateClinicStaffProfileCommand request,
        CancellationToken cancellationToken)
    {
        var staff = await _clinicStaffRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (staff is null)
            return Result<ClinicStaffProfileDto>.NotFound("Clinic staff profile not found.");

        int? genderValue = request.Gender?.ToLowerInvariant() switch
        {
            "male" => (int)Gender.Male,
            "female" => (int)Gender.Female,
            "other" => (int)Gender.Other,
            "prefernottotsay" => (int)Gender.PreferNotToSay,
            _ => null
        };

        DateTime? dateOfBirth = null;
        if (!string.IsNullOrWhiteSpace(request.DateOfBirth) &&
            DateTime.TryParse(request.DateOfBirth, out var dob))
        {
            dateOfBirth = DateTime.SpecifyKind(dob, DateTimeKind.Utc);
        }

        var (succeeded, errors) = await _identityService.UpdateUserProfileAsync(
            request.UserId,
            request.FullName,
            request.Phone,
            dateOfBirth,
            genderValue,
            request.Address,
            request.CitizenId,
            cancellationToken);

        if (!succeeded)
            return Result<ClinicStaffProfileDto>.Failure(errors);

        staff.UpdateProfile(request.Department, request.EmployeeCode, request.Phone);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

