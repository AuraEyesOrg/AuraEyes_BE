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
    public string Name { get; private set; } = string.Empty;
    public string? IssuingAuthority { get; private set; }
    public DateTime IssuedDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public string? CertificateUrl { get; private set; }

    private Certificate() { } // EF Core

    public Certificate(
        Guid ophthalmologistId,
        CertificateType type,
        string name,
        string? issuingAuthority,
        DateTime issuedDate,
        DateTime? expiryDate = null,
        string? certificateUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Certificate name cannot be empty", nameof(name));

        OphthalmologistId = ophthalmologistId;
        Type = type;
        Name = name;
        IssuingAuthority = issuingAuthority;
        IssuedDate = issuedDate;
        ExpiryDate = expiryDate;
        CertificateUrl = certificateUrl;
    }

    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;

    public void UpdateCertificate(
        CertificateType type,
        string name,
        string? issuingAuthority,
        DateTime issuedDate,
        DateTime? expiryDate,
        string? certificateUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Certificate name cannot be empty", nameof(name));

        Type = type;
        Name = name;
        IssuingAuthority = issuingAuthority;
        IssuedDate = issuedDate;
        ExpiryDate = expiryDate;
        CertificateUrl = certificateUrl;
        UpdatedAt = DateTime.UtcNow;
    }
}
