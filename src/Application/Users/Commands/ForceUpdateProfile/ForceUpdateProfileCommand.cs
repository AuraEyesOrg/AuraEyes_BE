using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.Users.Commands.ForceUpdateProfile;

/// <summary>
/// Command for new users to force update their profile and change temporary password.
/// </summary>
public record ForceUpdateProfileCommand : ICommand<bool>
{
    public string NewPassword { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public Gender? Gender { get; init; }
    public string? Address { get; init; }
    public string? CitizenId { get; init; }

    // Ophthalmologist specific
    public string? Bio { get; init; }
    public int? YearsOfExperience { get; init; }
    public decimal? ConsultationFee { get; init; }

    // Clinic Staff specific
    public string? Department { get; init; }
}
