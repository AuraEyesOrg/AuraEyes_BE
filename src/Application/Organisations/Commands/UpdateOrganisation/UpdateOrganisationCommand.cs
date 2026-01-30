using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.Organisations.Commands.UpdateOrganisation;

/// <summary>
/// Command to update an existing organisation.
/// </summary>
public record UpdateOrganisationCommand : ICommand
{
    /// <summary>
    /// The ID of the organisation to update.
    /// </summary>
    public Guid Id { get; init; }

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
