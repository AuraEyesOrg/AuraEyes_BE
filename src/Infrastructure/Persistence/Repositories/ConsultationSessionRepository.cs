using Domain.Entities.Consultation;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ConsultationSessionRepository : Repository<ConsultationSession>, IConsultationSessionRepository
{
    public ConsultationSessionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ConsultationSession?> GetByIdWithConversationsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Conversations)
            .ThenInclude(c => c.Messages)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<ConsultationSession>> GetByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.PatientId == patientId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ConsultationSession>> GetByOphthalmologistIdAsync(
        Guid ophthalmologistId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.OphthalmologistId == ophthalmologistId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<ConsultationSession> Items, int TotalCount)> GetPagedAsync(
        Guid? patientId = null,
        Guid? ophthalmologistId = null,
        ConsultationSessionType? type = null,
        SessionStatus? status = null,
        ChatStatus? chatStatus = null,
        Guid? participantProfileId = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (participantProfileId.HasValue)
        {
            var pid = participantProfileId.Value;
            query = query.Where(s => s.PatientId == pid || s.OphthalmologistId == pid);
        }

        if (patientId.HasValue)
            query = query.Where(s => s.PatientId == patientId.Value);

        if (ophthalmologistId.HasValue)
            query = query.Where(s => s.OphthalmologistId == ophthalmologistId.Value);

        if (type.HasValue)
            query = query.Where(s => s.Type == type.Value);

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        if (chatStatus.HasValue)
            query = query.Where(s => s.ChatStatus == chatStatus.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(s => s.LastActivityAt)
            .ThenByDescending(s => s.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<ConsultationSession>> GetStaleSessions(
        TimeSpan inactivityThreshold,
        TimeSpan reminderCooldown,
        CancellationToken cancellationToken = default)
    {
        var activityCutoff = DateTime.UtcNow - inactivityThreshold;
        var reminderCutoff = DateTime.UtcNow - reminderCooldown;

        return await _dbSet
            .Where(s =>
                s.Status != SessionStatus.Completed &&
                s.Status != SessionStatus.Cancelled &&
                s.ChatStatus == ChatStatus.Open &&
                s.LastActivityAt < activityCutoff &&
                (s.LastReminderSentAt == null || s.LastReminderSentAt < reminderCutoff))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ConsultationSession>> GetSessionsReadyToOpenAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await _dbSet
            .Where(s =>
                s.Type == ConsultationSessionType.VideoCall &&
                s.ChatStatus == ChatStatus.MemoOnly &&
                s.Status != SessionStatus.Cancelled &&
                s.AppointmentTime != null &&
                s.AppointmentTime <= now)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ConsultationSession>> GetSessionsPastGracePeriodAsync(
        TimeSpan slotDuration,
        TimeSpan gracePeriod,
        CancellationToken cancellationToken = default)
    {
        var cutoff = DateTime.UtcNow - slotDuration - gracePeriod;

        return await _dbSet
            .Where(s =>
                s.Type == ConsultationSessionType.VideoCall &&
                s.ChatStatus == ChatStatus.Open &&
                s.Status != SessionStatus.Completed &&
                s.Status != SessionStatus.Cancelled &&
                s.AppointmentTime != null &&
                s.AppointmentTime <= cutoff)
            .ToListAsync(cancellationToken);
    }
}
