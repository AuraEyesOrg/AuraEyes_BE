namespace Application.Ophthalmologists.Queries.GetDashboardMetrics;

public class OphthalmologistDashboardMetricsDto
{
    public int PendingReviews { get; set; }
    public int UrgentCases { get; set; }
    public int CompletedToday { get; set; }
    public int OpenSlotsToday { get; set; }
}