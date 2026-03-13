using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Users;

/// <summary>
/// Ophthalmologist profile entity - linked to ApplicationUser
/// </summary>
public class Ophthalmologist : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public string? Bio { get; private set; }
    public string? Phone { get; private set; }
    public int YearsOfExperience { get; private set; }
    public bool IsVerified { get; private set; }
    public VerificationStatus VerificationStatus { get; private set; }
    public string? LicenseUrl { get; private set; }
    public string? DegreeUrl { get; private set; }
    public string? RejectionReason { get; private set; }
    public decimal RatingAverage { get; private set; }
    public int RatingCount { get; private set; }

    // Navigation properties
    private readonly List<Certificate> _certificates = new();
    public IReadOnlyCollection<Certificate> Certificates => _certificates.AsReadOnly();

    private Ophthalmologist() { } // EF Core

    public Ophthalmologist(Guid userId, string? bio = null, int yearsOfExperience = 0, string? phone = null, string? licenseUrl = null, string? degreeUrl = null)
    {
        UserId = userId;
        Bio = bio;
        Phone = phone;
        YearsOfExperience = yearsOfExperience;
        IsVerified = false;
        VerificationStatus = VerificationStatus.PendingVerification;
        LicenseUrl = licenseUrl;
        DegreeUrl = degreeUrl;
        RatingAverage = 0m;
        RatingCount = 0;
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
        VerificationStatus = VerificationStatus.Approved;
        RejectionReason = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(string? reason = null)
    {
        IsVerified = false;
        VerificationStatus = VerificationStatus.Rejected;
        RejectionReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unverify()
    {
        IsVerified = false;
        VerificationStatus = VerificationStatus.PendingVerification;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddCertificate(Certificate certificate)
    {
        _certificates.Add(certificate);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCredentialFiles(string? licenseUrl, string? degreeUrl)
    {
        if (licenseUrl != null) LicenseUrl = licenseUrl;
        if (degreeUrl != null) DegreeUrl = degreeUrl;
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
