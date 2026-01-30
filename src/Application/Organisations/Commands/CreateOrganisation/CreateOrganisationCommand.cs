using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.Organisations.Commands.CreateOrganisation;

/// <summary>
/// Command to create a new organisation.
/// </summary>
public record CreateOrganisationCommand : ICommand<Guid>
{
    /// <summary>
    /// The ID of the ApplicationUser who owns this organisation.
    /// </summary>
    public Guid OwnerId { get; init; }

    /// <summary>
    /// Organisation name.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Organisation address.
    /// </summary>
    public string? Address { get; init; }

    /// <summary>
    /// Business license number.
    /// </summary>
    public string? LicenseNumber { get; init; }

    /// <summary>
    /// Type of organisation (Hospital, Clinic, etc.).
    /// </summary>
    public OrgType OrgType { get; init; }
}
