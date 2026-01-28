using Application.Common.Interfaces;

namespace Application.Ophthalmologists.Commands.CreateOphthalmologist;

/// <summary>
/// Command to create a new ophthalmologist profile.
/// </summary>
public record CreateOphthalmologistCommand : ICommand<Guid>
{
    /// <summary>
    /// The ID of the ApplicationUser to link this ophthalmologist profile to.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Bio/description of the ophthalmologist.
    /// </summary>
    public string? Bio { get; init; }

    /// <summary>
    /// Number of years of professional experience.
    /// </summary>
    public int YearsOfExperience { get; init; }
}
