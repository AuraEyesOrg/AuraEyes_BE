namespace Application.SystemAdmin.Organisations.Queries.GetOrganisationMetrics;

/// <summary>
/// DTO for organisation metrics
/// </summary>
public class OrganisationMetricsDto
{
    public int TotalOrganisations { get; set; }
    public int ActiveOrganisations { get; set; }
    public int InactiveOrganisations { get; set; }
    public decimal MonthlyChangePercentage { get; set; }

    public int TotalDevices { get; set; }
    public int OnlineDevices { get; set; }
    public int CalibrationRequiredDevices { get; set; }
    public bool CalibrationActionNeeded { get; set; }

    // By type breakdown
    public int HospitalCount { get; set; }
    public int ClinicCount { get; set; }
    public int PrivatePracticeCount { get; set; }
    public int OtherCount { get; set; }
}
