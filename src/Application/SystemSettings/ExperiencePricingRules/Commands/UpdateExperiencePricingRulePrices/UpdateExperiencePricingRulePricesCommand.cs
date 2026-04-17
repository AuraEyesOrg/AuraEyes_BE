using Application.Common.Interfaces;
using Application.SystemSettings.ExperiencePricingRules.Common;

namespace Application.SystemSettings.ExperiencePricingRules.Commands.UpdateExperiencePricingRulePrices;

public class UpdateExperiencePricingRulePricesCommand : ICommand<IReadOnlyList<ExperiencePricingRuleDto>>
{
    public List<UpdateExperiencePricingRulePriceItem> Rules { get; init; } = new();
}

public class UpdateExperiencePricingRulePriceItem
{
    public Guid Id { get; init; }
    public decimal MinPrice { get; init; }
    public decimal MaxPrice { get; init; }
}