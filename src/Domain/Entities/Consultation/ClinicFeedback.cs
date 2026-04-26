using Domain.Common;

namespace Domain.Entities.Consultation;

/// <summary>
/// ClinicFeedback - Feedback for the clinic visit experience.
/// Replaces the old OrganisationFeedback since organization logic was removed.
/// Can target the overall clinic experience, a specific doctor, or clinic staff.
/// </summary>
public class ClinicFeedback : BaseEntity, IAggregateRoot
{
    public Guid PatientId { get; private set; }
    
    /// <summary>Optional OrganisationId for legacy support or future use, but no longer required.</summary>
    public Guid? OrganisationId { get; private set; }
    
    public Guid AppointmentId { get; private set; }
    
    public int Rating { get; private set; }
    
    public string? Comment { get; private set; }

    /// <summary>Optional reference to a specific doctor being reviewed.</summary>
    public Guid? DoctorId { get; private set; }

    /// <summary>Optional reference to a specific staff member being reviewed.</summary>
    public Guid? StaffId { get; private set; }

    private ClinicFeedback() { }

    public ClinicFeedback(
        Guid patientId,
        Guid appointmentId,
        int rating,
        string? comment,
        Guid? doctorId = null,
        Guid? staffId = null,
        Guid? organisationId = null)
    {
        if (patientId == Guid.Empty)
            throw new ArgumentException("Patient ID is required.", nameof(patientId));
        if (appointmentId == Guid.Empty)
            throw new ArgumentException("Appointment ID is required.", nameof(appointmentId));
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.", nameof(rating));

        PatientId = patientId;
        AppointmentId = appointmentId;
        Rating = rating;
        Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
        DoctorId = doctorId;
        StaffId = staffId;
        OrganisationId = organisationId;
    }
}
