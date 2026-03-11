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
    public int YearsOfExperience { get; set; }
    public bool IsVerified { get; set; }
}
