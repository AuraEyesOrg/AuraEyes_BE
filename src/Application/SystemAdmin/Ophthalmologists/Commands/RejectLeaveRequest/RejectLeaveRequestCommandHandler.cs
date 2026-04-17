using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Ophthalmologists.Commands.RejectLeaveRequest;

public class RejectLeaveRequestCommandHandler : ICommandHandler<RejectLeaveRequestCommand>
{
    private readonly IOphthalmologistLeaveRequestRepository _leaveRequestRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RejectLeaveRequestCommandHandler> _logger;

    public RejectLeaveRequestCommandHandler(
        IOphthalmologistLeaveRequestRepository leaveRequestRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ILogger<RejectLeaveRequestCommandHandler> logger)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(RejectLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var leaveRequest = await _leaveRequestRepository.GetByIdAsync(request.LeaveRequestId, cancellationToken);
        if (leaveRequest is null)
        {
            return Result.NotFound($"Leave request '{request.LeaveRequestId}' was not found.");
        }

        if (leaveRequest.Status != OphthalmologistLeaveRequestStatus.Pending)
        {
            return Result.Conflict("Only pending leave requests can be rejected.");
        }

        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(leaveRequest.OphthalmologistId, cancellationToken);
        if (ophthalmologist is null)
        {
            return Result.NotFound($"Ophthalmologist '{leaveRequest.OphthalmologistId}' was not found.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            leaveRequest.Reject(request.ReviewedByAdminUserId, request.AdminNote);
            await _leaveRequestRepository.UpdateAsync(leaveRequest, cancellationToken);
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
                "Yêu cầu nghỉ phép đã bị từ chối",
                $"Yêu cầu nghỉ phép từ {leaveRequest.StartDate:dd/MM/yyyy} đến {leaveRequest.EndDate:dd/MM/yyyy} đã bị từ chối.",
                NotificationType.SystemAlert,
                new
                {
                    LeaveRequestId = leaveRequest.Id,
                    Status = leaveRequest.Status.ToString(),
                    StartDate = leaveRequest.StartDate,
                    EndDate = leaveRequest.EndDate,
                    AdminNote = leaveRequest.AdminNote
                },
                cancellationToken,
                leaveRequest.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send leave rejection notification to ophthalmologist user {UserId}.",
                ophthalmologist.UserId);
        }

        return Result.Success();
    }
}
