using Domain.Entities;
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

    public async Task<IReadOnlyList<ConsultationSession>> GetStaleSessions(
        TimeSpan inactivityThreshold,
        CancellationToken cancellationToken = default)
    {
        var cutoff = DateTime.UtcNow - inactivityThreshold;

        return await _dbSet
            .Where(s =>
                s.Status != SessionStatus.Completed &&
                s.Status != SessionStatus.Cancelled &&
                s.ChatStatus == ChatStatus.Open &&
                s.LastActivityAt < cutoff)
            .ToListAsync(cancellationToken);
    }
}
