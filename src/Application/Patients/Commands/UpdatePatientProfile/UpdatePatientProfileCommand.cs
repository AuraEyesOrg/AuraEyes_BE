using Application.Common.Interfaces;
using Application.Patients.Common;

namespace Application.Patients.Commands.UpdatePatientProfile;

/// <summary>
/// Command to update patient profile information.
/// </summary>
public record UpdatePatientProfileCommand : ICommand<PatientProfileDto>
{
    /// <summary>
    /// ApplicationUser.Id of the current user.
    /// </summary>
    public Guid UserId { get; init; }

    public string FullName { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? DateOfBirth { get; init; }
    public string? Gender { get; init; }
    public string? Address { get; init; }
}
