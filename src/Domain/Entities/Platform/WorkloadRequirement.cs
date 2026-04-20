using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Platform;

/// <summary>
/// Required working hours configuration by employment type and period type.
/// </summary>
public class WorkloadRequirement : BaseEntity, IAggregateRoot
{
    public OphthalmologistEmploymentType EmploymentType { get; private set; }
    public WorkloadPeriodType PeriodType { get; private set; }
    public decimal RequiredHours { get; private set; }

    private WorkloadRequirement()
    {
    }

    public WorkloadRequirement(
        OphthalmologistEmploymentType employmentType,
        WorkloadPeriodType periodType,
        decimal requiredHours)
    {
        ValidateRequiredHours(requiredHours);

        EmploymentType = employmentType;
        PeriodType = periodType;
        RequiredHours = requiredHours;
    }

    public void UpdateRequiredHours(decimal requiredHours)
    {
        ValidateRequiredHours(requiredHours);

        RequiredHours = requiredHours;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateRequiredHours(decimal requiredHours)
    {
        if (requiredHours < 0)
            throw new ArgumentException("Required hours cannot be negative", nameof(requiredHours));

        if (requiredHours > 744)
            throw new ArgumentException("Required hours must be less than or equal to 744", nameof(requiredHours));
    }
}
