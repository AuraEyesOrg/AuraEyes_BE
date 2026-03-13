using Domain.Common;

namespace Domain.Entities.Consultation;

public class OphthalmologistFeedback : BaseEntity, IAggregateRoot
{
    public Guid PatientId { get; private set; }
    public Guid OphthalmologistId { get; private set; }
    public Guid ConsultationSessionId { get; private set; }
    public int Rating { get; private set; }
    public string? Comment { get; private set; }

    private OphthalmologistFeedback() { }

    public OphthalmologistFeedback(
        Guid patientId,
        Guid ophthalmologistId,
        Guid consultationSessionId,
        int rating,
        string? comment)
    {
        if (patientId == Guid.Empty)
            throw new ArgumentException("Patient ID is required.", nameof(patientId));
        if (ophthalmologistId == Guid.Empty)
            throw new ArgumentException("Ophthalmologist ID is required.", nameof(ophthalmologistId));
        if (consultationSessionId == Guid.Empty)
            throw new ArgumentException("Consultation session ID is required.", nameof(consultationSessionId));
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.", nameof(rating));

        PatientId = patientId;
        OphthalmologistId = ophthalmologistId;
        ConsultationSessionId = consultationSessionId;
        Rating = rating;
        Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
    }
}
