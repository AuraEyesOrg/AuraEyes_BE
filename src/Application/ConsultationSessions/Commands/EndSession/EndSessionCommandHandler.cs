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
///   - Wallet capture: transfer consultation fee to the doctor's wallet.
/// </summary>
public class EndSessionCommandHandler : ICommandHandler<EndSessionCommand>
{
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

            // ── 3. Wallet capture: ensure patient payment exists, then transfer fee to doctor ──
            if (session.Price > 0 && session.OphthalmologistId.HasValue)
            {
                var patient = await _patientRepository.GetByIdAsync(session.PatientId, cancellationToken);
                if (patient is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.NotFound($"Patient '{session.PatientId}' not found.");
                }

                var patientWallet = await _walletRepository.GetByUserIdWithTransactionsAsync(
                    patient.UserId,
                    cancellationToken);

                if (patientWallet is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure("Patient wallet not found.");
                }

                var hasPrepaidBooking = patientWallet.Transactions.Any(tx =>
                    tx.TransactionType == TransactionType.Payment
                    && tx.ReferenceType == "Booking"
                    && tx.ReferenceId.HasValue
                    && (tx.ReferenceId.Value == session.Id
                        || (session.AppointmentSlotId.HasValue
                            && tx.ReferenceId.Value == session.AppointmentSlotId.Value)));

                if (!hasPrepaidBooking)
                {
                    if (patientWallet.Balance < session.Price)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return Result.Failure(
                            $"Insufficient patient wallet balance. Required: {session.Price:N0} VND, Available: {patientWallet.Balance:N0} VND.");
                    }

                    patientWallet.Withdraw(session.Price,
                        $"Consultation payment – Session {session.Id}");

                    var patientPaymentTx = new WalletTransaction(
                        patientWallet.Id,
                        session.Price,
                        TransactionType.Payment,
                        "Consultation payment",
                        referenceType: "Booking",
                        referenceId: session.Id);

                    patientWallet.AddTransaction(patientPaymentTx);
                    await _walletRepository.AddTransactionAsync(patientPaymentTx, cancellationToken);
                }

                var doctor = await _ophthalmologistRepository.GetByIdAsync(
                    session.OphthalmologistId.Value, cancellationToken);

                if (doctor is not null)
                {
                    // CommissionRate on Ophthalmologist is stored as 0–100 (platform's share of the session fee).
                    var commissionPercent = doctor.CommissionRate ?? 0m;
                    var platformShare = Math.Round(
                        session.Price * (commissionPercent / 100m),
                        0,
                        MidpointRounding.AwayFromZero);
                    var doctorShare = session.Price - platformShare;
                    var doctorSharePercent = 100m - commissionPercent;

                    if (doctorShare < 0 || platformShare < 0)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return Result.Failure("Invalid commission configuration for this ophthalmologist.");
                    }

                    var doctorWallet = await _walletRepository.GetByUserIdAsync(
                        doctor.UserId, cancellationToken);

                    if (doctorShare > 0 && doctorWallet is null)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return Result.Failure("Doctor wallet not found; consultation earnings cannot be recorded.");
                    }

                    if (doctorWallet is not null && doctorShare > 0)
                    {
                        var doctorNote = $"Consultation earnings: {doctorShare:N0} VND (Receive {doctorSharePercent:0.##}% from original fee {session.Price:N0} VND) – Session {session.Id}";
                        
                        doctorWallet.Deposit(doctorShare, doctorNote);

                        var earningsTx = new WalletTransaction(
                            doctorWallet.Id,
                            doctorShare,
                            TransactionType.Deposit,
                            $"Consultation earnings (After deducting {commissionPercent:0.##}% platform fee)",
                            referenceType: "Booking",
                            referenceId: session.Id);

                        doctorWallet.AddTransaction(earningsTx);
                        await _walletRepository.AddTransactionAsync(earningsTx, cancellationToken);

                        _logger.LogInformation(
                            "Captured {DoctorShare} VND (platform {PlatformShare} VND) to doctor wallet {WalletId} for session {SessionId}.",
                            doctorShare, platformShare, doctorWallet.Id, session.Id);
                    }

                    if (platformShare > 0)
                    {
                        var platformWallet = await _walletRepository.GetSystemWalletAsync(cancellationToken);
                        if (platformWallet is null)
                        {
                            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                            return Result.Failure(
                                "System (platform) wallet not found. Ensure a wallet with OwnerType \"System\" exists.");
                        }

                        var platformNote =
                            $"Platform commission: {platformShare:N0} VND ({commissionPercent:0.##}% of {session.Price:N0} VND) – Session {session.Id}";
                        platformWallet.Deposit(platformShare, platformNote);

                        var platformTx = new WalletTransaction(
                            platformWallet.Id,
                            platformShare,
                            TransactionType.Deposit,
                            $"Platform commission ({commissionPercent:0.##}% of fee)",
                            referenceType: "Booking",
                            referenceId: session.Id);

                        platformWallet.AddTransaction(platformTx);
                        await _walletRepository.AddTransactionAsync(platformTx, cancellationToken);
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
            _logger.LogError(ex, "Error ending session {SessionId}", request.SessionId);
            throw;
        }
    }
}
