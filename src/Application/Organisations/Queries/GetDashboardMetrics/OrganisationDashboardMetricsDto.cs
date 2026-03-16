namespace Application.Organisations.Queries.GetDashboardMetrics;

public class OrganisationDashboardMetricsDto
{
    public int TotalAppointments { get; set; }
    public int PendingAppointments { get; set; }
    public int AvailableSlotsToday { get; set; }
    public int ActiveDoctors { get; set; }
}