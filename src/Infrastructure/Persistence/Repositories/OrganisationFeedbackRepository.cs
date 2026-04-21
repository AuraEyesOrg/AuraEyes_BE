using Domain.Entities.Consultation;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class OrganisationFeedbackRepository : Repository<OrganisationFeedback>, IOrganisationFeedbackRepository
{
    public OrganisationFeedbackRepository(ApplicationDbContext context) : base(context)
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

    public async Task<OrganisationFeedback?> GetByIdForOrganisationAsync(
        Guid organisationId,
        Guid feedbackId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(
            x => x.Id == feedbackId && x.OrganisationId == organisationId,
            cancellationToken);
    }

    public async Task<(IReadOnlyList<OrganisationFeedback> Items, int TotalCount)> GetPagedByOrganisationAsync(
        Guid organisationId,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(x => x.OrganisationId == organisationId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(decimal RatingAverage, int RatingCount, Dictionary<int, int> Distribution)> GetRatingSummaryAsync(
        Guid organisationId,
        CancellationToken cancellationToken = default)
    {
        var summary = await _dbSet
            .Where(x => x.OrganisationId == organisationId)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Count = g.Count(),
                Avg = g.Average(x => (decimal?)x.Rating) ?? 0m
            })
            .FirstOrDefaultAsync(cancellationToken);

        var distributionRows = await _dbSet
            .Where(x => x.OrganisationId == organisationId)
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
}
