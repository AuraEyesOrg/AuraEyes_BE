using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.ConsultationSessions.Common;
using AutoMapper;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Application.ConsultationSessions.Queries.GetConsultationSession;

public class GetConsultationSessionQueryHandler
    : IQueryHandler<GetConsultationSessionQuery, ConsultationSessionDto>
{
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;
    private readonly IConsultationParticipantEnrichmentService _participantEnrichmentService;
    private readonly IRepository<AiScreening> _aiScreeningRepository;

    public GetConsultationSessionQueryHandler(
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
        var isPatient = _currentUser.ProfileId.HasValue
                        && session.PatientId == _currentUser.ProfileId.Value;

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

        var canViewRetinalImages = isAdmin || isPatient || session.IsRetinalImagesShared;
        var canViewAiResults = isAdmin || isPatient || session.IsAIResultShared;
        var isAssignedDoctorOnInternalSession = _currentUser.ProfileId.HasValue
            && session.OphthalmologistId == _currentUser.ProfileId.Value
            && (session.Type == Domain.Enums.ConsultationSessionType.Verification
                || session.Type == Domain.Enums.ConsultationSessionType.ClinicBooking);

        if (isAssignedDoctorOnInternalSession)
        {
            canViewRetinalImages = true;
            canViewAiResults = true;
        }

        var caseSnapshot = await BuildCaseSnapshotAsync(
            session.AiScreeningId,
            canViewRetinalImages,
            canViewAiResults,
            cancellationToken);

        dto = dto with
        {
            IsRetinalImagesShared = session.IsRetinalImagesShared,
            IsAIResultShared = session.IsAIResultShared,
            CaseSnapshot = caseSnapshot
        };

        return Result<ConsultationSessionDto>.Success(dto);
    }

    private async Task<ConsultationCaseSnapshotDto?> BuildCaseSnapshotAsync(
        Guid? screeningId,
        bool canViewRetinalImages,
        bool canViewAiResults,
        CancellationToken cancellationToken)
    {
        if (!screeningId.HasValue || (!canViewRetinalImages && !canViewAiResults)) return null;

        var screening = await _aiScreeningRepository
            .Query()
            .Include(x => x.RetinalImages)
            .Include(x => x.ScreeningResults)
            .FirstOrDefaultAsync(x => x.Id == screeningId.Value, cancellationToken);

        if (screening is null) return null;

        ScreeningResult? latestResult = null;
        if (canViewAiResults)
        {
            latestResult = screening.ScreeningResults
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault();
        }

        var symptoms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (canViewAiResults && !string.IsNullOrWhiteSpace(latestResult?.Findings))
        {
            foreach (var item in latestResult.Findings
                         .Split([',', ';', '\n', '\r'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            {
                symptoms.Add(item);
            }
        }

        string? annotatedImageUrl = null;
        if (canViewAiResults && !string.IsNullOrWhiteSpace(screening.RawJsonOutput))
        {
            try
            {
                using var doc = JsonDocument.Parse(screening.RawJsonOutput);
                var root = doc.RootElement;

                if (root.TryGetProperty("annotatedImageUrl", out var annotated))
                {
                    annotatedImageUrl = annotated.GetString();
                }
                else if (root.TryGetProperty("image_url", out var imageUrl))
                {
                    annotatedImageUrl = imageUrl.GetString();
                }

                if (root.TryGetProperty("detections", out var detections) &&
                    detections.ValueKind == JsonValueKind.Array)
                {
                    foreach (var det in detections.EnumerateArray())
                    {
                        if (det.TryGetProperty("class_name", out var className))
                        {
                            var value = className.GetString();
                            if (!string.IsNullOrWhiteSpace(value)) symptoms.Add(value);
                        }
                    }
                }

                if (root.TryGetProperty("prediction", out var prediction) &&
                    prediction.TryGetProperty("top_k", out var topK) &&
                    topK.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in topK.EnumerateArray())
                    {
                        if (item.TryGetProperty("class_name", out var classNameEl))
                        {
                            var value = classNameEl.GetString();
                            if (!string.IsNullOrWhiteSpace(value)) symptoms.Add(value);
                        }
                    }
                }
            }
            catch
            {
                // Keep snapshot resilient even if raw output format changes.
            }
        }

        return new ConsultationCaseSnapshotDto
        {
            ScreeningId = screening.Id,
            RiskLevel = canViewAiResults ? latestResult?.RiskLevel.ToString() : null,
            ConfidenceScore = canViewAiResults ? latestResult?.ConfidenceScore : null,
            Summary = canViewAiResults ? latestResult?.Summary : null,
            Findings = canViewAiResults ? latestResult?.Findings : null,
            AnnotatedImageUrl = canViewAiResults ? annotatedImageUrl : null,
            RawJsonOutput = canViewAiResults ? screening.RawJsonOutput : null,
            OriginalImageUrls = canViewRetinalImages
                ? screening.RetinalImages
                    .OrderBy(x => x.CreatedAt)
                    .Select(x => x.ImageUrl)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList()
                : [],
            Symptoms = canViewAiResults ? symptoms.ToList() : []
        };
    }
}
