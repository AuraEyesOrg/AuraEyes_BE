using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.ConsultationSessions.Commands.CancelSession;

public class CancelSessionCommandHandler : ICommandHandler<CancelSessionCommand>
{
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IGoogleMeetService _googleMeetService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelSessionCommandHandler> _logger;

    public CancelSessionCommandHandler(
        IConsultationSessionRepository sessionRepository,
        IGoogleMeetService googleMeetService,
        IUnitOfWork unitOfWork,
        ILogger<CancelSessionCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _googleMeetService = googleMeetService;
        _unitOfWork = unitOfWork;
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

        var reason = request.Reason ?? "UserCancelled";

        session.Cancel(request.CancelledByUserId, reason);

        if (!string.IsNullOrEmpty(session.CalendarEventId))
        {
            try
            {
                await _googleMeetService.DeleteMeetingAsync(session.CalendarEventId, cancellationToken);
                _logger.LogInformation(
                    "Deleted calendar event {CalendarEventId} for cancelled session {SessionId}",
                    session.CalendarEventId, session.Id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Failed to delete calendar event {CalendarEventId} for session {SessionId}. Event may be orphaned.",
                    session.CalendarEventId, session.Id);
            }

            session.ClearMeetingInfo();
        }

        await _sessionRepository.UpdateAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
