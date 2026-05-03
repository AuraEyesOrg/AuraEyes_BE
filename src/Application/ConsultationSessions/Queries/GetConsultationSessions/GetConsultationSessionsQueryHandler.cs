using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.ConsultationSessions.Common;
using AutoMapper;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.ConsultationSessions.Queries.GetConsultationSessions;

public class GetConsultationSessionsQueryHandler
    : IQueryHandler<GetConsultationSessionsQuery, PagedResult<ConsultationSessionListDto>>
{
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;
    private readonly IConsultationParticipantEnrichmentService _participantEnrichmentService;
    private readonly IRepository<AiScreening> _aiScreeningRepository;

    public GetConsultationSessionsQueryHandler(
        IConsultationSessionRepository sessionRepository,
        ICurrentUserService currentUser,
        IMapper mapper,
        IConsultationParticipantEnrichmentService participantEnrichmentService,
        IRepository<AiScreening> aiScreeningRepository)
    {
        _sessionRepository = sessionRepository;
        _currentUser = currentUser;
        _mapper = mapper;
        _participantEnrichmentService = participantEnrichmentService;
        _aiScreeningRepository = aiScreeningRepository;
    }

    public async Task<Result<PagedResult<ConsultationSessionListDto>>> Handle(
        GetConsultationSessionsQuery request,
        CancellationToken cancellationToken)
    {
        var isAdmin = _currentUser.Roles.Any(r => Roles.Admins.Contains(r));
        Guid? participantProfileId = isAdmin ? null : _currentUser.ProfileId;

        var (items, totalCount) = await _sessionRepository.GetPagedAsync(
            request.PatientId,
            request.OphthalmologistId,
            request.AiScreeningId,
            request.Type,
            request.Status,
            request.ChatStatus,
            participantProfileId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var baseDtos = _mapper.Map<List<ConsultationSessionListDto>>(items);

        // Security: admins + patients always can see; doctors only if patient shared AI results.
        baseDtos = await AttachCaseSnapshotsAsync(baseDtos, items, isAdmin, participantProfileId, cancellationToken);
        baseDtos = await AttachLatestMessagePreviewsAsync(baseDtos, items, cancellationToken);

        var dtoList = await _participantEnrichmentService.EnrichListAsync(
            baseDtos,
            items,
            cancellationToken);

        var pagedResult = new PagedResult<ConsultationSessionListDto>(
            dtoList.ToList(), totalCount, request.PageNumber, request.PageSize);

        return Result<PagedResult<ConsultationSessionListDto>>.Success(pagedResult);
    }

    private const int PreviewMaxLength = 120;

    private async Task<List<ConsultationSessionListDto>> AttachLatestMessagePreviewsAsync(
        List<ConsultationSessionListDto> dtos,
        IReadOnlyList<Domain.Entities.Consultation.ConsultationSession> sessions,
        CancellationToken cancellationToken)
    {
        if (dtos.Count == 0) return dtos;

        var sessionIds = sessions.Select(s => s.Id).ToList();

        var latestMessages = await _sessionRepository.Query().AsNoTracking()
            .Where(s => sessionIds.Contains(s.Id))
            .Select(s => new
            {
                SessionId = s.Id,
                LatestMessage = s.Conversations
                    .SelectMany(c => c.Messages)
                    .OrderByDescending(m => m.SentAt)
                    .Select(m => m.Message)
                    .FirstOrDefault()
            })
            .ToDictionaryAsync(x => x.SessionId, x => x.LatestMessage, cancellationToken);

        for (var i = 0; i < dtos.Count; i++)
        {
            if (!latestMessages.TryGetValue(dtos[i].Id, out var message) || string.IsNullOrWhiteSpace(message))
                continue;

            var preview = message.Length > PreviewMaxLength
                ? string.Concat(message.AsSpan(0, PreviewMaxLength), "…")
                : message;

            dtos[i] = dtos[i] with { LatestMessagePreview = preview };
        }

        return dtos;
    }

    private async Task<List<ConsultationSessionListDto>> AttachCaseSnapshotsAsync(
        List<ConsultationSessionListDto> dtos,
        IReadOnlyList<Domain.Entities.Consultation.ConsultationSession> sessions,
        bool isAdmin,
        Guid? currentProfileId,
        CancellationToken cancellationToken)
    {
        if (dtos.Count == 0) return dtos;

        var screeningIds = sessions
            .Where(s => s.AiScreeningId.HasValue && CanViewAiResults(s, isAdmin, currentProfileId))
            .Select(s => s.AiScreeningId!.Value)
            .Distinct()
            .ToList();

        if (screeningIds.Count == 0) return dtos;

        var screenings = await _aiScreeningRepository
            .Query().AsNoTracking()
            .Where(x => screeningIds.Contains(x.Id))
            .Include(x => x.ScreeningResults)
            .ToListAsync(cancellationToken);

        var screeningMap = screenings.ToDictionary(x => x.Id, x => x);

        for (var i = 0; i < dtos.Count; i++)
        {
            var session = sessions[i];
            if (!session.AiScreeningId.HasValue || !CanViewAiResults(session, isAdmin, currentProfileId)) continue;
            if (!screeningMap.TryGetValue(session.AiScreeningId.Value, out var screening)) continue;

            var snapshot = MapToCaseSnapshot(screening);
            if (snapshot != null)
            {
                dtos[i] = dtos[i] with { CaseSnapshot = snapshot };
            }
        }

        return dtos;
    }

    private static ConsultationCaseSnapshotDto? MapToCaseSnapshot(AiScreening screening)
    {
        var latestResult = screening.ScreeningResults
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefault();

        if (latestResult is null) return null;

        return new ConsultationCaseSnapshotDto
        {
            ScreeningId = screening.Id,
            RiskLevel = latestResult.RiskLevel.ToString(),
            ConfidenceScore = latestResult.ConfidenceScore,
            Summary = latestResult.Summary,
            Findings = latestResult.Findings,
            AnnotatedImageUrl = null,
            RawJsonOutput = null,
            OriginalImageUrls = [],
            Symptoms = []
        };
    }

    private static bool CanViewAiResults(
        Domain.Entities.Consultation.ConsultationSession session,
        bool isAdmin,
        Guid? currentProfileId)
    {
        if (isAdmin) return true;
        if (!currentProfileId.HasValue) return false;

        var profileId = currentProfileId.Value;
        if (session.PatientId == profileId) return true;

        if (session.OphthalmologistId == profileId &&
            (session.Type == Domain.Enums.ConsultationSessionType.Verification ||
             session.Type == Domain.Enums.ConsultationSessionType.ClinicBooking))
        {
            return true;
        }

        return false;
    }
}

