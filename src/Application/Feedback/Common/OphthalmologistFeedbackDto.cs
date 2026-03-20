namespace Application.Feedback.Common;

public class OphthalmologistFeedbackDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string? PatientFullName { get; set; }
    public Guid OphthalmologistId { get; set; }
    public Guid ConsultationSessionId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
