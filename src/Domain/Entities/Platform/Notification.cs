using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Platform;

/// <summary>
/// Notification entity - system-wide user notifications with JSONB payload support
/// Supports real-time delivery via SignalR and persistent storage in PostgreSQL
/// </summary>
public class Notification : BaseEntity, IAggregateRoot
{
    /// <summary>
    /// The user ID who will receive this notification
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Notification title - brief summary
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Notification message - detailed content
    /// </summary>
    public string Message { get; private set; } = string.Empty;

    /// <summary>
    /// Type of notification - determines routing and icon display
    /// </summary>
    public NotificationType Type { get; private set; }

    /// <summary>
    /// Optional domain object reference this notification points to (session, appointment, screening, etc.)
    /// </summary>
    public Guid? ReferenceId { get; private set; }

    /// <summary>
    /// Whether the notification has been read by the user
    /// </summary>
    public bool IsRead { get; private set; }

    /// <summary>
    /// JSON payload containing metadata specific to notification type
    /// Stored as JSONB in PostgreSQL for efficient querying
    /// </summary>
    public string? Payload { get; private set; }

    private Notification() { } // EF Core

    /// <summary>
    /// Creates a new notification with type and optional JSONB payload
    /// </summary>
    public Notification(
        Guid userId,
        string title,
        string message,
        NotificationType type,
        Guid? referenceId = null,
        string? payload = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be empty", nameof(message));

        UserId = userId;
        Title = title;
        Message = message;
        Type = type;
        ReferenceId = referenceId;
        IsRead = false;
        Payload = payload;
    }

    /// <summary>
    /// Mark notification as read
    /// </summary>
    public void MarkAsRead()
    {
        IsRead = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Mark notification as unread
    /// </summary>
    public void MarkAsUnread()
    {
        IsRead = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
