using Domain.Common;

namespace Domain.Entities.Platform;

/// <summary>
/// Defines a named leave compensation policy set by admin.
/// Examples: "Trực ngày lễ nhỏ" = +2 days, "Trực Tết" = +3 days.
/// </summary>
public class LeavePolicy : BaseEntity, IAggregateRoot
{
    /// <summary>Human-readable name of the policy (e.g., "Trực Tết").</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Number of leave days to add when this policy is applied.</summary>
    public int AdditionalDays { get; private set; }

    /// <summary>Optional description explaining the context of the policy.</summary>
    public string? Description { get; private set; }

    private LeavePolicy() { } // EF Core

    public static LeavePolicy Create(string name, int additionalDays, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Policy name cannot be empty.", nameof(name));

        if (additionalDays <= 0)
            throw new ArgumentException("Additional days must be positive.", nameof(additionalDays));

        return new LeavePolicy
        {
            Name = name.Trim(),
            AdditionalDays = additionalDays,
            Description = description?.Trim()
        };
    }

    public void Update(string name, int additionalDays, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Policy name cannot be empty.", nameof(name));

        if (additionalDays <= 0)
            throw new ArgumentException("Additional days must be positive.", nameof(additionalDays));

        Name = name.Trim();
        AdditionalDays = additionalDays;
        Description = description?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}
