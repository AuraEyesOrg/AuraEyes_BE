namespace Application.SystemAdmin.Dashboard.Queries.GetTodaySummary;

public class TodaySummaryDto
{
    public int TotalAppointments { get; set; }
    public int CheckedInPatients { get; set; }
    public int CompletedVisits { get; set; }
    public int NoShowCount { get; set; }
}
