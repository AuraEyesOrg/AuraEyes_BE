using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using Domain.Enums;

namespace Application.Ophthalmologists.Commands.UpdateOphthalmologist;

/// <summary>
/// Handler for UpdateOphthalmologistCommand.
/// </summary>
public class UpdateOphthalmologistCommandHandler : ICommandHandler<UpdateOphthalmologistCommand>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;     
    private readonly IIdentityService _identityService;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOphthalmologistCommandHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        IIdentityService identityService,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _identityService = identityService;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateOphthalmologistCommand request, CancellationToken cancellationToken)
    {
        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(request.Id, cancellationToken);

        if (ophthalmologist is null)
        {
            return Result.NotFound($"Ophthalmologist with ID '{request.Id}' was not found.");
        }

        // Update profile using domain method
        ophthalmologist.UpdateProfile(request.Bio, request.YearsOfExperience);  

        bool statusChangedToPending = false;

        // Update credentials if provided
        if (!string.IsNullOrWhiteSpace(request.DegreeUrl) || !string.IsNullOrWhiteSpace(request.LicenseUrl))
        {
            var oldStatus = ophthalmologist.VerificationStatus;
            ophthalmologist.UpdateCredentialFiles(request.LicenseUrl, request.DegreeUrl);

            if (oldStatus == VerificationStatus.Approved && ophthalmologist.VerificationStatus == VerificationStatus.PendingUpdate)
            {
                statusChangedToPending = true;
            }
        }

        // Self-profile flow can also update user identity fields.
        if (request.UserId.HasValue)
        {
            var (succeeded, errors) = await _identityService.UpdateUserProfileAsync(
                request.UserId.Value,
                request.FullName ?? string.Empty,
                request.Phone,
                null,
                null,
                request.Address,
                cancellationToken);

            if (!succeeded)
            {
                return Result.Failure(errors);
            }
        }

        await _ophthalmologistRepository.UpdateAsync(ophthalmologist, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notify SystemAdmin if an already verified doctor uploaded new credentials
        if (statusChangedToPending)
        {
            var (systemAdmins, _) = await _identityService.GetUsersAsync(roleFilter: "SystemAdmin", pageNumber: 1, pageSize: 1000, cancellationToken: cancellationToken);
            foreach (var admin in systemAdmins)
            {
                await _notificationService.SendAsync(      
                    admin.Id,
                    "Chờ duyệt chứng chỉ",
                    $"Bác sĩ {request.FullName ?? "ẩn danh"} vừa cập nhật thêm chứng chỉ, vui lòng kiểm tra và duyệt lại.",
                    NotificationType.SystemAlert,
                    payload: new { action = "pending_update_verification" },    
                    cancellationToken: cancellationToken,
                    referenceId: ophthalmologist.Id);
            }
        }

        return Result.Success();
    }
}
