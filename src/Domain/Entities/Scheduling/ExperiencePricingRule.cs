using Domain.Common;

namespace Domain.Entities.Scheduling;

/// <summary>
/// Defines the allowed slot pricing band for a years-of-experience interval.
/// </summary>
public class ExperiencePricingRule : BaseEntity, IAggregateRoot
{
    public int MinYearsExperience { get; private set; }
    public int MaxYearsExperience { get; private set; }
    public decimal MinPrice { get; private set; }
    public decimal MaxPrice { get; private set; }
    public bool IsActive { get; private set; }

    private ExperiencePricingRule()
    {
    }

    public ExperiencePricingRule(
        int minYearsExperience,
        int maxYearsExperience,
        decimal minPrice,
        decimal maxPrice,
        bool isActive = true)
    {
        Validate(minYearsExperience, maxYearsExperience, minPrice, maxPrice);

        MinYearsExperience = minYearsExperience;
        MaxYearsExperience = maxYearsExperience;
        MinPrice = minPrice;
        MaxPrice = maxPrice;
        IsActive = isActive;
    }

    public bool AppliesTo(int yearsOfExperience)
        => yearsOfExperience >= MinYearsExperience && yearsOfExperience <= MaxYearsExperience;

    public bool AllowsPrice(decimal price)
        => price >= MinPrice && price <= MaxPrice;

    public void Update(int minYearsExperience, int maxYearsExperience, decimal minPrice, decimal maxPrice)
    {
        Validate(minYearsExperience, maxYearsExperience, minPrice, maxPrice);

        MinYearsExperience = minYearsExperience;
        MaxYearsExperience = maxYearsExperience;
        MinPrice = minPrice;
        MaxPrice = maxPrice;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void Validate(
        int minYearsExperience,
        int maxYearsExperience,
        decimal minPrice,
        decimal maxPrice)
    {
        if (minYearsExperience < 0)
            throw new ArgumentException("Minimum years of experience cannot be negative", nameof(minYearsExperience));

        if (maxYearsExperience < minYearsExperience)
            throw new ArgumentException("Maximum years of experience must be greater than or equal to minimum years", nameof(maxYearsExperience));

        if (minPrice <= 0)
            throw new ArgumentException("Minimum price must be greater than zero", nameof(minPrice));

        if (maxPrice < minPrice)
            throw new ArgumentException("Maximum price must be greater than or equal to minimum price", nameof(maxPrice));

        if (decimal.Truncate(minPrice) != minPrice)
            throw new ArgumentException("Minimum price must be an integer value", nameof(minPrice));

        if (decimal.Truncate(maxPrice) != maxPrice)
            throw new ArgumentException("Maximum price must be an integer value", nameof(maxPrice));
    }
}