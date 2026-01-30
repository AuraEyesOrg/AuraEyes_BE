namespace Application.Organisations.Common;

/// <summary>
/// DTO for detailed organisation view.
/// </summary>
public class OrganisationDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string? OwnerFullName { get; set; }
    public string? OwnerEmail { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? LicenseNumber { get; set; }
    public string OrgType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
