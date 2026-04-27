using Application.Common.Helpers;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Consultation;
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
    private readonly IRepository<Patient> _patientRepository;
    private readonly IGoogleMeetService _googleMeetService;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveLeaveRequestCommandHandler> _logger;

    public ApproveLeaveRequestCommandHandler(
        IOphthalmologistLeaveRequestRepository leaveRequestRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IConsultationSessionRepository consultationSessionRepository,
        IAppointmentSlotRepository appointmentSlotRepository,
        IRepository<Patient> patientRepository,
        IGoogleMeetService googleMeetService,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ILogger<ApproveLeaveRequestCommandHandler> logger)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _consultationSessionRepository = consultationSessionRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
        _patientRepository = patientRepository;
        _googleMeetService = googleMeetService;
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

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            leaveRequest.Approve(request.ReviewedByAdminUserId, request.AdminNote);

            var sessionWindowStartUtc = GetUtcStartOfDay(leaveRequest.StartDate);
            var sessionWindowEndUtcExclusive = GetUtcStartOfDay(leaveRequest.EndDate.AddDays(1));

            var sessions = await _consultationSessionRepository.Query()
                .Where(x => x.OphthalmologistId == leaveRequest.OphthalmologistId)
                .Where(x => x.Status != SessionStatus.Completed && x.Status != SessionStatus.Cancelled)
                .Where(x => x.AppointmentTime.HasValue
                            && x.AppointmentTime.Value >= sessionWindowStartUtc
                            && x.AppointmentTime.Value < sessionWindowEndUtcExclusive)
                .ToListAsync(cancellationToken);

            var patientIds = sessions.Select(x => x.PatientId).ToHashSet();

            var patientUserMap = await _patientRepository.Query()
                .Where(x => patientIds.Contains(x.Id))
                .Select(x => new { x.Id, x.UserId })
                .ToDictionaryAsync(x => x.Id, x => x.UserId, cancellationToken);

            var cancelledSessions = 0;
            foreach (var session in sessions)
            {
                session.Cancel(request.ReviewedByAdminUserId, "CancelledDueToApprovedDoctorLeave");
                cancelledSessions++;

                await TryDeleteCalendarEventAsync(session, cancellationToken);

                if (session.AppointmentSlotId.HasValue)
                {
                    var sessionSlot = await _appointmentSlotRepository.GetByIdWithLockAsync(session.AppointmentSlotId.Value, cancellationToken);
                    if (sessionSlot != null)
                    {
                        ReleaseBookedOrReservedState(sessionSlot);
                        await _appointmentSlotRepository.UpdateAsync(sessionSlot, cancellationToken);
                    }
                }

                if (patientUserMap.TryGetValue(session.PatientId, out var patientUserId)
                    && patientUserId.HasValue)
                {
                    pendingNotifications.Add(new PendingNotification(
                        patientUserId.Value,
                        "Lịch tư vấn bị hủy",
                        BuildSessionCancellationMessage(session.AppointmentTime),
                        NotificationType.ScheduleChanged,
                        new
                        {
                            ConsultationSessionId = session.Id,
                            AppointmentSlotId = session.AppointmentSlotId,
                            LeaveRequestId = leaveRequest.Id,
                            Action = "DoctorLeaveApproved"
                        },
                        session.Id));
                }
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
                CancelledConsultationSessions = cancelledSessions,
                CancelledAppointments = 0,
                BlockedSlots = 0
            });
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<ApproveLeaveRequestResultDto>.Conflict(ex.Message);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private static DateTime GetUtcStartOfDay(DateOnly date)
    {
        var localDateTime = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(localDateTime, VietnamTimeZoneResolver.TimeZone);
    }

    private static string BuildSessionCancellationMessage(DateTime? appointmentTimeUtc)
    {
        if (!appointmentTimeUtc.HasValue)
        {
            return "Lịch tư vấn của bạn đã bị hủy do bác sĩ nghỉ phép được phê duyệt.";
        }

        var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(appointmentTimeUtc.Value, VietnamTimeZoneResolver.TimeZone);
        return $"Lịch tư vấn lúc {vietnamTime:HH:mm} ngày {vietnamTime:dd/MM/yyyy} đã bị hủy do bác sĩ nghỉ phép được phê duyệt.";
    }

    private async Task TryDeleteCalendarEventAsync(
        ConsultationSession session,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(session.CalendarEventId))
        {
            return;
        }

        try
        {
            await _googleMeetService.DeleteMeetingAsync(session.CalendarEventId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Failed to delete calendar event {CalendarEventId} for cancelled leave session {SessionId}.",
                session.CalendarEventId,
                session.Id);
        }

        session.ClearMeetingInfo();
    }

    private static void ReleaseBookedOrReservedState(Domain.Entities.Scheduling.AppointmentSlot slot)
    {
        if (slot.BookedCount > 0)
        {
            slot.CancelBooking();
        }
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
