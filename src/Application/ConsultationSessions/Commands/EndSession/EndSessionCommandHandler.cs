using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.ConsultationSessions.Commands.EndSession;

/// <summary>
/// Doctor completes the session:
///   - Session → Completed, ChatStatus → Archived.
///   - Slot → Completed.
/// </summary>
public class EndSessionCommandHandler : ICommandHandler<EndSessionCommand>
{
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EndSessionCommandHandler> _logger;

    public EndSessionCommandHandler(
        IConsultationSessionRepository sessionRepository,
        IUnitOfWork unitOfWork,
        ILogger<EndSessionCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(EndSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
        if (session is null)
            return Result.NotFound($"Session '{request.SessionId}' not found.");

        var validationResult = ValidateEndSession(session, request.DoctorId);
        if (!validationResult.IsSuccess)
            return validationResult;

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // ── 1. Complete the session ──
            session.EndSession(request.DoctorId);

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

    private static Result ValidateEndSession(Domain.Entities.Consultation.ConsultationSession session, Guid doctorId)
    {
        if (session.Status == SessionStatus.Completed)
            return Result.Failure("Session is already completed.");

        if (session.OphthalmologistId.HasValue && session.OphthalmologistId.Value != doctorId)
            return Result.Forbidden("Only the assigned ophthalmologist can end this session.");

        return Result.Success();
    }
}
