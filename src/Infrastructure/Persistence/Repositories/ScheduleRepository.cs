using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Schedule aggregate root.
/// </summary>
public class ScheduleRepository : Repository<Schedule>, IScheduleRepository
{
    public ScheduleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Schedule>> GetByOphthalmologistIdAsync(
        Guid ophthalmologistId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.AvailableSlot)
            .Where(s => s.AvailableSlot != null && s.AvailableSlot.OphthalmologistId == ophthalmologistId)
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Schedule>> GetAvailableByOphthalmologistAsync(
        Guid ophthalmologistId,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(s => s.AvailableSlot)
            .Where(s => s.AvailableSlot != null && s.AvailableSlot.OphthalmologistId == ophthalmologistId)
            .Where(s => s.Status == ScheduleStatus.Available);

        if (fromDate.HasValue)
            query = query.Where(s => s.Date >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(s => s.Date <= toDate.Value);

        return await query
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Schedule>> GetByDateAsync(
        Guid ophthalmologistId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.AvailableSlot)
            .Where(s => s.AvailableSlot != null && s.AvailableSlot.OphthalmologistId == ophthalmologistId && s.Date == date)
            .OrderBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasOverlappingScheduleAsync(
        Guid ophthalmologistId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        Guid? excludeScheduleId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(s => s.AvailableSlot)
            .Where(s => s.AvailableSlot != null && s.AvailableSlot.OphthalmologistId == ophthalmologistId)
            .Where(s => s.Date == date)
            .Where(s => s.Status != ScheduleStatus.Cancelled);

        if (excludeScheduleId.HasValue)
            query = query.Where(s => s.Id != excludeScheduleId.Value);

        return await query.AnyAsync(s =>
            startTime < s.EndTime && endTime > s.StartTime,
            cancellationToken);
    }

    public async Task<(IReadOnlyList<Schedule> Items, int TotalCount)> GetPagedAsync(
        Guid ophthalmologistId,
        ScheduleStatus? status = null,
        SlotType? slotType = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(s => s.AvailableSlot)
            .Where(s => s.AvailableSlot != null && s.AvailableSlot.OphthalmologistId == ophthalmologistId);

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        if (slotType.HasValue)
            query = query.Where(s => s.SlotType == slotType.Value);

        if (fromDate.HasValue)
            query = query.Where(s => s.Date >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(s => s.Date <= toDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<int> GetActiveCountByAvailableSlotAsync(
        Guid availableSlotId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.AvailableSlotId == availableSlotId &&
                        s.Status != ScheduleStatus.Cancelled)
            .CountAsync(cancellationToken);
    }

    public async Task<Schedule?> GetByIdWithSlotAsync(
        Guid scheduleId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.AvailableSlot)
            .FirstOrDefaultAsync(s => s.Id == scheduleId, cancellationToken);
    }

    public async Task<Dictionary<ScheduleStatus, int>> GetStatusCountsAsync(
        Guid ophthalmologistId,
        CancellationToken cancellationToken = default)
    {
        var counts = await _dbSet
            .Include(s => s.AvailableSlot)
            .Where(s => s.AvailableSlot != null && s.AvailableSlot.OphthalmologistId == ophthalmologistId)
            .GroupBy(s => s.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return counts.ToDictionary(x => x.Status, x => x.Count);
    }
}
