using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Ophthalmologists.Commands.RejectEmploymentTypeChangeRequest;

public class RejectEmploymentTypeChangeRequestCommandHandler : ICommandHandler<RejectEmploymentTypeChangeRequestCommand>
{
    private readonly IOphthalmologistEmploymentTypeChangeRequestRepository _requestRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RejectEmploymentTypeChangeRequestCommandHandler> _logger;

    public RejectEmploymentTypeChangeRequestCommandHandler(
        IOphthalmologistEmploymentTypeChangeRequestRepository requestRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ILogger<RejectEmploymentTypeChangeRequestCommandHandler> logger)
    {
        _requestRepository = requestRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(RejectEmploymentTypeChangeRequestCommand request, CancellationToken cancellationToken)
    {
        var changeRequest = await _requestRepository.GetByIdAsync(request.RequestId, cancellationToken);
        if (changeRequest is null)
        {
            return Result.NotFound($"Employment type change request '{request.RequestId}' was not found.");
        }

        if (changeRequest.Status != OphthalmologistEmploymentTypeChangeRequestStatus.Pending)
        {
            return Result.Conflict("Only pending employment type change requests can be rejected.");
        }

        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(changeRequest.OphthalmologistId, cancellationToken);
        if (ophthalmologist is null)
        {
            return Result.NotFound($"Ophthalmologist '{changeRequest.OphthalmologistId}' was not found.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            changeRequest.Reject(request.ReviewedByAdminUserId, request.AdminNote);
            await _requestRepository.UpdateAsync(changeRequest, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Conflict(ex.Message);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        try
        {
            await _notificationService.SendAsync(
                ophthalmologist.UserId,
                "Yêu cầu chuyển loại hình làm việc đã bị từ chối",
                $"Yêu cầu chuyển từ {changeRequest.CurrentEmploymentType} sang {changeRequest.TargetEmploymentType} đã bị từ chối.",
                NotificationType.SystemAlert,
                new
                {
                    RequestId = changeRequest.Id,
                    CurrentEmploymentType = changeRequest.CurrentEmploymentType.ToString(),
                    TargetEmploymentType = changeRequest.TargetEmploymentType.ToString(),
                    Status = changeRequest.Status.ToString(),
                    AdminNote = changeRequest.AdminNote
                },
                cancellationToken,
                changeRequest.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send employment-type-change rejection notification to ophthalmologist user {UserId}.",
                ophthalmologist.UserId);
        }

        return Result.Success();
    }
}
