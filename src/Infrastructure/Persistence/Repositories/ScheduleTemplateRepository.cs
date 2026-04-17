using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for ScheduleTemplate aggregate root.
/// </summary>
public class ScheduleTemplateRepository : Repository<ScheduleTemplate>, IScheduleTemplateRepository
{
    public ScheduleTemplateRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ScheduleTemplate>> GetByOphthalmologistIdAsync(
        Guid ophthalId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.OphthalId == ophthalId && t.IsActive)
            .OrderBy(t => t.DayOfWeek)
            .ThenBy(t => t.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ScheduleTemplate>> GetByOrganisationIdAsync(
        Guid orgId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.OrgId == orgId && t.IsActive)
            .OrderBy(t => t.DayOfWeek)
            .ThenBy(t => t.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ScheduleTemplate>> GetByDayOfWeekAsync(
        Guid? ophthalId,
        Guid? orgId,
        DayOfWeek dayOfWeek,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(t => t.DayOfWeek == dayOfWeek && t.IsActive);

        if (ophthalId.HasValue)
            query = query.Where(t => t.OphthalId == ophthalId.Value);

        if (orgId.HasValue)
            query = query.Where(t => t.OrgId == orgId.Value);

        return await query
            .OrderBy(t => t.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasOverlappingTemplateAsync(
        Guid? ophthalId,
        Guid? orgId,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        Guid? excludeTemplateId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(t => t.DayOfWeek == dayOfWeek && t.IsActive);

        if (ophthalId.HasValue)
            query = query.Where(t => t.OphthalId == ophthalId.Value);

        if (orgId.HasValue)
            query = query.Where(t => t.OrgId == orgId.Value);

        if (excludeTemplateId.HasValue)
            query = query.Where(t => t.Id != excludeTemplateId.Value);

        // Check for overlap: existing.Start < newEnd AND existing.End > newStart
        return await query.AnyAsync(t =>
            startTime < t.EndTime && endTime > t.StartTime,
            cancellationToken);
    }

    public async Task<(IReadOnlyList<ScheduleTemplate> Items, int TotalCount)> GetPagedAsync(
        Guid? ophthalId,
        Guid? orgId,
        DayOfWeek? dayOfWeek = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(t => t.IsActive);

        if (ophthalId.HasValue)
            query = query.Where(t => t.OphthalId == ophthalId.Value);

        if (orgId.HasValue)
            query = query.Where(t => t.OrgId == orgId.Value);

        if (dayOfWeek.HasValue)
            query = query.Where(t => t.DayOfWeek == dayOfWeek.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(t => t.DayOfWeek)
            .ThenBy(t => t.StartTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<ScheduleTemplate?> GetByIdWithSlotsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.AppointmentSlots)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<ScheduleTemplate>> GetActiveSystemGeneratedByOphthalmologistIdAsync(
        Guid ophthalId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.OphthalId == ophthalId
                && t.IsActive
                && t.Source == ScheduleTemplateSource.SystemGenerated)
            .OrderBy(t => t.DayOfWeek)
            .ThenBy(t => t.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsActiveSystemGeneratedTemplateAsync(
        Guid ophthalId,
        DayOfWeek dayOfWeek,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(t =>
            t.OphthalId == ophthalId
            && t.DayOfWeek == dayOfWeek
            && t.IsActive
            && t.Source == ScheduleTemplateSource.SystemGenerated,
            cancellationToken);
    }
}
