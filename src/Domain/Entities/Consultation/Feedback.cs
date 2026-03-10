using Domain.Common;

namespace Domain.Entities.Consultation;

/// <summary>
/// Feedback entity - user feedback and ratings.
/// TargetType+TargetId polymorphic reference: Doctor | Organisation | App.
/// </summary>
public class Feedback : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }

    /// <summary>"Doctor" | "Organisation" | "App"</summary>
    public string TargetType { get; private set; } = string.Empty;

    /// <summary>Nullable — App target has no specific entity ID.</summary>
    public Guid? TargetId { get; private set; }

    public string Content { get; private set; } = string.Empty;
    public int Rating { get; private set; }

    private Feedback() { } // EF Core

    public Feedback(Guid userId, string targetType, string content, int rating, Guid? targetId = null)
    {
        if (string.IsNullOrWhiteSpace(targetType))
            throw new ArgumentException("TargetType cannot be empty", nameof(targetType));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty", nameof(content));
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));

        UserId = userId;
        TargetType = targetType;
        TargetId = targetId;
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
