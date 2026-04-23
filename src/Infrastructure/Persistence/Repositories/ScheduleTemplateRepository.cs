using Domain.Entities.Scheduling;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for ScheduleTemplate aggregate root.
/// Clinic-centric model.
/// </summary>
public class ScheduleTemplateRepository : Repository<ScheduleTemplate>, IScheduleTemplateRepository
{
    public ScheduleTemplateRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ScheduleTemplate>> GetByDayOfWeekAsync(
        DayOfWeek dayOfWeek,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.DayOfWeek == dayOfWeek && t.IsActive)
            .OrderBy(t => t.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasOverlappingTemplateAsync(
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        Guid? excludeTemplateId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(t => t.DayOfWeek == dayOfWeek && t.IsActive);

        if (excludeTemplateId.HasValue)
            query = query.Where(t => t.Id != excludeTemplateId.Value);

        return await query.AnyAsync(t =>
            startTime < t.EndTime && endTime > t.StartTime,
            cancellationToken);
    }

    public async Task<(IReadOnlyList<ScheduleTemplate> Items, int TotalCount)> GetPagedAsync(
        DayOfWeek? dayOfWeek = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(t => t.IsActive);

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

    public async Task<IReadOnlyList<ScheduleTemplate>> GetActiveTemplatesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.IsActive)
            .OrderBy(t => t.DayOfWeek)
            .ThenBy(t => t.StartTime)
            .ToListAsync(cancellationToken);
    }
}
