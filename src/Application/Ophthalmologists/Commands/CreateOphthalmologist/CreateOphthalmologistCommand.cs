using Application.Common.Interfaces;
using Domain.Enums;

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
    /// Employment type of the ophthalmologist profile.
    /// </summary>
    public OphthalmologistEmploymentType EmploymentType { get; init; } = OphthalmologistEmploymentType.FullTime;

}
