using Domain.Entities.Scheduling;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ExperiencePricingRuleRepository : Repository<ExperiencePricingRule>, IExperiencePricingRuleRepository
{
    public ExperiencePricingRuleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ExperiencePricingRule?> GetByYearsOfExperienceAsync(
        int yearsOfExperience,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.IsActive)
            .Where(r => r.MinYearsExperience <= yearsOfExperience && r.MaxYearsExperience >= yearsOfExperience)
            .OrderBy(r => r.MinYearsExperience)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ExperiencePricingRule>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.IsActive)
            .OrderBy(r => r.MinYearsExperience)
            .ToListAsync(cancellationToken);
    }
}