using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for AvailableSlot aggregate root.
/// </summary>
public class AvailableSlotRepository : Repository<AvailableSlot>, IAvailableSlotRepository
{
    public AvailableSlotRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<AvailableSlot>> GetByOphthalmologistIdAsync(
        Guid ophthalmologistId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.OphthalmologistId == ophthalmologistId)
            .OrderBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AvailableSlot>> GetByOrganisationIdAsync(
        Guid organisationId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.OrganisationId == organisationId)
            .OrderBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AvailableSlot>> GetByDateRangeAsync(
        Guid? ophthalmologistId,
        Guid? organisationId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (ophthalmologistId.HasValue)
            query = query.Where(s => s.OphthalmologistId == ophthalmologistId.Value);

        if (organisationId.HasValue)
            query = query.Where(s => s.OrganisationId == organisationId.Value);

        return await query
            .Where(s => s.StartTime >= fromDate && s.EndTime <= toDate)
            .OrderBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasOverlappingSlotAsync(
        Guid? ophthalmologistId,
        Guid? organisationId,
        DateTime startTime,
        DateTime endTime,
        Guid? excludeSlotId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        // Match on ophthalmologist or organisation
        if (ophthalmologistId.HasValue)
            query = query.Where(s => s.OphthalmologistId == ophthalmologistId.Value);

        if (organisationId.HasValue)
            query = query.Where(s => s.OrganisationId == organisationId.Value);

        if (excludeSlotId.HasValue)
            query = query.Where(s => s.Id != excludeSlotId.Value);

        // Check for overlap: existing.Start < newEnd AND existing.End > newStart
        return await query.AnyAsync(s =>
            startTime < s.EndTime && endTime > s.StartTime,
            cancellationToken);
    }

    public async Task<(IReadOnlyList<AvailableSlot> Items, int TotalCount)> GetPagedAsync(
        Guid? ophthalmologistId,
        Guid? organisationId,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (ophthalmologistId.HasValue)
            query = query.Where(s => s.OphthalmologistId == ophthalmologistId.Value);

        if (organisationId.HasValue)
            query = query.Where(s => s.OrganisationId == organisationId.Value);

        if (fromDate.HasValue)
            query = query.Where(s => s.StartTime >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(s => s.EndTime <= toDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(s => s.StartTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<AvailableSlot?> GetByIdWithSchedulesAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Schedules)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<AvailableSlot>> GetSlotsWithAvailableCapacityAsync(
        Guid? ophthalmologistId,
        Guid? organisationId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(s => s.Schedules)
            .AsQueryable();

        if (ophthalmologistId.HasValue)
            query = query.Where(s => s.OphthalmologistId == ophthalmologistId.Value);

        if (organisationId.HasValue)
            query = query.Where(s => s.OrganisationId == organisationId.Value);

        return await query
            .Where(s => s.StartTime >= fromDate && s.EndTime <= toDate)
            .Where(s => s.Schedules.Count(sch => sch.Status != ScheduleStatus.Cancelled) < s.MaxCapacity)
            .OrderBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }
}
