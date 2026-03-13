using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for AppointmentSlot aggregate root.
/// </summary>
public class AppointmentSlotRepository : Repository<AppointmentSlot>, IAppointmentSlotRepository
{
    public AppointmentSlotRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<AppointmentSlot>> GetByTemplateIdAsync(
        Guid scheduleTemplateId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.ScheduleTemplateId == scheduleTemplateId)
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AppointmentSlot>> GetByDateRangeAsync(
        Guid scheduleTemplateId,
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.ScheduleTemplateId == scheduleTemplateId)
            .Where(s => s.Date >= fromDate && s.Date <= toDate)
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AppointmentSlot>> GetAvailableSlotsAsync(
        Guid? scheduleTemplateId,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(s => s.ScheduleTemplate)
            .Where(s => s.Status == ScheduleStatus.Available);

        if (scheduleTemplateId.HasValue)
            query = query.Where(s => s.ScheduleTemplateId == scheduleTemplateId.Value);

        if (fromDate.HasValue)
            query = query.Where(s => s.Date >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(s => s.Date <= toDate.Value);

        // Filter slots with available capacity
        var slots = await query
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);

        return slots
            .Where(s => s.ScheduleTemplate != null && s.BookedCount < s.ScheduleTemplate.MaxCapacity)
            .ToList();
    }

    public async Task<AppointmentSlot?> GetByIdWithTemplateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.ScheduleTemplate)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<AppointmentSlot> Items, int TotalCount)> GetPagedAsync(
        Guid? scheduleTemplateId,
        ScheduleStatus? status = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (scheduleTemplateId.HasValue)
            query = query.Where(s => s.ScheduleTemplateId == scheduleTemplateId.Value);

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

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

    public async Task<IReadOnlyList<AppointmentSlot>> GetByOphthalmologistAsync(
        Guid ophthalId,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        ScheduleStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(s => s.ScheduleTemplate)
            .Where(s => s.ScheduleTemplate != null && s.ScheduleTemplate.OphthalId == ophthalId);

        if (fromDate.HasValue)
            query = query.Where(s => s.Date >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(s => s.Date <= toDate.Value);

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        return await query
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AppointmentSlot>> GetByOrganisationAsync(
        Guid orgId,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        ScheduleStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(s => s.ScheduleTemplate)
            .Where(s => s.ScheduleTemplate != null && s.ScheduleTemplate.OrgId == orgId);

        if (fromDate.HasValue)
            query = query.Where(s => s.Date >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(s => s.Date <= toDate.Value);

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        return await query
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<ScheduleStatus, int>> GetStatusCountsAsync(
        Guid? ophthalId,
        Guid? orgId,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Include(s => s.ScheduleTemplate).AsQueryable();

        if (ophthalId.HasValue)
            query = query.Where(s => s.ScheduleTemplate != null && s.ScheduleTemplate.OphthalId == ophthalId.Value);

        if (orgId.HasValue)
            query = query.Where(s => s.ScheduleTemplate != null && s.ScheduleTemplate.OrgId == orgId.Value);

        return await query
            .GroupBy(s => s.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);
    }

    public async Task<bool> HasOverlappingSlotAsync(
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        Guid? excludeSlotId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(s => s.Date == date)
            .Where(s => s.Status != ScheduleStatus.Cancelled);

        if (excludeSlotId.HasValue)
            query = query.Where(s => s.Id != excludeSlotId.Value);

        return await query.AnyAsync(s =>
            startTime < s.EndTime && endTime > s.StartTime,
            cancellationToken);
    }

    public async Task<IReadOnlyList<AppointmentSlot>> GetExpiredReservationsAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(s => s.Status == ScheduleStatus.Reserved)
            .Where(s => s.ReservationExpireAt.HasValue && s.ReservationExpireAt.Value < now)
            .ToListAsync(cancellationToken);
    }

    public async Task<AppointmentSlot?> GetByIdWithLockAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        // Use raw SQL for row-level locking in PostgreSQL
        // Note: Template will be loaded separately if needed
        var slot = await _dbSet
            .FromSqlRaw("SELECT * FROM \"AppointmentSlots\" WHERE \"Id\" = {0} FOR UPDATE", id)
            .FirstOrDefaultAsync(cancellationToken);

        if (slot != null)
        {
            // Load the template separately
            await _context.Entry(slot)
                .Reference(s => s.ScheduleTemplate)
                .LoadAsync(cancellationToken);
        }

        return slot;
    }

    public async Task<IReadOnlyList<AppointmentSlot>> GetAvailableByOrganisationWithCapacityAsync(
        Guid organisationId,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(s => s.ScheduleTemplate)
            .Where(s => s.ScheduleTemplate != null && s.ScheduleTemplate.OrgId == organisationId)
            .Where(s => s.Status == ScheduleStatus.Available)
            .Where(s => s.BookedCount < s.MaxCapacity);

        if (fromDate.HasValue)
            query = query.Where(s => s.Date >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(s => s.Date <= toDate.Value);

        return await query
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }
}
