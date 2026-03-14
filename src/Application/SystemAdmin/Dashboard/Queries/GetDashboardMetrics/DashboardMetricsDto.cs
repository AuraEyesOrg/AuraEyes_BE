namespace Application.SystemAdmin.Dashboard.Queries.GetDashboardMetrics;

/// <summary>
/// DTO for dashboard metrics summary
/// </summary>
public class DashboardMetricsDto
{
    // Total Screenings Today (3.3.2)
    public int TotalScreeningsToday { get; set; }
    public int TotalScreeningsYesterday { get; set; }
    public decimal ScreeningsChangePercentage { get; set; }

    // AI Accuracy (3.3.3)
    public decimal AiAccuracy { get; set; }
    public decimal AiAccuracyChangePercentage { get; set; }

    // Pending Reviews (3.3.4)
    public int PendingReviews { get; set; }
    public int CriticalCases { get; set; }
    public bool ActionRequired { get; set; }

    // Quick Stats
    public int TotalActiveClinics { get; set; }
    public int TotalActiveDevices { get; set; }
    public int TotalUsers { get; set; }
    public int TotalPatients { get; set; }
}
