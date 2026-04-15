namespace Application.Organisations.Common;

public class OrganisationSettingsDto
{
    public Guid OrganisationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string OrgType { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? LicenseNumber { get; set; }
    public string? TaxCode { get; set; }
    public string? Description { get; set; }
    public string ContactFullName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string? ContactPhone { get; set; }
    public string? AvatarUrl { get; set; }
}
