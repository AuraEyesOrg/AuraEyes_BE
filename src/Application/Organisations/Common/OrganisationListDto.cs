namespace Application.Organisations.Common;

/// <summary>
/// DTO for organisation list item.
/// </summary>
public class OrganisationListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? LicenseNumber { get; set; }
    public string OrgType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
