namespace Application.SystemAdmin.Users.Queries.GetUsers;

/// <summary>
/// DTO for user list item
/// </summary>
public class UserListDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public List<string> Roles { get; set; } = new();
    public string Status { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public decimal? ConsultationFee { get; set; }
    public Guid? OphthalmologistId { get; set; }
    public List<string> SubRoles { get; set; } = new();
    public Guid? ClinicStaffId { get; set; }
    public string? AvatarUrl { get; set; }
    public string? ProviderAvatarUrl { get; set; }
}
