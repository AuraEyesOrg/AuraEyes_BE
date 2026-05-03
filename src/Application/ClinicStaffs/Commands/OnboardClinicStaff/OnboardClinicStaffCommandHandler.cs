using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.ClinicStaffs.Commands.OnboardClinicStaff;

public class OnboardClinicStaffCommandHandler : ICommandHandler<OnboardClinicStaffCommand, bool>
{
    private readonly IIdentityService _identityService;
    private readonly IClinicStaffRepository _clinicStaffRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<OnboardClinicStaffCommandHandler> _logger;

    public OnboardClinicStaffCommandHandler(
        IIdentityService identityService,
        IClinicStaffRepository clinicStaffRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<OnboardClinicStaffCommandHandler> logger)
    {
        _identityService = identityService;
        _clinicStaffRepository = clinicStaffRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(OnboardClinicStaffCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
            return Result<bool>.Failure("Unauthorized access.");

        var staff = await _clinicStaffRepository.GetByUserIdAsync(userId.Value, cancellationToken);
        if (staff == null)
            return Result<bool>.NotFound("Clinic staff profile not found.");

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. Update Identity Profile
            var (userSucceeded, userErrors) = await _identityService.UpdateUserProfileAsync(
                userId.Value,
                request.FullName,
                request.Phone,
                request.DateOfBirth,
                request.Gender,
                request.Address,
                request.CitizenId,
                cancellationToken);

            if (!userSucceeded)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.Failure(userErrors);
            }

            // 2. Change Password if provided
            if (!string.IsNullOrWhiteSpace(request.NewPassword) && !string.IsNullOrWhiteSpace(request.CurrentPassword))
            {
                var (pwdSucceeded, pwdErrors) = await _identityService.ChangePasswordAsync(
                    userId.Value,
                    request.CurrentPassword,
                    request.NewPassword,
                    cancellationToken);

                if (!pwdSucceeded)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure(pwdErrors);
                }
            }

            // 3. Update Avatar if provided
            if (!string.IsNullOrWhiteSpace(request.AvatarUrl))
            {
                await _identityService.UpdateAvatarUrlAsync(userId.Value, request.AvatarUrl, cancellationToken);
            }

            // 4. Update Staff Details
            staff.UpdateProfile(request.Department, request.EmployeeCode, request.Phone);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 5. Clear MustUpdateProfile flag
            var (clearSucceeded, clearErrors) = await _identityService.ClearMustUpdateProfileFlagAsync(userId.Value);
            if (!clearSucceeded)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.Failure(clearErrors);
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during clinic staff onboarding for user {UserId}", userId);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<bool>.Failure("An internal error occurred during onboarding.");
        }
    }
}
