using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for PatientVisit aggregate root.
/// </summary>
public class PatientVisitRepository : Repository<PatientVisit>, IPatientVisitRepository
{
    public PatientVisitRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PatientVisit?> GetByIdWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(v => v.Appointment)
                .ThenInclude(a => a!.AppointmentSlot)
            .Include(v => v.Patient)
            .Include(v => v.AssignedDoctor)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<PatientVisit?> GetByAppointmentIdAsync(
        Guid appointmentId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(v => v.Patient)
            .Include(v => v.AssignedDoctor)
            .FirstOrDefaultAsync(v => v.AppointmentId == appointmentId, cancellationToken);
    }

    public async Task<IReadOnlyList<PatientVisit>> GetByPatientAsync(
        Guid patientId,
        PatientVisitStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(v => v.Appointment)
                .ThenInclude(a => a!.AppointmentSlot)
            .Include(v => v.AssignedDoctor)
            .Where(v => v.PatientId == patientId);

        if (status.HasValue)
            query = query.Where(v => v.Status == status.Value);

        return await query
            .OrderByDescending(v => v.CheckedInAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PatientVisit>> GetByDoctorAsync(
        Guid doctorId,
        PatientVisitStatus? status = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(v => v.Patient)
            .Include(v => v.Appointment)
                .ThenInclude(a => a!.AppointmentSlot)
            .Where(v => v.AssignedDoctorId == doctorId);

        if (status.HasValue)
            query = query.Where(v => v.Status == status.Value);

        if (fromDate.HasValue)
        {
            var fromDateTime = fromDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            query = query.Where(v => v.CheckedInAt >= fromDateTime);
        }

        if (toDate.HasValue)
        {
            var toDateTime = toDate.Value.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            query = query.Where(v => v.CheckedInAt < toDateTime);
        }

        return await query
            .OrderByDescending(v => v.CheckedInAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<PatientVisit> Items, int TotalCount)> GetPagedAsync(
        Guid? patientId = null,
        Guid? doctorId = null,
        PatientVisitStatus? status = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(v => v.Patient)
            .Include(v => v.AssignedDoctor)
            .Include(v => v.Appointment)
                .ThenInclude(a => a!.AppointmentSlot)
            .AsQueryable();

        if (patientId.HasValue)
            query = query.Where(v => v.PatientId == patientId.Value);

        if (doctorId.HasValue)
            query = query.Where(v => v.AssignedDoctorId == doctorId.Value);

        if (status.HasValue)
            query = query.Where(v => v.Status == status.Value);

        if (fromDate.HasValue)
        {
            var fromDateTime = fromDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            query = query.Where(v => v.CheckedInAt >= fromDateTime);
        }

        if (toDate.HasValue)
        {
            var toDateTime = toDate.Value.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            query = query.Where(v => v.CheckedInAt < toDateTime);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(v => v.CheckedInAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> HasActiveVisitAsync(
        Guid appointmentId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(v =>
            v.AppointmentId == appointmentId &&
            v.Status != PatientVisitStatus.Completed,
            cancellationToken);
    }
}
