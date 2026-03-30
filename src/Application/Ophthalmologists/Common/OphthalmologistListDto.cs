namespace Application.Ophthalmologists.Common;

/// <summary>
/// DTO for Ophthalmologist list item (lightweight version for lists).
/// </summary>
public class OphthalmologistListDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? UserFullName { get; set; }
    public string? UserEmail { get; set; }
    public string? UserAvatarUrl { get; set; }
    public string? Bio { get; set; }
    public int YearsOfExperience { get; set; }
    public bool IsVerified { get; set; }
    public int CertificateCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? LicenseUrl { get; set; }
    public string? DegreeUrl { get; set; }
    public decimal RatingAverage { get; set; }
    public int RatingCount { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public List<DegreeDto> Degrees { get; set; } = new();
    public List<CertificateDto> Certificates { get; set; } = new();
}
