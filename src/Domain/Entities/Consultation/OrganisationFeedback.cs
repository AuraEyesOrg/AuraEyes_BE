using Domain.Common;

namespace Domain.Entities.Consultation;

public class OrganisationFeedback : BaseEntity, IAggregateRoot
{
    public Guid PatientId { get; private set; }
    public Guid OrganisationId { get; private set; }
    public Guid AppointmentId { get; private set; }
    public int Rating { get; private set; }
    public string? Comment { get; private set; }

    private OrganisationFeedback() { }

    public OrganisationFeedback(
        Guid patientId,
        Guid organisationId,
        Guid appointmentId,
        int rating,
        string? comment)
    {
        if (patientId == Guid.Empty)
            throw new ArgumentException("Patient ID is required.", nameof(patientId));
        if (organisationId == Guid.Empty)
            throw new ArgumentException("Organisation ID is required.", nameof(organisationId));
        if (appointmentId == Guid.Empty)
            throw new ArgumentException("Appointment ID is required.", nameof(appointmentId));
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.", nameof(rating));

        PatientId = patientId;
        OrganisationId = organisationId;
        AppointmentId = appointmentId;
        Rating = rating;
        Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
    }
}
