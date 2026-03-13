using Domain.Entities.Consultation;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class OphthalmologistFeedbackRepository : Repository<OphthalmologistFeedback>, IOphthalmologistFeedbackRepository
{
    public OphthalmologistFeedbackRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByPatientAndSessionAsync(
        Guid patientId,
        Guid consultationSessionId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(
            x => x.PatientId == patientId && x.ConsultationSessionId == consultationSessionId,
            cancellationToken);
    }

    public async Task<OphthalmologistFeedback?> GetByIdForOphthalmologistAsync(
        Guid ophthalmologistId,
        Guid feedbackId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(
            x => x.Id == feedbackId && x.OphthalmologistId == ophthalmologistId,
            cancellationToken);
    }

    public async Task<(IReadOnlyList<OphthalmologistFeedback> Items, int TotalCount)> GetPagedByOphthalmologistAsync(
        Guid ophthalmologistId,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(x => x.OphthalmologistId == ophthalmologistId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(decimal RatingAverage, int RatingCount, Dictionary<int, int> Distribution)> GetRatingSummaryAsync(
        Guid ophthalmologistId,
        CancellationToken cancellationToken = default)
    {
        var summary = await _dbSet
            .Where(x => x.OphthalmologistId == ophthalmologistId)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Count = g.Count(),
                Avg = g.Average(x => (decimal?)x.Rating) ?? 0m
            })
            .FirstOrDefaultAsync(cancellationToken);

        var distributionRows = await _dbSet
            .Where(x => x.OphthalmologistId == ophthalmologistId)
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
