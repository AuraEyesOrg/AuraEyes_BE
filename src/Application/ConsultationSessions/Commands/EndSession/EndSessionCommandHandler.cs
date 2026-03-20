using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Financial;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EndSessionCommandHandler> _logger;

    public EndSessionCommandHandler(
        IConsultationSessionRepository sessionRepository,
        IAppointmentSlotRepository slotRepository,
        IWalletRepository walletRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IUnitOfWork unitOfWork,
        ILogger<EndSessionCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _slotRepository = slotRepository;
        _walletRepository = walletRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
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
            session.EndSession(request.DoctorId);

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

            // ── 3. Wallet capture: transfer consultation fee to doctor ──
            if (session.Price > 0 && session.OphthalmologistId.HasValue)
            {
                var doctor = await _ophthalmologistRepository.GetByIdAsync(
                    session.OphthalmologistId.Value, cancellationToken);

                if (doctor is not null)
                {
                    var doctorWallet = await _walletRepository.GetByUserIdAsync(
                        doctor.UserId, cancellationToken);

                    if (doctorWallet is not null)
                    {
                        doctorWallet.Deposit(session.Price,
                            $"Consultation earnings – Session {session.Id}");

                        var earningsTx = new WalletTransaction(
                            doctorWallet.Id,
                            session.Price,
                            TransactionType.Transfer,
                            "Consultation earnings",
                            referenceType: "Booking",
                            referenceId: session.Id);

                        doctorWallet.AddTransaction(earningsTx);
                        await _walletRepository.AddTransactionAsync(earningsTx, cancellationToken);

                        _logger.LogInformation(
                            "Captured {Amount} VND to doctor wallet {WalletId} for session {SessionId}.",
                            session.Price, doctorWallet.Id, session.Id);
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Doctor {DoctorId} has no wallet. Earnings for session {SessionId} not captured.",
                            doctor.Id, session.Id);
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
