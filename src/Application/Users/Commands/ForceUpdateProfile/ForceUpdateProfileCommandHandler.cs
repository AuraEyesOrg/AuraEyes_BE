using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using Application.Common.Constants;
using MediatR;
using Microsoft.Extensions.Logging;
using Application.Ophthalmologists.Commands.UpdateOphthalmologist;
using Application.ClinicStaffs.Commands.UpdateClinicStaffProfile;

namespace Application.Users.Commands.ForceUpdateProfile;

public class ForceUpdateProfileCommandHandler : ICommandHandler<ForceUpdateProfileCommand, bool>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly ILogger<ForceUpdateProfileCommandHandler> _logger;

    public ForceUpdateProfileCommandHandler(
        IIdentityService identityService,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IMediator mediator,
        IOphthalmologistRepository ophthalmologistRepository,
        ILogger<ForceUpdateProfileCommandHandler> logger)
    {
        _identityService = identityService;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _ophthalmologistRepository = ophthalmologistRepository;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(ForceUpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<bool>.Failure("Unauthorized access.");
        }

        var roles = await _identityService.GetUserRolesAsync(userId.Value);
        
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. Dispatch Role-Specific Update
            if (roles.Contains(Roles.Ophthalmologist))
            {
                var ophthalmologist = await _ophthalmologistRepository.GetByUserIdAsync(userId.Value, cancellationToken);
                if (ophthalmologist == null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure("Ophthalmologist profile not found.");
                }

                var updateCmd = new UpdateOphthalmologistCommand
                {
                    Id = ophthalmologist.Id,
                    UserId = userId.Value,
                    FullName = request.FullName,
                    Phone = request.Phone,
                    Address = request.Address,
                    Bio = request.Bio,
                    ConsultationFee = request.ConsultationFee,
                    DateOfBirth = request.DateOfBirth,
                    Gender = (int?)request.Gender,
                    CitizenId = request.CitizenId
                };
                
                var ophthalmologistResult = await _mediator.Send(updateCmd, cancellationToken);
                if (!ophthalmologistResult.IsSuccess)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure(ophthalmologistResult.Errors);
                }
            }
            else if (roles.Contains(Roles.ClinicStaff))
            {
                var updateCmd = new UpdateClinicStaffProfileCommand
                {
                    UserId = userId.Value,
                    FullName = request.FullName,
                    Phone = request.Phone,
                    Address = request.Address,
                    DateOfBirth = request.DateOfBirth?.ToString("yyyy-MM-dd"),
                    Gender = request.Gender?.ToString(),
                    CitizenId = request.CitizenId,
                    Department = request.Department,
                    EmployeeCode = request.EmployeeCode
                };
                
                var staffResult = await _mediator.Send(updateCmd, cancellationToken);
                if (!staffResult.IsSuccess)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure(staffResult.Errors);
                }
            }

            // 2. Update Avatar if provided
            if (!string.IsNullOrEmpty(request.AvatarUrl))
            {
                await _identityService.UpdateAvatarUrlAsync(userId.Value, request.AvatarUrl, cancellationToken);
            }

            // 3. Change Password (if provided)
            if (!string.IsNullOrEmpty(request.NewPassword) && !string.IsNullOrEmpty(request.CurrentPassword))
            {
                var passwordResult = await _identityService.ChangePasswordAsync(userId.Value, request.CurrentPassword, request.NewPassword, cancellationToken);
                
                if (!passwordResult.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure(passwordResult.Errors);
                }
            }

            // 4. Clear MustUpdateProfile flag
            await _identityService.ClearMustUpdateProfileFlagAsync(userId.Value);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during force profile update for user {UserId}", userId);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
