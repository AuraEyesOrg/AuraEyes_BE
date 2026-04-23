using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.SystemAdmin.Users.Commands.OnboardStaff;

/// <summary>
/// Command to onboard a new staff member (Ophthalmologist, or ClinicStaff).
/// Creates both the Identity user and the corresponding profile entity.
/// </summary>
public record OnboardStaffCommand : ICommand<Guid>
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty; // Ophthalmologist, ClinicStaff
    
    // Additional fields for profiles could be added here if needed,
    // but for initial onboarding, basic info is usually enough.
}
