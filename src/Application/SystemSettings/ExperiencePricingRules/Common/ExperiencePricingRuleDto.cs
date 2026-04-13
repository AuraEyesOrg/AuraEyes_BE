namespace Application.SystemSettings.ExperiencePricingRules.Common;

public record ExperiencePricingRuleDto
{
    public Guid Id { get; init; }
    public int MinYearsExperience { get; init; }
    public int MaxYearsExperience { get; init; }
    public decimal MinPrice { get; init; }
    public decimal MaxPrice { get; init; }
    public bool IsActive { get; init; }
}