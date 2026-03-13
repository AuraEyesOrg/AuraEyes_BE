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
    public OrgType OrgType { get; private set; }
    public decimal RatingAverage { get; private set; }
    public int RatingCount { get; private set; }

    private Organisation() { } // EF Core

    public Organisation(Guid ownerId, string name, OrgType orgType, string? address = null, string? licenseNumber = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Organisation name cannot be empty", nameof(name));

        OwnerId = ownerId;
        Name = name;
        OrgType = orgType;
        Address = address;
        LicenseNumber = licenseNumber;
        RatingAverage = 0m;
        RatingCount = 0;
    }

    public void UpdateDetails(string name, string? address, string? licenseNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Organisation name cannot be empty", nameof(name));

        Name = name;
        Address = address;
        LicenseNumber = licenseNumber;
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
}
