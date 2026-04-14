namespace Application.SystemAdmin.Organisations.Queries.GetOrganisations;

/// <summary>
/// DTO for organisation list item (clinic/hospital)
/// </summary>
public class OrganisationListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? LicenseNumber { get; set; }
    public string OrgType { get; set; } = string.Empty;
    public decimal RatingAverage { get; set; }
    public int RatingCount { get; set; }
    public string? OwnerAvatarUrl { get; set; }
    public int DeviceCount { get; set; }
    public int PurchasedAiQuota { get; set; }
    public int MonthlyQuotaLimit { get; set; }
    public int MonthlyQuotaUsed { get; set; }
    public int MonthlyQuotaRemaining { get; set; }
    public int ManagedPatientCount { get; set; }
    public int RegisteredPatientCount { get; set; }
    public int WalkInPatientCount { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
