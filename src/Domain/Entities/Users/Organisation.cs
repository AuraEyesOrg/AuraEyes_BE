using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Users;

/// <summary>
/// Organisation entity - represents hospitals, clinics, etc.
/// </summary>
public class Organisation : BaseEntity, IAggregateRoot
{
    public Guid OwnerId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Address { get; private set; }
    public string? LicenseNumber { get; private set; }
    public string? TaxCode { get; private set; }
    public string? Description { get; private set; }
    public OrgType OrgType { get; private set; }
    public decimal RatingAverage { get; private set; }
    public int RatingCount { get; private set; }

    /// <summary>Current purchased AI screening credits balance.</summary>
    public int PurchasedAiQuota { get; private set; }

    /// <summary>Monthly AI quota allocated from contract.</summary>
    public int MonthlyQuotaLimit { get; private set; }

    /// <summary>Monthly AI quota already consumed in the current month.</summary>
    public int MonthlyQuotaUsed { get; private set; }

    /// <summary>UTC timestamp of the most recent monthly quota reset/allocation.</summary>
    public DateTime? MonthlyQuotaLastResetAt { get; private set; }

    private Organisation() { } // EF Core

    public Organisation(
        Guid ownerId,
        string name,
        OrgType orgType,
        string? address = null,
        string? licenseNumber = null,
        string? taxCode = null,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Organisation name cannot be empty", nameof(name));

        OwnerId = ownerId;
        Name = name;
        OrgType = orgType;
        Address = address;
        LicenseNumber = licenseNumber;
        TaxCode = taxCode;
        Description = description;
        RatingAverage = 0m;
        RatingCount = 0;
    }

    public void UpdateDetails(
        string name,
        string? address,
        string? licenseNumber,
        string? taxCode,
        string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Organisation name cannot be empty", nameof(name));

        Name = name;
        Address = address;
        LicenseNumber = licenseNumber;
        TaxCode = taxCode;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeOrgType(OrgType orgType)
    {
        OrgType = orgType;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ApplyNewRating(int rating)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));

        var total = (RatingAverage * RatingCount) + rating;
        RatingCount += 1;
        RatingAverage = Math.Round(total / RatingCount, 2, MidpointRounding.AwayFromZero);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddPurchasedQuota(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive", nameof(amount));

        PurchasedAiQuota += amount;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool HasAvailableQuota()
    {
        return MonthlyQuotaUsed < MonthlyQuotaLimit || PurchasedAiQuota > 0;
    }

    public void ConsumeQuota()
    {
        if (MonthlyQuotaUsed < MonthlyQuotaLimit)
        {
            MonthlyQuotaUsed++;
            UpdatedAt = DateTime.UtcNow;
            return;
        }

        if (PurchasedAiQuota <= 0)
            throw new InvalidOperationException("No AI quota available.");

        PurchasedAiQuota--;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConfigureMonthlyQuota(int monthlyQuotaLimit, DateTime resetAtUtc)
    {
        if (monthlyQuotaLimit < 0)
            throw new ArgumentOutOfRangeException(nameof(monthlyQuotaLimit));

        MonthlyQuotaLimit = monthlyQuotaLimit;
        MonthlyQuotaUsed = 0;
        MonthlyQuotaLastResetAt = resetAtUtc;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateMonthlyQuotaLimit(int monthlyQuotaLimit)
    {
        if (monthlyQuotaLimit < 0)
            throw new ArgumentOutOfRangeException(nameof(monthlyQuotaLimit));

        MonthlyQuotaLimit = monthlyQuotaLimit;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ResetMonthlyQuota(DateTime resetAtUtc)
    {
        MonthlyQuotaUsed = 0;
        MonthlyQuotaLastResetAt = resetAtUtc;
        UpdatedAt = DateTime.UtcNow;
    }
}
