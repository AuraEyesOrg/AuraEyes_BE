using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Feedback entity - user feedback and ratings
/// </summary>
public class Feedback : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public int Rating { get; private set; }

    private Feedback() { } // EF Core

    public Feedback(Guid userId, string content, int rating)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty", nameof(content));
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));

        UserId = userId;
        Content = content;
        Rating = rating;
    }

    public void Update(string content, int rating)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty", nameof(content));
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));

        Content = content;
        Rating = rating;
        UpdatedAt = DateTime.UtcNow;
    }
}
