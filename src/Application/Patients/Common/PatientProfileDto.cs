using Domain.Enums;

namespace Application.Patients.Common;

/// <summary>
/// Patient profile DTO returned from profile queries.
/// </summary>
public class PatientProfileDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
}
