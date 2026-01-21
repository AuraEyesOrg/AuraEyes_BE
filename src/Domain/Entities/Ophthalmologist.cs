using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Ophthalmologist profile entity - linked to ApplicationUser
/// </summary>
public class Ophthalmologist : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public string? Bio { get; private set; }
    public int YearsOfExperience { get; private set; }
    public bool IsVerified { get; private set; }

    // Navigation properties
    private readonly List<Certificate> _certificates = new();
    public IReadOnlyCollection<Certificate> Certificates => _certificates.AsReadOnly();

    private Ophthalmologist() { } // EF Core

    public Ophthalmologist(Guid userId, string? bio = null, int yearsOfExperience = 0)
    {
        UserId = userId;
        Bio = bio;
        YearsOfExperience = yearsOfExperience;
        IsVerified = false;
    }

    public void UpdateProfile(string? bio, int yearsOfExperience)
    {
        if (yearsOfExperience < 0)
            throw new ArgumentException("Years of experience cannot be negative", nameof(yearsOfExperience));

        Bio = bio;
        YearsOfExperience = yearsOfExperience;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Verify()
    {
        IsVerified = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unverify()
    {
        IsVerified = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddCertificate(Certificate certificate)
    {
        _certificates.Add(certificate);
        UpdatedAt = DateTime.UtcNow;
    }
}
