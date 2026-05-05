using Domain.Entities.Consultation;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ClinicFeedbackRepository : Repository<ClinicFeedback>, IClinicFeedbackRepository
{
    public ClinicFeedbackRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByPatientAndAppointmentAsync(
        Guid patientId,
        Guid appointmentId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(
            x => x.PatientId == patientId && x.AppointmentId == appointmentId,
            cancellationToken);
    }

    public async Task<bool> ExistsByTargetAsync(
        Guid patientId,
        Guid appointmentId,
        Guid? doctorId,
        Guid? staffId,
        CancellationToken cancellationToken = default)
    {
        if (doctorId.HasValue)
        {
            return await _dbSet.AnyAsync(
                x => x.PatientId == patientId
                     && x.AppointmentId == appointmentId
                     && x.DoctorId == doctorId,
                cancellationToken);
        }

        if (staffId.HasValue)
        {
            return await _dbSet.AnyAsync(
                x => x.PatientId == patientId
                     && x.AppointmentId == appointmentId
                     && x.StaffId == staffId,
                cancellationToken);
        }

        // CLINIC target: no doctorId, no staffId
        return await _dbSet.AnyAsync(
            x => x.PatientId == patientId
                 && x.AppointmentId == appointmentId
                 && x.DoctorId == null
                 && x.StaffId == null,
            cancellationToken);
    }

    public async Task<IReadOnlyDictionary<Guid, IReadOnlySet<string>>> GetSubmittedTargetsByAppointmentsAsync(
        Guid patientId,
        IReadOnlyCollection<Guid> appointmentIds,
        CancellationToken cancellationToken = default)
    {
        if (appointmentIds is null || appointmentIds.Count == 0)
            return new Dictionary<Guid, IReadOnlySet<string>>();

        var distinctIds = appointmentIds.Distinct().ToArray();

        var rows = await _dbSet
            .Where(x => x.PatientId == patientId && distinctIds.Contains(x.AppointmentId))
            .Select(x => new { x.AppointmentId, x.DoctorId, x.StaffId })
            .ToListAsync(cancellationToken);

        // Build the map: each row determines its target type
        var result = new Dictionary<Guid, HashSet<string>>();
        foreach (var row in rows)
        {
            if (!result.TryGetValue(row.AppointmentId, out var set))
            {
                set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                result[row.AppointmentId] = set;
            }

            if (row.DoctorId.HasValue)
                set.Add("DOCTOR");
            else if (row.StaffId.HasValue)
                set.Add("STAFF");
            else
                set.Add("CLINIC");
        }

        return result.ToDictionary(
            kvp => kvp.Key,
            kvp => (IReadOnlySet<string>)kvp.Value);
    }

    public async Task<IReadOnlySet<Guid>> GetAppointmentIdsWithFeedbackAsync(
        Guid patientId,
        IReadOnlyCollection<Guid> appointmentIds,
        CancellationToken cancellationToken = default)
    {
        if (appointmentIds is null || appointmentIds.Count == 0)
            return new HashSet<Guid>();

        var distinctIds = appointmentIds.Distinct().ToArray();

        var existing = await _dbSet
            .Where(x => x.PatientId == patientId && distinctIds.Contains(x.AppointmentId))
            .Select(x => x.AppointmentId)
            .ToListAsync(cancellationToken);

        return new HashSet<Guid>(existing);
    }

    public async Task<(IReadOnlyList<ClinicFeedback> Items, int TotalCount)> GetPagedAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet;

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(decimal RatingAverage, int RatingCount, Dictionary<int, int> Distribution)> GetRatingSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var summary = await _dbSet
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Count = g.Count(),
                Avg = g.Average(x => (decimal?)x.Rating) ?? 0m
            })
            .FirstOrDefaultAsync(cancellationToken);

        var distributionRows = await _dbSet
            .GroupBy(x => x.Rating)
            .Select(g => new { Rating = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var distribution = new Dictionary<int, int>
        {
            [1] = 0,
            [2] = 0,
            [3] = 0,
            [4] = 0,
            [5] = 0
        };

        foreach (var item in distributionRows)
            distribution[item.Rating] = item.Count;

        var ratingAverage = summary is null
            ? 0m
            : Math.Round(summary.Avg, 2, MidpointRounding.AwayFromZero);

        return (ratingAverage, summary?.Count ?? 0, distribution);
    }

    public async Task<(decimal RatingAverage, int RatingCount)> GetDoctorRatingSummaryAsync(
        Guid doctorId,
        CancellationToken cancellationToken = default)
    {
        var summary = await _dbSet
            .Where(x => x.DoctorId == doctorId)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Count = g.Count(),
                Avg = g.Average(x => (decimal?)x.Rating) ?? 0m
            })
            .FirstOrDefaultAsync(cancellationToken);

        var ratingAverage = summary is null
            ? 0m
            : Math.Round(summary.Avg, 2, MidpointRounding.AwayFromZero);

        return (ratingAverage, summary?.Count ?? 0);
    }
}
