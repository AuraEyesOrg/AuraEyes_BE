namespace Application.Feedback.Common;

public class ClinicFeedbackDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string? PatientFullName { get; set; }
    public Guid AppointmentId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public Guid? DoctorId { get; set; }
    public Guid? StaffId { get; set; }
    public DateTime CreatedAt { get; set; }
}
