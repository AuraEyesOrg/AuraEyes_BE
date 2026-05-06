namespace Application.SystemAdmin.Dashboard.Queries.GetTodaySummary;

public class TodaySummaryDto
{
    public int TotalAppointments { get; set; }
    public int CheckedInPatients { get; set; }
    public int CompletedVisits { get; set; }
    public int NoShowCount { get; set; }
    public decimal TodayRevenue { get; set; }
    public decimal GrowthPercentageDay { get; set; }
    public decimal GrowthPercentageMonth { get; set; }
    public decimal GrowthPercentageYear { get; set; }
    public decimal RevenueGrowthPercentageDay { get; set; }
    public decimal RevenueGrowthPercentageMonth { get; set; }
    public decimal RevenueGrowthPercentageYear { get; set; }
}
