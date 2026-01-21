using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Notification entity - user notifications
/// </summary>
public class Notification : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public bool IsRead { get; private set; }

    private Notification() { } // EF Core

    public Notification(Guid userId, string title, string content)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty", nameof(content));

        UserId = userId;
        Title = title;
        Content = content;
        IsRead = false;
    }

    public void MarkAsRead()
    {
        IsRead = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsUnread()
    {
        IsRead = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
