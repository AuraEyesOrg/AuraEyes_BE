using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemSettings.ExperiencePricingRules.Common;
using Domain.Repositories;

namespace Application.SystemSettings.ExperiencePricingRules.Queries.GetExperiencePricingRules;

public class GetExperiencePricingRulesQueryHandler
    : IQueryHandler<GetExperiencePricingRulesQuery, IReadOnlyList<ExperiencePricingRuleDto>>
{
    private readonly IExperiencePricingRuleRepository _experiencePricingRuleRepository;

    public GetExperiencePricingRulesQueryHandler(IExperiencePricingRuleRepository experiencePricingRuleRepository)
    {
        _experiencePricingRuleRepository = experiencePricingRuleRepository;
    }

    public async Task<Result<IReadOnlyList<ExperiencePricingRuleDto>>> Handle(
        GetExperiencePricingRulesQuery request,
        CancellationToken cancellationToken)
    {
        var rules = await _experiencePricingRuleRepository.GetActiveAsync(cancellationToken);

        var dto = rules
            .OrderBy(r => r.MinYearsExperience)
            .Select(r => new ExperiencePricingRuleDto
            {
                Id = r.Id,
                MinYearsExperience = r.MinYearsExperience,
                MaxYearsExperience = r.MaxYearsExperience,
                MinPrice = r.MinPrice,
                MaxPrice = r.MaxPrice,
                IsActive = r.IsActive
            })
            .ToList();

        return Result<IReadOnlyList<ExperiencePricingRuleDto>>.Success(dto);
    }
}