using Application.Common.Helpers;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Scheduling;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Ophthalmologists.Commands.ApproveLeaveRequest;

public class ApproveLeaveRequestCommandHandler : ICommandHandler<ApproveLeaveRequestCommand, ApproveLeaveRequestResultDto>
{
    private readonly IOphthalmologistLeaveRequestRepository _leaveRequestRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IConsultationSessionRepository _consultationSessionRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveLeaveRequestCommandHandler> _logger;

    public ApproveLeaveRequestCommandHandler(
        IOphthalmologistLeaveRequestRepository leaveRequestRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IConsultationSessionRepository consultationSessionRepository,
        IAppointmentSlotRepository appointmentSlotRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ILogger<ApproveLeaveRequestCommandHandler> logger)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _consultationSessionRepository = consultationSessionRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ApproveLeaveRequestResultDto>> Handle(
        ApproveLeaveRequestCommand request,
        CancellationToken cancellationToken)
    {
        var leaveRequest = await _leaveRequestRepository.GetByIdAsync(request.LeaveRequestId, cancellationToken);
        if (leaveRequest is null)
        {
            return Result<ApproveLeaveRequestResultDto>.NotFound(
                $"Leave request '{request.LeaveRequestId}' was not found.");
        }

        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(leaveRequest.OphthalmologistId, cancellationToken);
        if (ophthalmologist is null)
        {
            return Result<ApproveLeaveRequestResultDto>.NotFound(
                $"Ophthalmologist '{leaveRequest.OphthalmologistId}' was not found.");
        }

        if (ophthalmologist.EmploymentType != OphthalmologistEmploymentType.FullTime)
        {
            return Result<ApproveLeaveRequestResultDto>.Conflict(
                "Only full-time ophthalmologists can use leave requests.");
        }

        if (leaveRequest.Status != OphthalmologistLeaveRequestStatus.Pending)
        {
            return Result<ApproveLeaveRequestResultDto>.Conflict(
                "Only pending leave requests can be approved.");
        }

        var pendingNotifications = new List<PendingNotification>();

        // Rule 4: Check and deduct leave days fund
        var requestedDays = (int)(leaveRequest.EndDate.ToDateTime(TimeOnly.MinValue) - leaveRequest.StartDate.ToDateTime(TimeOnly.MinValue)).TotalDays + 1;
        if (ophthalmologist.AvailableLeaveDays < requestedDays)
        {
            return Result<ApproveLeaveRequestResultDto>.Conflict(
                $"Bác sĩ không đủ ngày phép. Cần: {requestedDays} Hiện có: {ophthalmologist.AvailableLeaveDays}");
        }

        // Rule 1: No Pending Appointments - Check for active sessions/appointments
        var sessionWindowStartUtc = GetUtcStartOfDay(leaveRequest.StartDate);
        var sessionWindowEndUtcExclusive = GetUtcStartOfDay(leaveRequest.EndDate.AddDays(1));

        var activeSessions = await _consultationSessionRepository.Query()
            .Where(x => x.OphthalmologistId == leaveRequest.OphthalmologistId)
            .Where(x => x.Status != SessionStatus.Completed && x.Status != SessionStatus.Cancelled)
            .Where(x => x.AppointmentTime.HasValue
                        && x.AppointmentTime.Value >= sessionWindowStartUtc
                        && x.AppointmentTime.Value < sessionWindowEndUtcExclusive)
            .AnyAsync(cancellationToken);

        var activeAppointments = await _appointmentSlotRepository.Query()
            .Include(x => x.Appointments)
            .Where(x => x.OphthalId == leaveRequest.OphthalmologistId)
            .Where(x => x.Date >= leaveRequest.StartDate && x.Date <= leaveRequest.EndDate)
            .SelectMany(x => x.Appointments)
            .Where(a => a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.CheckedIn || a.Status == AppointmentStatus.InProgress)
            .AnyAsync(cancellationToken);

        if (activeSessions || activeAppointments)
        {
            return Result<ApproveLeaveRequestResultDto>.Conflict(
                "Không thể duyệt đơn. Bác sĩ vẫn còn các ca hẹn hoặc phiên tư vấn chưa được xử lý (chuyển giao hoặc đổi lịch).");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Deduct leave days
            ophthalmologist.DeductLeaveDays(requestedDays);
            await _ophthalmologistRepository.UpdateAsync(ophthalmologist, cancellationToken);

            leaveRequest.Approve(request.ReviewedByAdminUserId, request.AdminNote);

            // Block appointment slots for the leave period
            var slots = await _appointmentSlotRepository.Query()
                .Where(x => x.OphthalId == leaveRequest.OphthalmologistId)
                .Where(x => x.Date >= leaveRequest.StartDate && x.Date <= leaveRequest.EndDate)
                .Where(x => x.Status != ScheduleStatus.Blocked)
                .ToListAsync(cancellationToken);

            var blockedSlots = 0;
            foreach (var slot in slots)
            {
                slot.ForceBlock();
                blockedSlots++;
            }

            await _leaveRequestRepository.UpdateAsync(leaveRequest, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            pendingNotifications.Add(new PendingNotification(
                ophthalmologist.UserId,
                "Yêu cầu nghỉ phép đã được duyệt",
                $"Yêu cầu nghỉ phép từ {leaveRequest.StartDate:dd/MM/yyyy} đến {leaveRequest.EndDate:dd/MM/yyyy} đã được phê duyệt.",
                NotificationType.SystemAlert,
                new
                {
                    LeaveRequestId = leaveRequest.Id,
                    Status = leaveRequest.Status.ToString(),
                    StartDate = leaveRequest.StartDate,
                    EndDate = leaveRequest.EndDate
                },
                leaveRequest.Id));

            await SendNotificationsAsync(pendingNotifications, cancellationToken);

            return Result<ApproveLeaveRequestResultDto>.Success(new ApproveLeaveRequestResultDto
            {
                LeaveRequestId = leaveRequest.Id,
                CancelledConsultationSessions = 0,
                CancelledAppointments = 0,
                BlockedSlots = blockedSlots
            });
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<ApproveLeaveRequestResultDto>.Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving leave request {RequestId}", request.LeaveRequestId);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private static DateTime GetUtcStartOfDay(DateOnly date)
    {
        var localDateTime = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(localDateTime, VietnamTimeZoneResolver.TimeZone);
    }

    private async Task SendNotificationsAsync(
        IReadOnlyCollection<PendingNotification> notifications,
        CancellationToken cancellationToken)
    {
        foreach (var notification in notifications)
        {
            try
            {
                await _notificationService.SendAsync(
                    notification.UserId,
                    notification.Title,
                    notification.Message,
                    notification.Type,
                    notification.Payload,
                    cancellationToken,
                    notification.ReferenceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send leave-request notification to user {UserId}.",
                    notification.UserId);
            }
        }
    }

    private sealed record PendingNotification(
        Guid UserId,
        string Title,
        string Message,
        NotificationType Type,
        object Payload,
        Guid? ReferenceId);
}
