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
    public OphthalmologistEmploymentType EmploymentType { get; private set; }
    public int? WorkingHoursPerWeek { get; private set; }
    public decimal? ExpectedMonthlySalary { get; private set; }
    public decimal? CommissionRate { get; private set; }
    public decimal? ActualMonthlySalary { get; private set; }
    public int YearsOfExperience { get; private set; }
    public bool IsVerified { get; private set; }
    public VerificationStatus VerificationStatus { get; private set; }
    public string? LicenseUrl { get; private set; }
    public string? DegreeUrl { get; private set; }
    public string? RejectionReason { get; private set; }
    public decimal RatingAverage { get; private set; }
    public int RatingCount { get; private set; }
    public decimal ConsultationFee { get; private set; }

    // Navigation properties
    private readonly List<Certificate> _certificates = new();
    public IReadOnlyCollection<Certificate> Certificates => _certificates.AsReadOnly();

    private Ophthalmologist() { } // EF Core

    public Ophthalmologist(
        Guid userId,
        string? bio = null,
        int yearsOfExperience = 0,
        string? phone = null,
        string? licenseUrl = null,
        string? degreeUrl = null,
        OphthalmologistEmploymentType employmentType = OphthalmologistEmploymentType.FullTime,
        int? workingHoursPerWeek = null,
        decimal? expectedMonthlySalary = null)
    {
        if (yearsOfExperience < 0)
            throw new ArgumentException("Years of experience cannot be negative", nameof(yearsOfExperience));
        if (workingHoursPerWeek.HasValue && (workingHoursPerWeek < 1 || workingHoursPerWeek > 112))
            throw new ArgumentException("Working hours per week must be between 1 and 112", nameof(workingHoursPerWeek));
        if (expectedMonthlySalary.HasValue && expectedMonthlySalary < 0)
            throw new ArgumentException("Expected monthly salary cannot be negative", nameof(expectedMonthlySalary));

        UserId = userId;
        Bio = bio;
        Phone = phone;
        EmploymentType = employmentType;
        WorkingHoursPerWeek = workingHoursPerWeek;
        ExpectedMonthlySalary = expectedMonthlySalary;
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

    public void UpdateEmploymentPreferences(
        OphthalmologistEmploymentType employmentType,
        int? workingHoursPerWeek,
        decimal? expectedMonthlySalary)
    {
        if (workingHoursPerWeek.HasValue && (workingHoursPerWeek < 1 || workingHoursPerWeek > 112))
            throw new ArgumentException("Working hours per week must be between 1 and 112", nameof(workingHoursPerWeek));
        if (expectedMonthlySalary.HasValue && expectedMonthlySalary < 0)
            throw new ArgumentException("Expected monthly salary cannot be negative", nameof(expectedMonthlySalary));

        EmploymentType = employmentType;
        WorkingHoursPerWeek = workingHoursPerWeek;
        ExpectedMonthlySalary = expectedMonthlySalary;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDealTerms(decimal commissionRate, decimal actualMonthlySalary)
    {
        if (commissionRate < 0 || commissionRate > 100)
            throw new ArgumentException("Commission rate must be between 0 and 100", nameof(commissionRate));
        if (actualMonthlySalary < 0)
            throw new ArgumentException("Actual monthly salary cannot be negative", nameof(actualMonthlySalary));

        CommissionRate = commissionRate;
        ActualMonthlySalary = actualMonthlySalary;
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

    public void SubmitCredentialReviewRequest()
    {
        var previousStatus = VerificationStatus;

        // First-time or re-submission flow stays in PendingVerification.
        // Already-approved doctors move to PendingUpdate for re-review.
        VerificationStatus = previousStatus == VerificationStatus.Approved
            ? VerificationStatus.PendingUpdate
            : VerificationStatus.PendingVerification;

        IsVerified = false;
        RejectionReason = null;
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

    public void UpdateConsultationFee(decimal fee)
    {
        if (fee < 0)
            throw new ArgumentException("Consultation fee cannot be negative", nameof(fee));

        ConsultationFee = fee;
        UpdatedAt = DateTime.UtcNow;
    }
}
