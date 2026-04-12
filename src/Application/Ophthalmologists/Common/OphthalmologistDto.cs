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
    public string? LicenseUrl { get; set; }
    public string? DegreeUrl { get; set; }
    public decimal RatingAverage { get; set; }
    public int RatingCount { get; set; }
    public decimal? CommissionRate { get; set; }
    public decimal? ActualMonthlySalary { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public List<DegreeDto> Degrees { get; set; } = new();
    public List<CertificateDto> Certificates { get; set; } = new();
}

public class DegreeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? DegreeLevel { get; set; }
    public string? IssuingAuthority { get; set; }
    public DateTime IssuedDate { get; set; }
    public string? DegreeUrl { get; set; }
    public string? Title { get; set; }
    public string? Abbreviation { get; set; }
}

/// <summary>
/// DTO for Certificate.
/// </summary>
public class CertificateDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? DegreeLevel { get; set; }
    public string? IssuingAuthority { get; set; }
    public DateTime IssuedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? CertificateUrl { get; set; }
    public bool IsExpired { get; set; }
}
