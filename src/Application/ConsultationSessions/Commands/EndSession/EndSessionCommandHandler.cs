using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Financial;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.ConsultationSessions.Commands.EndSession;

/// <summary>
/// Doctor completes the session:
///   - Session → Completed, ChatStatus → Archived.
///   - Slot → Completed.
///   - Wallet capture (Escrow → Doctor + SystemAdmin commission), idempotent by session id.
/// </summary>
public class EndSessionCommandHandler : ICommandHandler<EndSessionCommand>
{
    private const string BookingReferenceType = "Booking";

    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IAppointmentSlotRepository _slotRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EndSessionCommandHandler> _logger;

    public EndSessionCommandHandler(
        IConsultationSessionRepository sessionRepository,
        IAppointmentSlotRepository slotRepository,
        IWalletRepository walletRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IRepository<Patient> patientRepository,
        IUnitOfWork unitOfWork,
        ILogger<EndSessionCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _slotRepository = slotRepository;
        _walletRepository = walletRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(EndSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
        if (session is null)
            return Result.NotFound($"Session '{request.SessionId}' not found.");

        if (session.Status == SessionStatus.Completed)
            return Result.Failure("Session is already completed.");

        if (session.OphthalmologistId.HasValue && session.OphthalmologistId.Value != request.DoctorId)
            return Result.Forbidden("Only the assigned ophthalmologist can end this session.");

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // ── 1. Complete the session ──
            var closingReason = string.IsNullOrWhiteSpace(request.Reason)
                ? "DoctorFinished"
                : request.Reason.Trim();
            session.EndSession(request.DoctorId, closingReason);

            // ── 2. Complete the linked slot ──
            if (session.AppointmentSlotId.HasValue)
            {
                var slot = await _slotRepository.GetByIdWithLockAsync(
                    session.AppointmentSlotId.Value, cancellationToken);

                if (slot is not null && slot.Status == ScheduleStatus.Booked)
                {
                    slot.Complete();
                }
            }

            // ── 3. Wallet capture: Escrow → Doctor + SystemAdmin commission ──
            if (session.Price > 0 && session.OphthalmologistId.HasValue)
            {
                var captureResult = await CaptureFromEscrowAsync(session, cancellationToken);
                if (!captureResult.IsSuccess)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return captureResult;
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
            _logger.LogError(ex, "Error ending session {SessionId}", request.SessionId);
            throw;
        }
    }

    /// <summary>
    /// Capture the booking amount from Escrow and split it between the Doctor wallet
    /// and the SystemAdmin commission wallet. Fully idempotent: if the doctor already has
    /// a Deposit transaction for this session, the whole step is a no-op.
    /// </summary>
    private async Task<Result> CaptureFromEscrowAsync(
        Domain.Entities.Consultation.ConsultationSession session,
        CancellationToken cancellationToken)
    {
        var doctor = await _ophthalmologistRepository.GetByIdAsync(
            session.OphthalmologistId!.Value, cancellationToken);
        if (doctor is null)
            return Result.NotFound($"Ophthalmologist '{session.OphthalmologistId}' not found.");

        var doctorWallet = await _walletRepository.GetByUserIdAsync(doctor.UserId, cancellationToken);
        if (doctorWallet is null)
            return Result.Failure("Doctor wallet not found; consultation earnings cannot be recorded.");

        // Idempotency guard: has this session already been captured?
        var alreadyCaptured = await _walletRepository.HasTransactionAsync(
            doctorWallet.Id,
            TransactionType.Deposit,
            BookingReferenceType,
            session.Id,
            cancellationToken);

        if (alreadyCaptured)
        {
            _logger.LogInformation(
                "Session {SessionId} already captured to doctor wallet {WalletId}; skipping.",
                session.Id, doctorWallet.Id);
            return Result.Success();
        }

        var commissionPercent = doctor.CommissionRate ?? 0m;
        if (commissionPercent < 0m || commissionPercent > 100m)
            return Result.Failure("Invalid commission configuration for this ophthalmologist.");

        var platformShare = Math.Round(
            session.Price * (commissionPercent / 100m),
            0,
            MidpointRounding.AwayFromZero);
        var doctorShare = session.Price - platformShare;
        var doctorSharePercent = 100m - commissionPercent;

        if (doctorShare < 0 || platformShare < 0)
            return Result.Failure("Invalid commission split calculated for this session.");

        // ── Escrow guard — ensure patient's booking was placed in escrow ──
        var escrowWallet = await _walletRepository.GetEscrowWalletAsync(cancellationToken);
        if (escrowWallet is null)
            return Result.Failure(
                "Platform escrow wallet is not configured. Please contact support.");

        if (escrowWallet.Balance < session.Price)
        {
            _logger.LogError(
                "Escrow balance {Balance} is insufficient to capture session {SessionId} (Price={Price}).",
                escrowWallet.Balance, session.Id, session.Price);
            return Result.Failure(
                "Escrow balance is insufficient to capture this session. " +
                "The booking payment may be missing or already released.");
        }

        // Withdraw the full price from escrow (double-entry).
        escrowWallet.Withdraw(session.Price, $"Escrow release – Session {session.Id}");

        var escrowReleaseTx = new WalletTransaction(
            escrowWallet.Id,
            session.Price,
            TransactionType.Withdrawal,
            $"Escrow release for session {session.Id}",
            referenceType: BookingReferenceType,
            referenceId: session.Id);

        escrowWallet.AddTransaction(escrowReleaseTx);
        await _walletRepository.AddTransactionAsync(escrowReleaseTx, cancellationToken);

        // Credit doctor wallet.
        if (doctorShare > 0)
        {
            var doctorNote =
                $"Consultation earnings: {doctorShare:N0} VND (Receive {doctorSharePercent:0.##}% from original fee {session.Price:N0} VND) – Session {session.Id}";

            doctorWallet.Deposit(doctorShare, doctorNote);

            var earningsTx = new WalletTransaction(
                doctorWallet.Id,
                doctorShare,
                TransactionType.Deposit,
                $"Consultation earnings (After deducting {commissionPercent:0.##}% platform fee)",
                referenceType: BookingReferenceType,
                referenceId: session.Id);

            doctorWallet.AddTransaction(earningsTx);
            await _walletRepository.AddTransactionAsync(earningsTx, cancellationToken);

            _logger.LogInformation(
                "Captured {DoctorShare} VND (platform {PlatformShare} VND) to doctor wallet {WalletId} for session {SessionId}.",
                doctorShare, platformShare, doctorWallet.Id, session.Id);
        }

        // Credit SystemAdmin commission wallet.
        if (platformShare > 0)
        {
            var platformWallet = await _walletRepository.GetSystemWalletAsync(cancellationToken);
            if (platformWallet is null)
            {
                return Result.Failure(
                    "System commission wallet not found. Ensure a wallet with OwnerType \"System\" exists.");
            }

            var platformNote =
                $"Platform commission: {platformShare:N0} VND ({commissionPercent:0.##}% of {session.Price:N0} VND) – Session {session.Id}";
            platformWallet.Deposit(platformShare, platformNote);

            var platformTx = new WalletTransaction(
                platformWallet.Id,
                platformShare,
                TransactionType.Deposit,
                $"Platform commission ({commissionPercent:0.##}% of fee)",
                referenceType: BookingReferenceType,
                referenceId: session.Id);

            platformWallet.AddTransaction(platformTx);
            await _walletRepository.AddTransactionAsync(platformTx, cancellationToken);
        }

        return Result.Success();
    }
}
