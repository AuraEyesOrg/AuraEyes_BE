namespace Application.Feedback.Common;

public class FeedbackRatingSummaryDto
{
    public Guid EntityId { get; set; }
    public decimal RatingAverage { get; set; }
    public int RatingCount { get; set; }
    public Dictionary<int, int> Distribution { get; set; } = new();
}
