using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Enums;

namespace Domain.Repositories;

public interface IConsultationSessionRepository : IRepository<ConsultationSession>
{
    Task<ConsultationSession?> GetByIdWithConversationsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ConsultationSession?> GetByIdWithConversationsForUpdateAsync(
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
        Guid? aiScreeningId = null,
        ConsultationSessionType? type = null,
        SessionStatus? status = null,
        ChatStatus? chatStatus = null,
        Guid? participantProfileId = null,
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

    /// <summary>
    /// Returns VideoCall sessions in MemoOnly chat status whose AppointmentTime has passed,
    /// meaning they are ready to transition to the IN_PROGRESS (Open) state.
    /// </summary>
    Task<IReadOnlyList<ConsultationSession>> GetSessionsReadyToOpenAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns VideoCall sessions in Open chat status whose grace period
    /// (AppointmentTime + slotDuration + gracePeriod) has expired,
    /// meaning they should auto-archive into the COMPLETED state.
    /// </summary>
    Task<IReadOnlyList<ConsultationSession>> GetSessionsPastGracePeriodAsync(
        TimeSpan slotDuration,
        TimeSpan gracePeriod,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts how many sessions a patient cancelled today (UTC).
    /// Used to enforce the 3-cancellations-per-day anti-spam rule.
    /// </summary>
    Task<int> CountCancelledTodayByPatientAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns ClinicBooking sessions in Open chat status that have exceeded the 14-day window.
    /// </summary>
    Task<IReadOnlyList<ConsultationSession>> GetExpiredClinicSessionsAsync(
        TimeSpan threshold,
        CancellationToken cancellationToken = default);
}
