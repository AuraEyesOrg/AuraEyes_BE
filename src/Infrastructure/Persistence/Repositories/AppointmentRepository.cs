using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Appointment aggregate root.
/// Clinic-centric booking-only model.
/// </summary>
public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    private static readonly string[] VietnamTimeZoneIds =
    [
        "SE Asia Standard Time",
        "Asia/Ho_Chi_Minh"
    ];

    public AppointmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Appointment>> GetBySlotAsync(
        Guid slotId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Where(a => a.AppointmentSlotId == slotId)
            .Where(a => a.Status != AppointmentStatus.Cancelled && a.Status != AppointmentStatus.NoShow)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Appointment?> GetByIdWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Include(a => a.AppointmentSlot)
                .ThenInclude(s => s!.ScheduleTemplate)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<bool> HasExistingAppointmentAsync(
        Guid patientId,
        Guid slotId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(a =>
                a.PatientId == patientId &&
                a.AppointmentSlotId == slotId &&
                a.Status != AppointmentStatus.Cancelled &&
                a.Status != AppointmentStatus.NoShow,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Appointment>> GetByPatientAsync(
        Guid patientId,
        AppointmentStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(a => a.AppointmentSlot)
            .Where(a => a.PatientId == patientId);

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        return await query
            .OrderByDescending(a => a.AppointmentSlot!.Date)
            .ThenBy(a => a.AppointmentSlot!.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Appointment> Items, int TotalCount)> GetPagedAsync(
        Guid? patientId = null,
        AppointmentStatus? status = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(a => a.Patient)
            .Include(a => a.AppointmentSlot)
            .AsQueryable();

        if (patientId.HasValue)
            query = query.Where(a => a.PatientId == patientId.Value);

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        if (fromDate.HasValue)
            query = query.Where(a => a.AppointmentSlot != null && a.AppointmentSlot.Date >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(a => a.AppointmentSlot != null && a.AppointmentSlot.Date <= toDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.AppointmentSlot!.Date)
            .ThenBy(a => a.AppointmentSlot!.StartTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(IReadOnlyList<Appointment> Items, int TotalCount)> GetPagedByPatientAsync(
        Guid patientId,
        IReadOnlyCollection<AppointmentStatus>? statuses,
        int pageNumber,
        int pageSize,
        bool upcomingOnly = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(a => a.AppointmentSlot)
            .Where(a => a.PatientId == patientId && a.AppointmentSlot != null);

        if (statuses is { Count: > 0 })
        {
            var statusArray = statuses.Distinct().ToArray();
            query = query.Where(a => statusArray.Contains(a.Status));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        // Define sort order based on tab context
        if (upcomingOnly)
        {
            // Nearest upcoming first
            query = query
                .OrderBy(a => a.AppointmentSlot!.Date)
                .ThenBy(a => a.AppointmentSlot!.StartTime);
        }
        else
        {
            // Most recent completed/cancelled/past first
            query = query
                .OrderByDescending(a => a.AppointmentSlot!.Date)
                .ThenByDescending(a => a.AppointmentSlot!.StartTime);
        }

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Dictionary<AppointmentStatus, int>> GetPatientStatusCountsAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.PatientId == patientId)
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);
    }

    public async Task<Dictionary<AppointmentStatus, int>> GetStatusCountsAsync(
        DateOnly? date = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(a => a.AppointmentSlot)
            .AsQueryable();

        if (date.HasValue)
            query = query.Where(a => a.AppointmentSlot != null && a.AppointmentSlot.Date == date.Value);

        return await query
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);
    }

    public async Task<IReadOnlyList<Appointment>> GetUpcomingByPatientAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        var today = GetVietnamToday();

        return await _dbSet
            .Include(a => a.AppointmentSlot)
            .Where(a => a.PatientId == patientId)
            .Where(a => a.Status == AppointmentStatus.Pending ||
                       a.Status == AppointmentStatus.Confirmed)
            .Where(a => a.AppointmentSlot != null && a.AppointmentSlot.Date >= today)
            .OrderBy(a => a.AppointmentSlot!.Date)
            .ThenBy(a => a.AppointmentSlot!.StartTime)
            .ToListAsync(cancellationToken);
    }

    private static DateOnly GetVietnamToday()
    {
        var utcNow = DateTime.UtcNow;
        foreach (var timeZoneId in VietnamTimeZoneIds)
        {
            try
            {
                var vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(
                    utcNow,
                    TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));
                return DateOnly.FromDateTime(vietnamNow);
            }
            catch (TimeZoneNotFoundException) { }
            catch (InvalidTimeZoneException) { }
        }

        return DateOnly.FromDateTime(utcNow);
    }
}
