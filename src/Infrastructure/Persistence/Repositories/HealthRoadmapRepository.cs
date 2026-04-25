using Domain.Entities.CarePlan;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for <see cref="HealthRoadmap"/> aggregate root.
/// </summary>
public class HealthRoadmapRepository : Repository<HealthRoadmap>, IHealthRoadmapRepository
{
    public HealthRoadmapRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<HealthRoadmap?> GetByPatientAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(r => r.PatientId == patientId, cancellationToken);
    }

    public async Task<HealthRoadmap?> GetByPatientWithStepsAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Steps)
            .FirstOrDefaultAsync(r => r.PatientId == patientId, cancellationToken);
    }

    public async Task<HealthRoadmapStep?> GetStepByIdAsync(
        Guid stepId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<HealthRoadmapStep>()
            .Include(s => s.Roadmap)
            .FirstOrDefaultAsync(s => s.Id == stepId, cancellationToken);
    }

    public void RemoveStep(HealthRoadmapStep step)
    {
        _context.Set<HealthRoadmapStep>().Remove(step);
    }
}
