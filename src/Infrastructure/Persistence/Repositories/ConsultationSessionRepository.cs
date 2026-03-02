using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ConsultationSessionRepository : Repository<ConsultationSession>, IConsultationSessionRepository
{
    private readonly DbSet<Patient> _patients;
    private readonly DbSet<Ophthalmologist> _ophthalmologists;

    public ConsultationSessionRepository(ApplicationDbContext context) : base(context)
    {
        _patients = context.Set<Patient>();
        _ophthalmologists = context.Set<Ophthalmologist>();
    }

    public async Task<ConsultationSession?> GetByIdWithConversationsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Conversations)
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
        Guid? participantUserId = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (participantUserId.HasValue)
        {
            var uid = participantUserId.Value;
            // participantUserId is the ApplicationUser.Id (from JWT).
            // ConsultationSession stores entity IDs (Patient.Id / Ophthalmologist.Id),
            // so we must join through the entity tables to match on UserId.
            query = query.Where(s =>
                _patients.Any(p => p.Id == s.PatientId && p.UserId == uid) ||
                _ophthalmologists.Any(o => o.Id == s.OphthalmologistId && o.UserId == uid));
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
            .OrderByDescending(s => s.CreatedAt)
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

    public async Task<bool> IsUserParticipantAsync(
        Guid sessionId,
        Guid applicationUserId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(s =>
            s.Id == sessionId &&
            (
                _patients.Any(p => p.Id == s.PatientId && p.UserId == applicationUserId) ||
                _ophthalmologists.Any(o => o.Id == s.OphthalmologistId && o.UserId == applicationUserId)
            ),
            cancellationToken);
    }
}
