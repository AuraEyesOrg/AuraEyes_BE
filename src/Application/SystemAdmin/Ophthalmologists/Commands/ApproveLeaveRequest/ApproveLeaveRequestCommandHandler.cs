using Application.Common.Helpers;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Financial;
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
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IGoogleMeetService _googleMeetService;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveLeaveRequestCommandHandler> _logger;

    public ApproveLeaveRequestCommandHandler(
        IOphthalmologistLeaveRequestRepository leaveRequestRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IConsultationSessionRepository consultationSessionRepository,
        IAppointmentRepository appointmentRepository,
        IAppointmentSlotRepository appointmentSlotRepository,
        IRepository<Patient> patientRepository,
        IWalletRepository walletRepository,
        IGoogleMeetService googleMeetService,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ILogger<ApproveLeaveRequestCommandHandler> logger)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _consultationSessionRepository = consultationSessionRepository;
        _appointmentRepository = appointmentRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
        _patientRepository = patientRepository;
        _walletRepository = walletRepository;
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

            var slotsInRange = (await _appointmentSlotRepository.GetByOphthalmologistAsync(
                leaveRequest.OphthalmologistId,
                leaveRequest.StartDate,
                leaveRequest.EndDate,
                null,
                cancellationToken)).ToList();

            var slotMap = slotsInRange.ToDictionary(x => x.Id, x => x);

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

            var appointments = await _appointmentRepository.GetByDoctorAsync(
                leaveRequest.OphthalmologistId,
                leaveRequest.StartDate,
                leaveRequest.EndDate,
                null,
                null,
                cancellationToken);

            foreach (var appointment in appointments)
            {
                patientIds.Add(appointment.PatientId);
            }

            var patientUserMap = await _patientRepository.Query()
                .Where(x => patientIds.Contains(x.Id))
                .Select(x => new { x.Id, x.UserId })
                .ToDictionaryAsync(x => x.Id, x => x.UserId, cancellationToken);

            var walletCache = new Dictionary<Guid, Wallet?>();

            var cancelledSessions = 0;
            foreach (var session in sessions)
            {
                session.Cancel(request.ReviewedByAdminUserId, "CancelledDueToApprovedDoctorLeave");
                cancelledSessions++;

                await TryDeleteCalendarEventAsync(session, cancellationToken);

                if (session.AppointmentSlotId.HasValue
                    && slotMap.TryGetValue(session.AppointmentSlotId.Value, out var sessionSlot))
                {
                    ReleaseBookedOrReservedState(sessionSlot);
                }

                await TryRefundSessionAsync(session, patientUserMap, walletCache, cancellationToken);

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

            var cancelledAppointments = 0;
            foreach (var appointment in appointments.Where(IsCancellableAppointment))
            {
                appointment.Cancel(request.ReviewedByAdminUserId, "CancelledDueToApprovedDoctorLeave");
                cancelledAppointments++;

                if (slotMap.TryGetValue(appointment.AppointmentSlotId, out var appointmentSlot))
                {
                    ReleaseBookedOrReservedState(appointmentSlot);
                }

                if (patientUserMap.TryGetValue(appointment.PatientId, out var patientUserId)
                    && patientUserId.HasValue)
                {
                    pendingNotifications.Add(new PendingNotification(
                        patientUserId.Value,
                        "Lịch hẹn bị hủy",
                        BuildAppointmentCancellationMessage(appointment.AppointmentSlot?.Date, appointment.AppointmentSlot?.StartTime),
                        NotificationType.ScheduleChanged,
                        new
                        {
                            AppointmentId = appointment.Id,
                            AppointmentSlotId = appointment.AppointmentSlotId,
                            LeaveRequestId = leaveRequest.Id,
                            Action = "DoctorLeaveApproved"
                        },
                        appointment.Id));
                }
            }

            var blockedSlots = 0;
            foreach (var slot in slotsInRange)
            {
                if (TryBlockSlot(slot))
                {
                    blockedSlots++;
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
                CancelledAppointments = cancelledAppointments,
                BlockedSlots = blockedSlots
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

    private static bool IsCancellableAppointment(Domain.Entities.Scheduling.Appointment appointment)
    {
        return appointment.Status == AppointmentStatus.Pending
            || appointment.Status == AppointmentStatus.Confirmed
            || appointment.Status == AppointmentStatus.CheckedIn;
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

    private static string BuildAppointmentCancellationMessage(DateOnly? date, TimeOnly? startTime)
    {
        if (!date.HasValue || !startTime.HasValue)
        {
            return "Lịch hẹn của bạn đã bị hủy do bác sĩ nghỉ phép được phê duyệt.";
        }

        return $"Lịch hẹn lúc {startTime:HH\\:mm} ngày {date:dd/MM/yyyy} đã bị hủy do bác sĩ nghỉ phép được phê duyệt.";
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
        if (slot.Status == ScheduleStatus.Booked && slot.BookedCount > 0)
        {
            slot.CancelBooking();
            return;
        }

        if (slot.Status == ScheduleStatus.Reserved)
        {
            slot.ReleaseReservation();
        }
    }

    private static bool TryBlockSlot(Domain.Entities.Scheduling.AppointmentSlot slot)
    {
        if (slot.Status == ScheduleStatus.Blocked
            || slot.Status == ScheduleStatus.Cancelled
            || slot.Status == ScheduleStatus.Completed
            || slot.Status == ScheduleStatus.NoShow)
        {
            return false;
        }

        if (slot.Status == ScheduleStatus.Reserved)
        {
            slot.ReleaseReservation();
        }

        if (slot.Status == ScheduleStatus.Booked)
        {
            if (slot.BookedCount > 0)
            {
                return false;
            }

            slot.UpdateStatus(ScheduleStatus.Available);
        }

        slot.Block();
        return true;
    }

    private async Task TryRefundSessionAsync(
        ConsultationSession session,
        IReadOnlyDictionary<Guid, Guid?> patientUserMap,
        IDictionary<Guid, Wallet?> walletCache,
        CancellationToken cancellationToken)
    {
        if (session.Price <= 0)
        {
            return;
        }

        if (!patientUserMap.TryGetValue(session.PatientId, out var patientUserId)
            || !patientUserId.HasValue)
        {
            return;
        }

        if (!walletCache.TryGetValue(patientUserId.Value, out var wallet))
        {
            wallet = await _walletRepository.GetByUserIdWithTransactionsAsync(patientUserId.Value, cancellationToken);
            walletCache[patientUserId.Value] = wallet;
        }

        if (wallet is null)
        {
            return;
        }

        var hasBookingPayment = wallet.Transactions.Any(tx =>
            tx.TransactionType == TransactionType.Payment
            && tx.ReferenceType == "Booking"
            && tx.ReferenceId.HasValue
            && (tx.ReferenceId.Value == session.Id
                || (session.AppointmentSlotId.HasValue
                    && tx.ReferenceId.Value == session.AppointmentSlotId.Value)));

        if (!hasBookingPayment)
        {
            return;
        }

        // Idempotency guard — avoid double refund if this method is replayed.
        var alreadyRefunded = await _walletRepository.HasTransactionAsync(
            wallet.Id,
            TransactionType.Refund,
            "Booking",
            session.Id,
            cancellationToken);

        if (alreadyRefunded)
        {
            return;
        }

        // Release from Escrow first so the ledger stays balanced.
        var escrowWallet = await _walletRepository.GetEscrowWalletAsync(cancellationToken);
        if (escrowWallet is null)
        {
            _logger.LogError(
                "Escrow wallet missing while refunding session {SessionId} (doctor leave).",
                session.Id);
            return;
        }

        if (escrowWallet.Balance < session.Price)
        {
            _logger.LogError(
                "Escrow balance {Balance} insufficient to refund session {SessionId} (doctor leave, Price={Price}).",
                escrowWallet.Balance, session.Id, session.Price);
            return;
        }

        escrowWallet.Withdraw(session.Price, $"Escrow refund – session {session.Id} (doctor leave)");

        var escrowRefundTx = new WalletTransaction(
            escrowWallet.Id,
            session.Price,
            TransactionType.Withdrawal,
            $"Escrow release (refund, doctor leave) – session {session.Id}",
            referenceType: "Booking",
            referenceId: session.Id);

        escrowWallet.AddTransaction(escrowRefundTx);
        await _walletRepository.AddTransactionAsync(escrowRefundTx, cancellationToken);

        wallet.Deposit(session.Price, $"Refund – session {session.Id} cancelled due to doctor leave");

        var refundTx = new WalletTransaction(
            wallet.Id,
            session.Price,
            TransactionType.Refund,
            "Consultation cancellation refund (doctor leave)",
            referenceType: "Booking",
            referenceId: session.Id);

        wallet.AddTransaction(refundTx);
        await _walletRepository.AddTransactionAsync(refundTx, cancellationToken);
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
