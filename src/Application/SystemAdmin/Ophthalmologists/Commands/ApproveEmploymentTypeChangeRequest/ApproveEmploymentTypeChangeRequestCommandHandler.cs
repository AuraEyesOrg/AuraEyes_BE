using Application.Common.Interfaces;
using Application.Common.Models;

using Application.SystemAdmin.Ophthalmologists.Interfaces;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Ophthalmologists.Commands.ApproveEmploymentTypeChangeRequest;

public class ApproveEmploymentTypeChangeRequestCommandHandler : ICommandHandler<ApproveEmploymentTypeChangeRequestCommand, ApproveEmploymentTypeChangeRequestResultDto>
{
    private readonly IOphthalmologistEmploymentTypeChangeRequestRepository _requestRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveEmploymentTypeChangeRequestCommandHandler> _logger;
    public ApproveEmploymentTypeChangeRequestCommandHandler(
        IOphthalmologistEmploymentTypeChangeRequestRepository requestRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ILogger<ApproveEmploymentTypeChangeRequestCommandHandler> logger)
    {
        _requestRepository = requestRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ApproveEmploymentTypeChangeRequestResultDto>> Handle(
        ApproveEmploymentTypeChangeRequestCommand request,
        CancellationToken cancellationToken)
    {
        var changeRequest = await _requestRepository.GetByIdAsync(request.RequestId, cancellationToken);
        if (changeRequest is null)
        {
            return Result<ApproveEmploymentTypeChangeRequestResultDto>.NotFound(
                $"Employment type change request '{request.RequestId}' was not found.");
        }

        if (changeRequest.Status != OphthalmologistEmploymentTypeChangeRequestStatus.Pending)
        {
            return Result<ApproveEmploymentTypeChangeRequestResultDto>.Conflict(
                "Only pending employment type change requests can be approved.");
        }

        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(changeRequest.OphthalmologistId, cancellationToken);
        if (ophthalmologist is null)
        {
            return Result<ApproveEmploymentTypeChangeRequestResultDto>.NotFound(
                $"Ophthalmologist '{changeRequest.OphthalmologistId}' was not found.");
        }

        if (ophthalmologist.EmploymentType != changeRequest.CurrentEmploymentType)
        {
            return Result<ApproveEmploymentTypeChangeRequestResultDto>.Conflict(
                "Employment type has changed since the request was created. Please submit a new request.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            changeRequest.Approve(request.ReviewedByAdminUserId, request.AdminNote);

            var previousEmploymentType = changeRequest.CurrentEmploymentType;
            var targetEmploymentType = changeRequest.TargetEmploymentType;

            ophthalmologist.UpdateEmploymentPreferences(
                targetEmploymentType,
                ophthalmologist.WorkingHoursPerWeek,
                ophthalmologist.ExpectedMonthlySalary);

            await _ophthalmologistRepository.UpdateAsync(ophthalmologist, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Since scheduling is single-clinic, we don't backfill or delete doctor-specific slots here.

            await _requestRepository.UpdateAsync(changeRequest, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            try
            {
                await _notificationService.SendAsync(
                    ophthalmologist.UserId,
                    "Yêu cầu chuyển loại hình làm việc đã được duyệt",
                    $"Yêu cầu chuyển từ {previousEmploymentType} sang {targetEmploymentType} đã được phê duyệt.",
                    NotificationType.SystemAlert,
                    new
                    {
                        RequestId = changeRequest.Id,
                        PreviousEmploymentType = previousEmploymentType.ToString(),
                        TargetEmploymentType = targetEmploymentType.ToString(),
                        Status = changeRequest.Status.ToString()
                    },
                    cancellationToken,
                    changeRequest.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send employment-type-change approval notification to ophthalmologist user {UserId}.",
                    ophthalmologist.UserId);
            }

            return Result<ApproveEmploymentTypeChangeRequestResultDto>.Success(new ApproveEmploymentTypeChangeRequestResultDto
            {
                RequestId = changeRequest.Id,
                PreviousEmploymentType = previousEmploymentType,
                TargetEmploymentType = targetEmploymentType,
                ExpiredContractId = null,
                NewPendingContractId = null
            });
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<ApproveEmploymentTypeChangeRequestResultDto>.Conflict(ex.Message);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
