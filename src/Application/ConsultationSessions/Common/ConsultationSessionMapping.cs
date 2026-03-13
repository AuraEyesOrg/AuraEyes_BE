using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Users;
using Domain.Repositories;

namespace Application.ConsultationSessions.Common;

internal sealed record ConsultationParticipantDisplayData(
    string? FullName,
    string? AvatarUrl);

internal static class ConsultationSessionMapping
{
    public static async Task<ConsultationSessionDto> ToDetailDtoAsync(
        ConsultationSession session,
        IRepository<Patient> patientRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IIdentityService identityService,
        IReadOnlyList<ChatMessageDto> messages,
        CancellationToken cancellationToken)
    {
        var patientDisplayLookup = await LoadParticipantDisplayDataAsync(
            [session.PatientId],
            (profileId, ct) => patientRepository.GetByIdAsync(profileId, ct),
            profile => profile.UserId,
            identityService,
            cancellationToken);

        var doctorDisplayLookup = await LoadParticipantDisplayDataAsync(
            session.OphthalmologistId.HasValue ? [session.OphthalmologistId.Value] : [],
            (profileId, ct) => ophthalmologistRepository.GetByIdAsync(profileId, ct),
            profile => profile.UserId,
            identityService,
            cancellationToken);

        patientDisplayLookup.TryGetValue(session.PatientId, out var patientDisplay);

        ConsultationParticipantDisplayData? doctorDisplay = null;
        if (session.OphthalmologistId.HasValue)
        {
            doctorDisplayLookup.TryGetValue(session.OphthalmologistId.Value, out doctorDisplay);
        }

        return new ConsultationSessionDto
        {
            Id = session.Id,
            PatientId = session.PatientId,
            OphthalmologistId = session.OphthalmologistId,
            OrganisationId = session.OrganisationId,
            AiScreeningId = session.AiScreeningId,
            PatientName = patientDisplay?.FullName,
            OphthalmologistName = doctorDisplay?.FullName,
            OrganisationName = null,
            OphthalmologistAvatarUrl = doctorDisplay?.AvatarUrl,
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
    }

    public static async Task<IReadOnlyList<ConsultationSessionListDto>> ToListDtosAsync(
        IReadOnlyList<ConsultationSession> sessions,
        IRepository<Patient> patientRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IIdentityService identityService,
        CancellationToken cancellationToken)
    {
        var patientDisplayLookup = await LoadParticipantDisplayDataAsync(
            sessions.Select(session => session.PatientId),
            (profileId, ct) => patientRepository.GetByIdAsync(profileId, ct),
            profile => profile.UserId,
            identityService,
            cancellationToken);

        var doctorDisplayLookup = await LoadParticipantDisplayDataAsync(
            sessions
                .Where(session => session.OphthalmologistId.HasValue)
                .Select(session => session.OphthalmologistId!.Value),
            (profileId, ct) => ophthalmologistRepository.GetByIdAsync(profileId, ct),
            profile => profile.UserId,
            identityService,
            cancellationToken);

        return sessions.Select(session =>
        {
            patientDisplayLookup.TryGetValue(session.PatientId, out var patientDisplay);

            ConsultationParticipantDisplayData? doctorDisplay = null;
            if (session.OphthalmologistId.HasValue)
            {
                doctorDisplayLookup.TryGetValue(session.OphthalmologistId.Value, out doctorDisplay);
            }

            return new ConsultationSessionListDto
            {
                Id = session.Id,
                PatientId = session.PatientId,
                OphthalmologistId = session.OphthalmologistId,
                PatientName = patientDisplay?.FullName,
                OphthalmologistName = doctorDisplay?.FullName,
                OrganisationName = null,
                OphthalmologistAvatarUrl = doctorDisplay?.AvatarUrl,
                Type = session.Type,
                Status = session.Status,
                ChatStatus = session.ChatStatus,
                Price = session.Price,
                AppointmentTime = session.AppointmentTime,
                MeetingLink = session.MeetingLink,
                LastActivityAt = session.LastActivityAt,
                CreatedAt = session.CreatedAt
            };
        }).ToList();
    }

    private static async Task<Dictionary<Guid, ConsultationParticipantDisplayData>> LoadParticipantDisplayDataAsync<TProfile>(
        IEnumerable<Guid> profileIds,
        Func<Guid, CancellationToken, Task<TProfile?>> getProfileAsync,
        Func<TProfile, Guid> getUserId,
        IIdentityService identityService,
        CancellationToken cancellationToken)
        where TProfile : class
    {
        var distinctProfileIds = profileIds.Distinct().ToList();
        if (distinctProfileIds.Count == 0)
        {
            return [];
        }

        var profileTasks = distinctProfileIds.ToDictionary(
            profileId => profileId,
            profileId => getProfileAsync(profileId, cancellationToken));

        await Task.WhenAll(profileTasks.Values);

        var userTasks = new Dictionary<Guid, Task<UserDto?>>();

        foreach (var profileId in distinctProfileIds)
        {
            var profile = await profileTasks[profileId];
            if (profile is null)
            {
                continue;
            }

            userTasks[profileId] = identityService.GetUserByIdAsync(
                getUserId(profile),
                cancellationToken);
        }

        await Task.WhenAll(userTasks.Values);

        var displayLookup = new Dictionary<Guid, ConsultationParticipantDisplayData>();

        foreach (var profileId in userTasks.Keys)
        {
            var user = await userTasks[profileId];
            displayLookup[profileId] = new ConsultationParticipantDisplayData(
                user?.FullName,
                user?.AvatarUrl);
        }

        return displayLookup;
    }
}