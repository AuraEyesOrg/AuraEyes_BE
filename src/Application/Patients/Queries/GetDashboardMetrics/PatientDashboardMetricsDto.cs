namespace Application.Patients.Queries.GetDashboardMetrics;

public class PatientDashboardMetricsDto
{
    public int CompletedScreenings { get; set; }
    public int TotalReports { get; set; }
    public int UpcomingAppointments { get; set; }
}