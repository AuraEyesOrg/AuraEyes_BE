using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
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
/// </summary>
public class CancelSessionCommandHandler : ICommandHandler<CancelSessionCommand>
{
    private const int MaxPatientCancelsPerDay = 3;
    private static readonly TimeSpan CancellationCutoff = TimeSpan.FromHours(3);

    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CancelSessionCommandHandler> _logger;

    public CancelSessionCommandHandler(
        IConsultationSessionRepository sessionRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<CancelSessionCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result> Handle(CancelSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
        if (session is null)
            return Result.NotFound($"Session '{request.SessionId}' not found.");

        var validationResult = await ValidateCancellationPolicyAsync(session, cancellationToken);
        if (!validationResult.IsSuccess)
            return validationResult;

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // ── 4. Cancel the session ──
            session.Cancel();

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

    private async Task<Result> ValidateCancellationPolicyAsync(
        Domain.Entities.Consultation.ConsultationSession session,
        CancellationToken cancellationToken)
    {
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

        return Result.Success();
    }
}
