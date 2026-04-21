using Application.Common.Interfaces;
using Application.Organisations.Common;

namespace Application.Organisations.Commands.UpdateOrganisationSettings;

public record UpdateOrganisationSettingsCommand : ICommand<OrganisationSettingsDto>
{
    public Guid OrgAdminUserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string? LicenseNumber { get; init; }
    public string? TaxCode { get; init; }
    public string? Description { get; init; }
    public string? ContactFullName { get; init; }
    public string? ContactEmail { get; init; }
    public string? ContactPhone { get; init; }
    public string? AvatarUrl { get; init; }
}
