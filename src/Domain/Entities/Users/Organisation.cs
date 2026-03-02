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
}
