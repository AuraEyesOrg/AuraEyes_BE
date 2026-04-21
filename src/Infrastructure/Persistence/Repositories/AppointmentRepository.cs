using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for unified Appointment aggregate root.
/// Supports both ONLINE_CONSULTATION and CLINIC_VISIT appointment types.
/// </summary>
public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    private static readonly string[] VietnamTimeZoneIds =
    [
        "SE Asia Standard Time", // Windows
        "Asia/Ho_Chi_Minh"       // Linux/macOS (IANA)
    ];

    public AppointmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Appointment>> GetByOrganisationAsync(
        Guid organisationId,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        AppointmentStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(a => a.Patient)
            .Include(a => a.AppointmentSlot)
            .Include(a => a.Doctor)
            .Include(a => a.Organisation)
            .Where(a => a.OrganisationId == organisationId);

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        if (fromDate.HasValue || toDate.HasValue)
        {
            query = query.Where(a => a.AppointmentSlot != null);

            if (fromDate.HasValue)
                query = query.Where(a => a.AppointmentSlot!.Date >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.AppointmentSlot!.Date <= toDate.Value);
        }

        return await query
            .OrderBy(a => a.AppointmentSlot!.Date)
            .ThenBy(a => a.AppointmentSlot!.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Appointment>> GetByDoctorAsync(
        Guid doctorId,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        AppointmentStatus? status = null,
        AppointmentType? type = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(a => a.Patient)
            .Include(a => a.AppointmentSlot)
            .Include(a => a.Organisation)
            .Where(a => a.DoctorId == doctorId);

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        if (type.HasValue)
            query = query.Where(a => a.Type == type.Value);

        if (fromDate.HasValue || toDate.HasValue)
        {
            query = query.Where(a => a.AppointmentSlot != null);

            if (fromDate.HasValue)
                query = query.Where(a => a.AppointmentSlot!.Date >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.AppointmentSlot!.Date <= toDate.Value);
        }

        return await query
            .OrderBy(a => a.AppointmentSlot!.Date)
            .ThenBy(a => a.AppointmentSlot!.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Appointment>> GetByPatientAsync(
        Guid patientId,
        AppointmentType? type = null,
        AppointmentStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(a => a.Organisation)
            .Include(a => a.Doctor)
            .Include(a => a.AppointmentSlot)
            .Where(a => a.PatientId == patientId);

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        if (type.HasValue)
            query = query.Where(a => a.Type == type.Value);

        return await query
            .OrderByDescending(a => a.AppointmentSlot!.Date)
            .ThenBy(a => a.AppointmentSlot!.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Appointment>> GetBySlotAsync(
        Guid slotId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
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
            .Include(a => a.Organisation)
            .Include(a => a.Doctor)
            .Include(a => a.AppointmentSlot)
                .ThenInclude(s => s!.ScheduleTemplate)
            .Include(a => a.ConsultationSession)
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

    public async Task<IReadOnlyList<Appointment>> GetByOrganisationAndDateAsync(
        Guid organisationId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Include(a => a.AppointmentSlot)
            .Include(a => a.Doctor)
            .Include(a => a.Organisation)
            .Where(a => a.OrganisationId == organisationId)
            .Where(a => a.AppointmentSlot != null && a.AppointmentSlot.Date == date)
            .Where(a => a.Status != AppointmentStatus.Cancelled)
            .OrderBy(a => a.AppointmentSlot!.StartTime)
            .ThenBy(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Appointment> Items, int TotalCount)> GetPagedAsync(
        Guid? organisationId = null,
        Guid? doctorId = null,
        Guid? patientId = null,
        AppointmentType? type = null,
        AppointmentStatus? status = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(a => a.Patient)
            .Include(a => a.Organisation)
            .Include(a => a.Doctor)
            .Include(a => a.AppointmentSlot)
            .AsQueryable();

        if (organisationId.HasValue)
            query = query.Where(a => a.OrganisationId == organisationId.Value);

        if (doctorId.HasValue)
            query = query.Where(a => a.DoctorId == doctorId.Value);

        if (patientId.HasValue)
            query = query.Where(a => a.PatientId == patientId.Value);

        if (type.HasValue)
            query = query.Where(a => a.Type == type.Value);

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
        AppointmentType? type = null,
        IReadOnlyCollection<AppointmentStatus>? statuses = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(a => a.Organisation)
            .Include(a => a.Doctor)
            .Include(a => a.AppointmentSlot)
            .Where(a => a.PatientId == patientId);

        if (type.HasValue)
            query = query.Where(a => a.Type == type.Value);

        if (statuses is { Count: > 0 })
        {
            var statusArray = statuses.Distinct().ToArray();
            query = query.Where(a => statusArray.Contains(a.Status));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var pageIndex = pageNumber < 1 ? 0 : pageNumber - 1;
        var safeSize = pageSize <= 0 ? 10 : pageSize;

        var items = await query
            .OrderByDescending(a => a.AppointmentSlot!.Date)
            .ThenByDescending(a => a.AppointmentSlot!.StartTime)
            .Skip(pageIndex * safeSize)
            .Take(safeSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Dictionary<AppointmentStatus, int>> GetPatientStatusCountsAsync(
        Guid patientId,
        AppointmentType? type = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(a => a.PatientId == patientId);

        if (type.HasValue)
            query = query.Where(a => a.Type == type.Value);

        return await query
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);
    }

    public async Task<Dictionary<AppointmentStatus, int>> GetStatusCountsAsync(
        Guid? organisationId = null,
        Guid? doctorId = null,
        DateOnly? date = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(a => a.AppointmentSlot)
            .AsQueryable();

        if (organisationId.HasValue)
            query = query.Where(a => a.OrganisationId == organisationId.Value);

        if (doctorId.HasValue)
            query = query.Where(a => a.DoctorId == doctorId.Value);

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
            .Include(a => a.Organisation)
            .Include(a => a.Doctor)
            .Include(a => a.AppointmentSlot)
            .Where(a => a.PatientId == patientId)
            .Where(a => a.Status == AppointmentStatus.Pending ||
                       a.Status == AppointmentStatus.Confirmed ||
                       a.Status == AppointmentStatus.CheckedIn)
            .Where(a => a.AppointmentSlot != null && a.AppointmentSlot.Date >= today)
            .OrderBy(a => a.AppointmentSlot!.Date)
            .ThenBy(a => a.AppointmentSlot!.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Appointment>> GetUpcomingByDoctorAsync(
        Guid doctorId,
        CancellationToken cancellationToken = default)
    {
        var today = GetVietnamToday();

        return await _dbSet
            .Include(a => a.Patient)
            .Include(a => a.Organisation)
            .Include(a => a.AppointmentSlot)
            .Where(a => a.DoctorId == doctorId)
            .Where(a => a.Status == AppointmentStatus.Pending ||
                       a.Status == AppointmentStatus.Confirmed ||
                       a.Status == AppointmentStatus.CheckedIn)
            .Where(a => a.AppointmentSlot != null && a.AppointmentSlot.Date >= today)
            .OrderBy(a => a.AppointmentSlot!.Date)
            .ThenBy(a => a.AppointmentSlot!.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<Appointment?> GetByConsultationSessionIdAsync(
        Guid consultationSessionId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Include(a => a.AppointmentSlot)
            .FirstOrDefaultAsync(a => a.ConsultationSessionId == consultationSessionId, cancellationToken);
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
            catch (TimeZoneNotFoundException)
            {
                // Try next ID.
            }
            catch (InvalidTimeZoneException)
            {
                // Try next ID.
            }
        }

        return DateOnly.FromDateTime(utcNow);
    }
}
