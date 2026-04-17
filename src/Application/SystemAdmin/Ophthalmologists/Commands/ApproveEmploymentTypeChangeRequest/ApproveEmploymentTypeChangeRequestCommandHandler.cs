using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.ScheduleTemplates.Interfaces;
using Application.SystemAdmin.Ophthalmologists.Commands.BackfillFullTimeSchedule;
using Application.SystemAdmin.Ophthalmologists.Commands.DeleteFutureOphthalmologistSlots;
using Application.SystemAdmin.Ophthalmologists.Interfaces;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Ophthalmologists.Commands.ApproveEmploymentTypeChangeRequest;

public class ApproveEmploymentTypeChangeRequestCommandHandler : ICommandHandler<ApproveEmploymentTypeChangeRequestCommand, ApproveEmploymentTypeChangeRequestResultDto>
{
    private const int FullTimeTransitionBackfillWindowDays = 7;

    private readonly IOphthalmologistEmploymentTypeChangeRequestRepository _requestRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IOphthalmologistContractProvisioningService _contractProvisioningService;
    private readonly IFullTimeTemplateProvisioningService _fullTimeTemplateProvisioningService;
    private readonly ISender _sender;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveEmploymentTypeChangeRequestCommandHandler> _logger;

    public ApproveEmploymentTypeChangeRequestCommandHandler(
        IOphthalmologistEmploymentTypeChangeRequestRepository requestRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IContractRepository contractRepository,
        IOphthalmologistContractProvisioningService contractProvisioningService,
        IFullTimeTemplateProvisioningService fullTimeTemplateProvisioningService,
        ISender sender,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ILogger<ApproveEmploymentTypeChangeRequestCommandHandler> logger)
    {
        _requestRepository = requestRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _contractRepository = contractRepository;
        _contractProvisioningService = contractProvisioningService;
        _fullTimeTemplateProvisioningService = fullTimeTemplateProvisioningService;
        _sender = sender;
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

        if (!ophthalmologist.IsVerified)
        {
            return Result<ApproveEmploymentTypeChangeRequestResultDto>.Conflict(
                "Only verified ophthalmologists can change employment type.");
        }

        if (ophthalmologist.EmploymentType != changeRequest.CurrentEmploymentType)
        {
            return Result<ApproveEmploymentTypeChangeRequestResultDto>.Conflict(
                "Employment type has changed since the request was created. Please submit a new request.");
        }

        var currentContract = await _contractRepository.GetByUserIdAsync(ophthalmologist.UserId, cancellationToken);
        if (currentContract is null || currentContract.Status != ContractStatus.Active)
        {
            return Result<ApproveEmploymentTypeChangeRequestResultDto>.Conflict(
                "Approval requires an active contract on the ophthalmologist profile.");
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

            var deleteFutureSlotsResult = await _sender.Send(
                new DeleteFutureOphthalmologistSlotsCommand
                {
                    OphthalmologistId = ophthalmologist.Id
                },
                cancellationToken);

            if (!deleteFutureSlotsResult.IsSuccess)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ApproveEmploymentTypeChangeRequestResultDto>.Failure(deleteFutureSlotsResult.ErrorMessage);
            }

            if (targetEmploymentType == OphthalmologistEmploymentType.FullTime)
            {
                var backfillResult = await _sender.Send(
                    new BackfillFullTimeScheduleCommand
                    {
                        OphthalmologistId = ophthalmologist.Id,
                        WindowDays = FullTimeTransitionBackfillWindowDays
                    },
                    cancellationToken);

                if (!backfillResult.IsSuccess)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ApproveEmploymentTypeChangeRequestResultDto>.Failure(backfillResult.ErrorMessage);
                }
            }

            if (targetEmploymentType == OphthalmologistEmploymentType.FullTime)
            {
                await _fullTimeTemplateProvisioningService.EnsureSystemGeneratedTemplatesAsync(
                    ophthalmologist,
                    cancellationToken);
            }

            currentContract.Expire();
            await _contractRepository.UpdateAsync(currentContract, cancellationToken);

            var newContract = await _contractProvisioningService.CreatePendingContractForEmploymentTypeAsync(
                ophthalmologist,
                cancellationToken);

            if (newContract is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ApproveEmploymentTypeChangeRequestResultDto>.Failure(
                    "No active contract template found for the target employment type.");
            }

            await _contractRepository.AddAsync(newContract, cancellationToken);
            await _requestRepository.UpdateAsync(changeRequest, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            try
            {
                await _notificationService.SendAsync(
                    ophthalmologist.UserId,
                    "Yêu cầu chuyển loại hình làm việc đã được duyệt",
                    $"Yêu cầu chuyển từ {previousEmploymentType} sang {targetEmploymentType} đã được phê duyệt. Vui lòng ký hợp đồng mới để kích hoạt lại tài khoản.",
                    NotificationType.SystemAlert,
                    new
                    {
                        RequestId = changeRequest.Id,
                        PreviousEmploymentType = previousEmploymentType.ToString(),
                        TargetEmploymentType = targetEmploymentType.ToString(),
                        Status = changeRequest.Status.ToString(),
                        NewContractId = newContract.Id
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
                ExpiredContractId = currentContract.Id,
                NewPendingContractId = newContract.Id
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
