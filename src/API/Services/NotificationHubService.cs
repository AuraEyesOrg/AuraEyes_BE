using API.Hubs;
using Application.Common.Interfaces;
using Application.Common.Models;
using Microsoft.AspNetCore.SignalR;

namespace API.Services;

/// <summary>
/// SignalR hub service implementation for broadcasting notifications
/// </summary>
public class NotificationHubService : INotificationHubService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<NotificationHubService> _logger;

    public NotificationHubService(
        IHubContext<NotificationHub> hubContext,
        ILogger<NotificationHubService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task BroadcastToUserAsync(
        Guid userId,
        NotificationDto notification,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients
                .User(userId.ToString())
                .SendAsync("ReceiveNotification", notification, cancellationToken);

            _logger.LogDebug(
                "Notification broadcast to user {UserId}: {Title}",
                userId, notification.Title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to broadcast notification to user {UserId}",
                userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task BroadcastToGroupAsync(
        string groupName,
        NotificationDto notification,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients
                .Group(groupName)
                .SendAsync("ReceiveNotification", notification, cancellationToken);

            _logger.LogDebug(
                "Notification broadcast to group {GroupName}: {Title}",
                groupName, notification.Title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to broadcast notification to group {GroupName}",
                groupName);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task BroadcastUnreadCountAsync(
        Guid userId,
        int count,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients
                .User(userId.ToString())
                .SendAsync("ReceiveUnreadCount", count, cancellationToken);

            _logger.LogDebug(
                "Unread count broadcast to user {UserId}: {Count}",
                userId,
                count);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to broadcast unread count to user {UserId}",
                userId);
            throw;
        }
    }

}
