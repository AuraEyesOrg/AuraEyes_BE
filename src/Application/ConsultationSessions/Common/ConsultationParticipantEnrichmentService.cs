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
        var patientDisplayLookup = await LoadPatientDisplayDataAsync(
            [session.PatientId],
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
        var patientDisplayLookup = await LoadPatientDisplayDataAsync(
            sessions.Select(session => session.PatientId),
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

    /// <summary>
    /// Patient-specific loader that handles both walk-in (UserId=null, name on entity)
    /// and registered (UserId != null, name from Identity) patients.
    /// </summary>
    private async Task<Dictionary<Guid, ConsultationParticipantDisplayData>> LoadPatientDisplayDataAsync(
        IEnumerable<Guid> patientProfileIds,
        CancellationToken cancellationToken)
    {
        var distinctIds = patientProfileIds.Distinct().ToList();
        if (distinctIds.Count == 0)
        {
            return [];
        }

        var displayLookup = new Dictionary<Guid, ConsultationParticipantDisplayData>();
        foreach (var profileId in distinctIds)
        {
            var patient = await _patientRepository.GetByIdAsync(profileId, cancellationToken);
            if (patient is null)
            {
                continue;
            }

            if (patient.IsWalkIn)
            {
                displayLookup[profileId] = new ConsultationParticipantDisplayData(
                    patient.FullName,
                    null);
            }
            else if (patient.UserId.HasValue)
            {
                var user = await _identityService.GetUserByIdAsync(
                    patient.UserId.Value,
                    cancellationToken);

                displayLookup[profileId] = new ConsultationParticipantDisplayData(
                    user?.FullName,
                    user?.AvatarUrl);
            }
        }

        return displayLookup;
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