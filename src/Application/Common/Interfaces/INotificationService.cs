using Domain.Enums;

namespace Application.Common.Interfaces;

/// <summary>
/// Service for sending real-time notifications via SignalR and persisting to database
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Sends a notification to a specific user
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <param name="title">Notification title</param>
    /// <param name="message">Notification message content</param>
    /// <param name="type">Type of notification for categorization and routing</param>
    /// <param name="payload">Optional payload object containing metadata (will be serialized to JSON)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendAsync(
        Guid userId,
        string title,
        string message,
        NotificationType type,
        object? payload = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Legacy method - sends a simple reminder notification
    /// </summary>
    [Obsolete("Use SendAsync with NotificationType instead")]
    Task SendAsync(Guid userId, string message, CancellationToken cancellationToken = default);
}
