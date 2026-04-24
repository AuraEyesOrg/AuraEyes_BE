using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for AppointmentSlot aggregate root.
/// Clinic-centric model.
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
            .Where(s => s.Status == ScheduleStatus.Available)
            .Where(s => s.BookedCount < s.MaxCapacity);

        if (scheduleTemplateId.HasValue)
            query = query.Where(s => s.ScheduleTemplateId == scheduleTemplateId.Value);

        if (fromDate.HasValue)
            query = query.Where(s => s.Date >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(s => s.Date <= toDate.Value);

        return await query
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
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
        bool excludePastSlots = false,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(s => s.ScheduleTemplate)
            .AsQueryable();

        if (scheduleTemplateId.HasValue)
            query = query.Where(s => s.ScheduleTemplateId == scheduleTemplateId.Value);

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        if (fromDate.HasValue)
            query = query.Where(s => s.Date >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(s => s.Date <= toDate.Value);

        if (excludePastSlots)
        {
            var vietnamNow = GetVietnamNow();
            var vietnamToday = DateOnly.FromDateTime(vietnamNow);
            var vietnamTime = TimeOnly.FromDateTime(vietnamNow);

            query = query.Where(s =>
                s.Date > vietnamToday ||
                (s.Date == vietnamToday && s.StartTime > vietnamTime));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
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
            .Where(s => s.Status != ScheduleStatus.Blocked);

        if (excludeSlotId.HasValue)
            query = query.Where(s => s.Id != excludeSlotId.Value);

        return await query.AnyAsync(s =>
            startTime < s.EndTime && endTime > s.StartTime,
            cancellationToken);
    }

    public async Task<AppointmentSlot?> GetByIdWithLockAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var slot = await _dbSet
            .FromSqlRaw("SELECT * FROM \"AppointmentSlots\" WHERE \"Id\" = {0} FOR UPDATE", id)
            .FirstOrDefaultAsync(cancellationToken);

        if (slot != null)
        {
            await _context.Entry(slot)
                .Reference(s => s.ScheduleTemplate)
                .LoadAsync(cancellationToken);
        }

        return slot;
    }

    public async Task<Dictionary<ScheduleStatus, int>> GetStatusCountsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .GroupBy(s => s.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);
    }

    public async Task<IReadOnlyList<AppointmentSlot>> GetByOrganisationAndDateRangeAsync(
        Guid organisationId,
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(s => s.ScheduleTemplate)
            .Where(s => s.Date >= fromDate && s.Date <= toDate);

        if (organisationId != Guid.Empty)
        {
            query = query.Where(s => s.ScheduleTemplate.OrgId == organisationId);
        }

        return await query
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    private static DateTime GetVietnamNow()
    {
        var utcNow = DateTime.UtcNow;
        foreach (var timeZoneId in new[] { "SE Asia Standard Time", "Asia/Ho_Chi_Minh" })
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                return TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone);
            }
            catch (TimeZoneNotFoundException) { }
            catch (InvalidTimeZoneException) { }
        }

        return utcNow + TimeSpan.FromHours(7);
    }
}
