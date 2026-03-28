namespace Application.Ophthalmologists.Queries.GetDashboardMetrics;

public class OphthalmologistDashboardMetricsDto
{
    public int PendingReviews { get; set; }
    public int UrgentCases { get; set; }
    public int CompletedToday { get; set; }
    public int OpenSlotsToday { get; set; }
    public List<OphthalmologistUrgentCaseDto> UrgentCaseList { get; set; } = new();
}

public class OphthalmologistUrgentCaseDto
{
    public Guid ConsultationSessionId { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = string.Empty;
    public decimal ConfidenceScore { get; set; }
    public DateTime? AppointmentTime { get; set; }
    public DateTime CreatedAt { get; set; }
}