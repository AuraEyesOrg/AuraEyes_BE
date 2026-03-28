using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Financial;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.ConsultationSessions.Commands.CancelSession;

/// <summary>
/// Enforces cancellation policy:
///   - 3-hour rule: forbidden when T ≤ 3 h before StartTime.
///   - Patient anti-spam: max 3 cancellations per day.
///   - Doctor cancels → slot is burned (locked).
///   - Patient cancels → slot released back to Available.
///   - 100 % wallet refund to patient.
/// </summary>
public class CancelSessionCommandHandler : ICommandHandler<CancelSessionCommand>
{
    private const int MaxPatientCancelsPerDay = 3;
    private static readonly TimeSpan CancellationCutoff = TimeSpan.FromHours(3);

    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IAppointmentSlotRepository _slotRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IGoogleMeetService _googleMeetService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CancelSessionCommandHandler> _logger;

    public CancelSessionCommandHandler(
        IConsultationSessionRepository sessionRepository,
        IAppointmentSlotRepository slotRepository,
        IWalletRepository walletRepository,
        IRepository<Patient> patientRepository,
        IGoogleMeetService googleMeetService,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<CancelSessionCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _slotRepository = slotRepository;
        _walletRepository = walletRepository;
        _patientRepository = patientRepository;
        _googleMeetService = googleMeetService;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result> Handle(CancelSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
        if (session is null)
            return Result.NotFound($"Session '{request.SessionId}' not found.");

        if (session.Status == SessionStatus.Completed)
            return Result.Failure("Cannot cancel a completed session.");

        if (session.Status == SessionStatus.Cancelled)
            return Result.Failure("Session is already cancelled.");

        // ── 1. The 3-Hour Rule ──
        if (session.AppointmentTime.HasValue)
        {
            var timeUntilStart = session.AppointmentTime.Value - DateTime.UtcNow;
            if (timeUntilStart <= CancellationCutoff)
            {
                return Result.Forbidden(
                    "Cancellation is not allowed within 3 hours of the appointment time.");
            }
        }

        // ── 2. Determine who is cancelling ──
        if (!_currentUserService.ProfileId.HasValue)
        {
            return Result.Forbidden("Unable to resolve user profile for cancellation.");
        }
        var cancelledByProfileId = _currentUserService.ProfileId.Value;

        bool isCancelledByPatient = session.PatientId == cancelledByProfileId;
        bool isCancelledByDoctor = session.OphthalmologistId.HasValue
                                   && session.OphthalmologistId.Value == cancelledByProfileId;

        if (!isCancelledByPatient && !isCancelledByDoctor)
        {
            return Result.Forbidden("You are not allowed to cancel this session.");
        }

        // ── 3. Patient anti-spam: 3 cancellations / day ──
        if (isCancelledByPatient)
        {
            var cancelledToday = await _sessionRepository.CountCancelledTodayByPatientAsync(
                session.PatientId, cancellationToken);

            if (cancelledToday >= MaxPatientCancelsPerDay)
            {
                return Result.Forbidden(
                    $"You have cancelled {MaxPatientCancelsPerDay} sessions today. " +
                    "Please wait 24 hours before booking again.");
            }
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // ── 4. Cancel the session ──
            var reason = request.Reason ?? (isCancelledByDoctor ? "DoctorCancelled" : "PatientCancelled");
            session.Cancel(cancelledByProfileId, reason);

            // ── 5. Delete Google Meet event ──
            if (!string.IsNullOrEmpty(session.CalendarEventId))
            {
                try
                {
                    await _googleMeetService.DeleteMeetingAsync(session.CalendarEventId, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "Failed to delete calendar event {CalendarEventId} for session {SessionId}.",
                        session.CalendarEventId, session.Id);
                }
                session.ClearMeetingInfo();
            }

            // ── 6. Slot: burn (doctor) or release (patient) ──
            if (session.AppointmentSlotId.HasValue)
            {
                var slot = await _slotRepository.GetByIdWithLockAsync(
                    session.AppointmentSlotId.Value, cancellationToken);

                if (slot is not null && slot.Status == ScheduleStatus.Booked)
                {
                    if (isCancelledByDoctor)
                    {
                        slot.CancelBooking(); // Release back to Available first to allow cancellation
                        slot.Cancel(); // "Burn" the slot — Cancelled, no rebooking
                        _logger.LogInformation(
                            "Slot {SlotId} burned (doctor-cancelled session {SessionId}).",
                            slot.Id, session.Id);
                    }
                    else
                    {
                        slot.CancelBooking(); // Release back to Available
                        _logger.LogInformation(
                            "Slot {SlotId} released back to Available (patient-cancelled session {SessionId}).",
                            slot.Id, session.Id);
                    }
                }
            }

            // ── 7. Refund only if patient actually paid for this booking ──
            if (session.Price > 0)
            {
                var patient = await _patientRepository.GetByIdAsync(session.PatientId, cancellationToken);
                if (patient is not null)
                {
                    var wallet = await _walletRepository.GetByUserIdWithTransactionsAsync(patient.UserId, cancellationToken);
                    if (wallet is not null)
                    {
                        var hasBookingPayment = wallet.Transactions.Any(tx =>
                            tx.TransactionType == TransactionType.Payment
                            && tx.ReferenceType == "Booking"
                            && tx.ReferenceId.HasValue
                            && (tx.ReferenceId.Value == session.Id
                                || (session.AppointmentSlotId.HasValue
                                    && tx.ReferenceId.Value == session.AppointmentSlotId.Value)));

                        if (!hasBookingPayment)
                        {
                            _logger.LogInformation(
                                "Skipped refund for session {SessionId} because no booking payment transaction was found.",
                                session.Id);
                        }
                        else
                        {
                            wallet.Deposit(session.Price, $"Refund – cancelled session {session.Id}");

                            var refundTx = new WalletTransaction(
                                wallet.Id,
                                session.Price,
                                TransactionType.Refund,
                                "Consultation cancellation refund",
                                referenceType: "Booking",
                                referenceId: session.Id);

                            wallet.AddTransaction(refundTx);
                            await _walletRepository.AddTransactionAsync(refundTx, cancellationToken);

                            _logger.LogInformation(
                                "Refunded {Amount} VND to patient wallet {WalletId} for session {SessionId}.",
                                session.Price, wallet.Id, session.Id);
                        }
                    }
                }
            }

            await _sessionRepository.UpdateAsync(session, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Error cancelling session {SessionId}", request.SessionId);
            throw;
        }
    }
}
