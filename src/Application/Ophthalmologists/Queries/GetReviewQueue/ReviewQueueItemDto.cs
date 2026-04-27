namespace Application.Ophthalmologists.Queries.GetReviewQueue;

public class ReviewQueueItemDto
{
    public Guid ScreeningId { get; set; }
    public Guid ConsultationSessionId { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = string.Empty;
    public decimal ConfidenceScore { get; set; }
    public string? AiSummary { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string ReviewStatus { get; set; } = "READY_FOR_REVIEW";
    public int WaitingMinutes { get; set; }
    public DateTime? AppointmentTime { get; set; }
    public DateTime CreatedAt { get; set; }
}
