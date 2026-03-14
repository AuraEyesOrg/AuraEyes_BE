namespace Application.SystemAdmin.Dashboard.Queries.GetRecentScreenings;

/// <summary>
/// DTO for recent screening list item
/// </summary>
public class RecentScreeningDto
{
    public Guid Id { get; set; }
    public string ScreeningCode { get; set; } = string.Empty;
    public Guid PatientId { get; set; }
    public string? PatientName { get; set; }
    public Guid? ClinicId { get; set; }
    public string? ClinicName { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? RiskLevel { get; set; }
    public bool IsCritical { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
