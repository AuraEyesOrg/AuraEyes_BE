namespace Application.Ophthalmologists.Common;

/// <summary>
/// DTO for Ophthalmologist details.
/// </summary>
public class OphthalmologistDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? UserFullName { get; set; }
    public string? UserEmail { get; set; }
    public string? UserPhoneNumber { get; set; }
    public string? UserAddress { get; set; }
    public string? Bio { get; set; }
    public int YearsOfExperience { get; set; }
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<CertificateDto> Certificates { get; set; } = new();
}

/// <summary>
/// DTO for Certificate.
/// </summary>
public class CertificateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? IssuingAuthority { get; set; }
    public DateTime IssuedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? CertificateUrl { get; set; }
    public bool IsExpired { get; set; }
}
