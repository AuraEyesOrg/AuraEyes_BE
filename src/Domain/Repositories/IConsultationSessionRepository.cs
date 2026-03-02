using Domain.Common;
using Domain.Entities;
using Domain.Enums;

namespace Domain.Repositories;

public interface IConsultationSessionRepository : IRepository<ConsultationSession>
{
    Task<ConsultationSession?> GetByIdWithConversationsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ConsultationSession>> GetByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ConsultationSession>> GetByOphthalmologistIdAsync(
        Guid ophthalmologistId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<ConsultationSession> Items, int TotalCount)> GetPagedAsync(
        Guid? patientId = null,
        Guid? ophthalmologistId = null,
        ConsultationSessionType? type = null,
        SessionStatus? status = null,
        ChatStatus? chatStatus = null,
        Guid? participantUserId = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns active sessions with open chat that have been inactive for the given threshold
    /// and have not received a reminder within the specified cooldown period.
    /// </summary>
    Task<IReadOnlyList<ConsultationSession>> GetStaleSessions(
        TimeSpan inactivityThreshold,
        TimeSpan reminderCooldown,
        CancellationToken cancellationToken = default);

    Task<bool> IsUserParticipantAsync(
        Guid sessionId,
        Guid applicationUserId,
        CancellationToken cancellationToken = default);
}
