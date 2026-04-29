namespace Application.Network.Profile.Queries.GetUserProfile;

/// <summary>
/// Profile information DTO for a network user (Ophthalmologist)
/// </summary>
public class UserProfileDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public int PostCount { get; set; }
    public List<UserProfileCertificateDto> Certificates { get; set; } = new();
}

public class UserProfileCertificateDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? DegreeLevel { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? IssuingAuthority { get; set; }
    public DateTime IssuedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? CertificateUrl { get; set; }
}
