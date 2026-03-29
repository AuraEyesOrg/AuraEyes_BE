using Domain.Enums;

namespace Application.Common.Models;

/// <summary>
/// Data transfer object for notifications
/// Used for API responses and SignalR real-time delivery
/// </summary>
public record NotificationDto
{
    /// <summary>
    /// Unique notification identifier
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// User who receives the notification
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Notification title
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Notification message content
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Type of notification for categorization
    /// </summary>
    public NotificationType Type { get; init; }

    /// <summary>
    /// Optional domain object reference this notification targets
    /// </summary>
    public Guid? ReferenceId { get; init; }

    /// <summary>
    /// Whether the notification has been read
    /// </summary>
    public bool IsRead { get; init; }

    /// <summary>
    /// JSON payload containing metadata
    /// </summary>
    public string? Payload { get; init; }

    /// <summary>
    /// When the notification was created
    /// </summary>
    public DateTime CreatedAt { get; init; }
}
