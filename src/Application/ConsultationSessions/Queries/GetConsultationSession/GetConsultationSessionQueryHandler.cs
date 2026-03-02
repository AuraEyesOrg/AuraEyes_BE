using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.ConsultationSessions.Common;
using Domain.Repositories;

namespace Application.ConsultationSessions.Queries.GetConsultationSession;

public class GetConsultationSessionQueryHandler
    : IQueryHandler<GetConsultationSessionQuery, ConsultationSessionDto>
{
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly ICurrentUserService _currentUser;

    public GetConsultationSessionQueryHandler(
        IConsultationSessionRepository sessionRepository,
        ICurrentUserService currentUser)
    {
        _sessionRepository = sessionRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<ConsultationSessionDto>> Handle(
        GetConsultationSessionQuery request,
        CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdWithConversationsAsync(
            request.SessionId, cancellationToken);
        if (session is null)
        {
            return Result<ConsultationSessionDto>.NotFound(
                $"Session with ID '{request.SessionId}' was not found.");
        }

        var isAdmin = _currentUser.Roles.Any(r => Roles.Admins.Contains(r));
        var isParticipant = _currentUser.ProfileId.HasValue
                            && (session.PatientId == _currentUser.ProfileId.Value
                                || session.OphthalmologistId == _currentUser.ProfileId.Value);

        if (!isAdmin && !isParticipant)
        {
           return Result<ConsultationSessionDto>.Forbidden(
               "You are not a participant of this session.");
        }

        // Flatten all messages from all conversations, ordered by SentAt
        var messages = session.Conversations
            .SelectMany(c => c.Messages)
            .OrderBy(m => m.SentAt)
            .Select(m => new ChatMessageDto
            {
                Id = m.Id,
                SenderUserId = m.SenderUserId,
                Message = m.Message,
                IsRead = m.IsRead,
                SentAt = m.SentAt,
            })
            .ToList();

        var dto = new ConsultationSessionDto
        {
            Id = session.Id,
            PatientId = session.PatientId,
            OphthalmologistId = session.OphthalmologistId,
            OrganisationId = session.OrganisationId,
            AiScreeningId = session.AiScreeningId,
            Type = session.Type,
            Status = session.Status,
            ChatStatus = session.ChatStatus,
            Price = session.Price,
            AppointmentTime = session.AppointmentTime,
            MeetingLink = session.MeetingLink,
            LastActivityAt = session.LastActivityAt,
            ClosedAt = session.ClosedAt,
            ClosedBy = session.ClosedBy,
            ClosingReason = session.ClosingReason,
            CreatedAt = session.CreatedAt,
            UpdatedAt = session.UpdatedAt,
            Messages = messages
        };

        return Result<ConsultationSessionDto>.Success(dto);
    }
}
