namespace Application.Feedback.Common;

public class OrganisationFeedbackDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid OrganisationId { get; set; }
    public Guid AppointmentId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
