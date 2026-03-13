using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Consultation;

public class WebsiteFeedback : BaseEntity, IAggregateRoot
{
    public Guid PatientId { get; private set; }
    public int Rating { get; private set; }
    public WebsiteFeedbackCategory Category { get; private set; }
    public string? Comment { get; private set; }

    private WebsiteFeedback() { }

    public WebsiteFeedback(Guid patientId, int rating, WebsiteFeedbackCategory category, string? comment)
    {
        if (patientId == Guid.Empty)
            throw new ArgumentException("Patient ID is required.", nameof(patientId));
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.", nameof(rating));

        PatientId = patientId;
        Rating = rating;
        Category = category;
        Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
    }
}
