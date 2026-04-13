using Domain.Common;
using Domain.Entities.Scheduling;

namespace Domain.Repositories;

public interface IExperiencePricingRuleRepository : IRepository<ExperiencePricingRule>
{
    Task<ExperiencePricingRule?> GetByYearsOfExperienceAsync(
        int yearsOfExperience,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExperiencePricingRule>> GetActiveAsync(CancellationToken cancellationToken = default);
}