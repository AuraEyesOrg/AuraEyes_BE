using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for ClinicAppointment aggregate root.
/// </summary>
public class ClinicAppointmentRepository : Repository<ClinicAppointment>, IClinicAppointmentRepository
{
    public ClinicAppointmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ClinicAppointment>> GetByOrganisationAsync(
        Guid organisationId,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        AppointmentStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(a => a.Patient)
            .Include(a => a.AppointmentSlot)
            .Include(a => a.AssignedDoctor)
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

    public async Task<IReadOnlyList<ClinicAppointment>> GetByPatientAsync(
        Guid patientId,
        AppointmentStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(a => a.Organisation)
            .Include(a => a.AppointmentSlot)
            .Include(a => a.AssignedDoctor)
            .Where(a => a.PatientId == patientId);

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        return await query
            .OrderByDescending(a => a.AppointmentSlot!.Date)
            .ThenBy(a => a.AppointmentSlot!.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ClinicAppointment>> GetBySlotAsync(
        Guid slotId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Include(a => a.AssignedDoctor)
            .Where(a => a.AppointmentSlotId == slotId)
            .Where(a => a.Status != AppointmentStatus.Cancelled)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ClinicAppointment?> GetByIdWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Include(a => a.Organisation)
            .Include(a => a.AppointmentSlot)
                .ThenInclude(s => s!.ScheduleTemplate)
            .Include(a => a.AssignedDoctor)
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
                a.Status != AppointmentStatus.Cancelled,
                cancellationToken);
    }

    public async Task<IReadOnlyList<ClinicAppointment>> GetByOrganisationAndDateAsync(
        Guid organisationId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Include(a => a.AppointmentSlot)
            .Include(a => a.AssignedDoctor)
            .Where(a => a.OrganisationId == organisationId)
            .Where(a => a.AppointmentSlot != null && a.AppointmentSlot.Date == date)
            .Where(a => a.Status != AppointmentStatus.Cancelled)
            .OrderBy(a => a.AppointmentSlot!.StartTime)
            .ThenBy(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<ClinicAppointment> Items, int TotalCount)> GetPagedAsync(
        Guid? organisationId = null,
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
            .Include(a => a.Organisation)
            .Include(a => a.AppointmentSlot)
            .Include(a => a.AssignedDoctor)
            .AsQueryable();

        if (organisationId.HasValue)
            query = query.Where(a => a.OrganisationId == organisationId.Value);

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

    public async Task<Dictionary<AppointmentStatus, int>> GetStatusCountsAsync(
        Guid organisationId,
        DateOnly? date = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(a => a.AppointmentSlot)
            .Where(a => a.OrganisationId == organisationId);

        if (date.HasValue)
            query = query.Where(a => a.AppointmentSlot != null && a.AppointmentSlot.Date == date.Value);

        return await query
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);
    }

    public async Task<IReadOnlyList<ClinicAppointment>> GetUpcomingByPatientAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return await _dbSet
            .Include(a => a.Organisation)
            .Include(a => a.AppointmentSlot)
            .Include(a => a.AssignedDoctor)
            .Where(a => a.PatientId == patientId)
            .Where(a => a.Status == AppointmentStatus.Pending || 
                       a.Status == AppointmentStatus.Confirmed ||
                       a.Status == AppointmentStatus.CheckedIn)
            .Where(a => a.AppointmentSlot != null && a.AppointmentSlot.Date >= today)
            .OrderBy(a => a.AppointmentSlot!.Date)
            .ThenBy(a => a.AppointmentSlot!.StartTime)
            .ToListAsync(cancellationToken);
    }
}
