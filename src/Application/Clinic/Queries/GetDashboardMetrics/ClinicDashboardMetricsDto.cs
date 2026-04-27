namespace Application.Clinic.Queries.GetDashboardMetrics;

public class ClinicDashboardMetricsDto
{
    public int TodayAppointments { get; set; }
    public int CheckedInPatients { get; set; }
    public int PendingTasks { get; set; }
    public int CompletedToday { get; set; }
    public List<ClinicActivityDto> RecentActivity { get; set; } = new();
}

public class ClinicActivityDto
{
    public string PatientName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public DateTime Time { get; set; }
    public string Status { get; set; } = "pending"; // completed, pending, urgent
}
