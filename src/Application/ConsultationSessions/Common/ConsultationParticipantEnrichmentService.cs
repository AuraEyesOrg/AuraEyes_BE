using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Users;
using Domain.Repositories;

namespace Application.ConsultationSessions.Common;

public interface IConsultationParticipantEnrichmentService
{
    Task<ConsultationSessionDto> EnrichDetailAsync(
        ConsultationSessionDto baseDto,
        ConsultationSession session,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ConsultationSessionListDto>> EnrichListAsync(
        IReadOnlyList<ConsultationSessionListDto> baseDtos,
        IReadOnlyList<ConsultationSession> sessions,
        CancellationToken cancellationToken);
}

internal sealed record ConsultationParticipantDisplayData(
    string? FullName,
    string? AvatarUrl);

public class ConsultationParticipantEnrichmentService : IConsultationParticipantEnrichmentService
{
    private readonly IRepository<Patient> _patientRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IIdentityService _identityService;

    public ConsultationParticipantEnrichmentService(
        IRepository<Patient> patientRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IIdentityService identityService)
    {
        _patientRepository = patientRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _identityService = identityService;
    }

    public async Task<ConsultationSessionDto> EnrichDetailAsync(
        ConsultationSessionDto baseDto,
        ConsultationSession session,
        CancellationToken cancellationToken)
    {
        var patientDisplayLookup = await LoadParticipantDisplayDataAsync(
            [session.PatientId],
            (profileId, ct) => _patientRepository.GetByIdAsync(profileId, ct),
            profile => profile.UserId,
            cancellationToken);

        var doctorDisplayLookup = await LoadParticipantDisplayDataAsync(
            session.OphthalmologistId.HasValue ? [session.OphthalmologistId.Value] : [],
            (profileId, ct) => _ophthalmologistRepository.GetByIdAsync(profileId, ct),
            profile => profile.UserId,
            cancellationToken);

        patientDisplayLookup.TryGetValue(session.PatientId, out var patientDisplay);

        ConsultationParticipantDisplayData? doctorDisplay = null;
        if (session.OphthalmologistId.HasValue)
        {
            doctorDisplayLookup.TryGetValue(session.OphthalmologistId.Value, out doctorDisplay);
        }

        return baseDto with
        {
            PatientName = patientDisplay?.FullName,
            PatientAvatarUrl = patientDisplay?.AvatarUrl,
            OphthalmologistName = doctorDisplay?.FullName,
            OrganisationName = null,
            OphthalmologistAvatarUrl = doctorDisplay?.AvatarUrl,
        };
    }

    public async Task<IReadOnlyList<ConsultationSessionListDto>> EnrichListAsync(
        IReadOnlyList<ConsultationSessionListDto> baseDtos,
        IReadOnlyList<ConsultationSession> sessions,
        CancellationToken cancellationToken)
    {
        var patientDisplayLookup = await LoadParticipantDisplayDataAsync(
            sessions.Select(session => session.PatientId),
            (profileId, ct) => _patientRepository.GetByIdAsync(profileId, ct),
            profile => profile.UserId,
            cancellationToken);

        var doctorDisplayLookup = await LoadParticipantDisplayDataAsync(
            sessions
                .Where(session => session.OphthalmologistId.HasValue)
                .Select(session => session.OphthalmologistId!.Value),
            (profileId, ct) => _ophthalmologistRepository.GetByIdAsync(profileId, ct),
            profile => profile.UserId,
            cancellationToken);

        return sessions.Zip(baseDtos, (session, baseDto) =>
        {
            patientDisplayLookup.TryGetValue(session.PatientId, out var patientDisplay);

            ConsultationParticipantDisplayData? doctorDisplay = null;
            if (session.OphthalmologistId.HasValue)
            {
                doctorDisplayLookup.TryGetValue(session.OphthalmologistId.Value, out doctorDisplay);
            }

            return baseDto with
            {
                PatientName = patientDisplay?.FullName,
                PatientAvatarUrl = patientDisplay?.AvatarUrl,
                OphthalmologistName = doctorDisplay?.FullName,
                OrganisationName = null,
                OphthalmologistAvatarUrl = doctorDisplay?.AvatarUrl
            };
        }).ToList();
    }

    private async Task<Dictionary<Guid, ConsultationParticipantDisplayData>> LoadParticipantDisplayDataAsync<TProfile>(
        IEnumerable<Guid> profileIds,
        Func<Guid, CancellationToken, Task<TProfile?>> getProfileAsync,
        Func<TProfile, Guid> getUserId,
        CancellationToken cancellationToken)
        where TProfile : class
    {
        var distinctProfileIds = profileIds.Distinct().ToList();
        if (distinctProfileIds.Count == 0)
        {
            return [];
        }

        var displayLookup = new Dictionary<Guid, ConsultationParticipantDisplayData>();
        foreach (var profileId in distinctProfileIds)
        {
            var profile = await getProfileAsync(profileId, cancellationToken);
            if (profile is null)
            {
                continue;
            }

            var user = await _identityService.GetUserByIdAsync(
                getUserId(profile),
                cancellationToken);

            displayLookup[profileId] = new ConsultationParticipantDisplayData(
                user?.FullName,
                user?.AvatarUrl);
        }

        return displayLookup;
    }
}