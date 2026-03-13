using Domain.Enums;

namespace Application.Feedback.Common;

public class WebsiteFeedbackDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public int Rating { get; set; }
    public WebsiteFeedbackCategory Category { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
