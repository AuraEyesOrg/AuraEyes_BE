using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.ConsultationSessions.Common;
using AutoMapper;
using Domain.Repositories;

namespace Application.ConsultationSessions.Queries.GetConsultationSession;

public class GetConsultationSessionQueryHandler
    : IQueryHandler<GetConsultationSessionQuery, ConsultationSessionDto>
{
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;
    private readonly IConsultationParticipantEnrichmentService _participantEnrichmentService;

    public GetConsultationSessionQueryHandler(
        IConsultationSessionRepository sessionRepository,
        ICurrentUserService currentUser,
        IMapper mapper,
        IConsultationParticipantEnrichmentService participantEnrichmentService)
    {
        _sessionRepository = sessionRepository;
        _currentUser = currentUser;
        _mapper = mapper;
        _participantEnrichmentService = participantEnrichmentService;
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
            .Select(m => _mapper.Map<ChatMessageDto>(m))
            .ToList();

        var baseDto = _mapper.Map<ConsultationSessionDto>(session) with
        {
            Messages = messages
        };

        var dto = await _participantEnrichmentService.EnrichDetailAsync(
            baseDto,
            session,
            cancellationToken);

        return Result<ConsultationSessionDto>.Success(dto);
    }
}
