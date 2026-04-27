namespace Application.ClinicStaffs.Common;

/// <summary>
/// Current clinic staff profile payload for self-service settings/profile screens.
/// </summary>
public class ClinicStaffProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public string? CitizenId { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Department { get; set; }
    public string? EmployeeCode { get; set; }
    public IReadOnlyList<string> SubRoles { get; set; } = [];
    public bool IsEmailVerified { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

