using Application.Common.Interfaces;

namespace Application.Ophthalmologists.Commands.UpdateOphthalmologist;

/// <summary>
/// Command to update an existing ophthalmologist profile.
/// </summary>
public record UpdateOphthalmologistCommand : ICommand
{
    /// <summary>
    /// The ID of the ophthalmologist to update.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Updated bio/description.
    /// </summary>
    public string? Bio { get; init; }

    /// <summary>
    /// Updated years of experience.
    /// </summary>
    public int YearsOfExperience { get; init; }
}
