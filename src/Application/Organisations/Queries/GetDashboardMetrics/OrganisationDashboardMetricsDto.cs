namespace Application.Organisations.Queries.GetDashboardMetrics;

public class OrganisationDashboardMetricsDto
{
    public decimal UtilizationRatePercent { get; set; }
    public int RemainingAiQuota { get; set; }
    public int TotalAppointments { get; set; }
    public OrganisationAppointmentStatusBreakdownDto AppointmentStatus { get; set; } = new();
}

public class OrganisationAppointmentStatusBreakdownDto
{
    public int Pending { get; set; }
    public int Confirmed { get; set; }
    public int Completed { get; set; }
    public int Cancelled { get; set; }
    public int NoShow { get; set; }
}