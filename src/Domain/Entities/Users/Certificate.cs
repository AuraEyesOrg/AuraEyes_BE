using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Users;

/// <summary>
/// Certificate entity - belongs to Ophthalmologist
/// </summary>
public class Certificate : BaseEntity
{
    public Guid OphthalmologistId { get; private set; }
    public CertificateType Type { get; private set; }
    public DegreeLevel? DegreeLevel { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? IssuingAuthority { get; private set; }
    public DateTime IssuedDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public string? CertificateUrl { get; private set; }
    public string? LicenseNumber { get; private set; }
    public string? ScopeOfPractice { get; private set; }
    public string? IssuingInstitution { get; private set; }

    private Certificate() { } // EF Core

    public Certificate(
        Guid ophthalmologistId,
        CertificateType type,
        string name,
        DegreeLevel? degreeLevel,
        string? issuingAuthority,
        DateTime issuedDate,
        DateTime? expiryDate = null,
        string? certificateUrl = null,
        string? licenseNumber = null,
        string? scopeOfPractice = null,
        string? issuingInstitution = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Certificate name cannot be empty", nameof(name));

        if (type == CertificateType.Degree && !degreeLevel.HasValue)
            throw new ArgumentException("Degree level is required for degree credentials", nameof(degreeLevel));

        if (type != CertificateType.Degree && degreeLevel.HasValue)
            throw new ArgumentException("Degree level can only be set for degree credentials", nameof(degreeLevel));

        OphthalmologistId = ophthalmologistId;
        Type = type;
        DegreeLevel = degreeLevel;
        Name = name;
        IssuingAuthority = issuingAuthority;
        IssuedDate = issuedDate;
        ExpiryDate = expiryDate;
        CertificateUrl = certificateUrl;
        LicenseNumber = licenseNumber;
        ScopeOfPractice = scopeOfPractice;
        IssuingInstitution = issuingInstitution;
    }

    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;

    public void UpdateCertificate(
        CertificateType type,
        string name,
        DegreeLevel? degreeLevel,
        string? issuingAuthority,
        DateTime issuedDate,
        DateTime? expiryDate,
        string? certificateUrl,
        string? licenseNumber,
        string? scopeOfPractice,
        string? issuingInstitution)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Certificate name cannot be empty", nameof(name));

        if (type == CertificateType.Degree && !degreeLevel.HasValue)
            throw new ArgumentException("Degree level is required for degree credentials", nameof(degreeLevel));

        if (type != CertificateType.Degree && degreeLevel.HasValue)
            throw new ArgumentException("Degree level can only be set for degree credentials", nameof(degreeLevel));

        Type = type;
        DegreeLevel = degreeLevel;
        Name = name;
        IssuingAuthority = issuingAuthority;
        IssuedDate = issuedDate;
        ExpiryDate = expiryDate;
        CertificateUrl = certificateUrl;
        LicenseNumber = licenseNumber;
        ScopeOfPractice = scopeOfPractice;
        IssuingInstitution = issuingInstitution;
        UpdatedAt = DateTime.UtcNow;
    }
}
