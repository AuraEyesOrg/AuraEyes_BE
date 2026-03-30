using Application.Common.Models;

namespace Application.Common.Interfaces;

/// <summary>
/// Interface for broadcasting notifications via SignalR
/// Abstracts the SignalR hub context for clean architecture compliance
/// </summary>
public interface INotificationHubService
{
    /// <summary>
    /// Broadcasts a notification to a specific user via SignalR
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <param name="notification">Notification DTO to broadcast</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task BroadcastToUserAsync(Guid userId, NotificationDto notification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Broadcasts a notification to a group of users
    /// </summary>
    /// <param name="groupName">Group name</param>
    /// <param name="notification">Notification DTO to broadcast</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task BroadcastToGroupAsync(string groupName, NotificationDto notification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Broadcasts unread notification count to a specific user
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <param name="count">Unread count value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task BroadcastUnreadCountAsync(Guid userId, int count, CancellationToken cancellationToken = default);
}
