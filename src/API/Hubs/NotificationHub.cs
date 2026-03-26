using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

/// <summary>
/// SignalR Hub for real-time notifications
/// Clients connect via WebSocket and receive push notifications
/// </summary>
[Authorize]
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Called when a client connects to the hub
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        _logger.LogInformation("Client connected: ConnectionId={ConnectionId}, UserId={UserId}",
            Context.ConnectionId, userId);

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects from the hub
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        _logger.LogInformation("Client disconnected: ConnectionId={ConnectionId}, UserId={UserId}",
            Context.ConnectionId, userId);

        if (exception != null)
        {
            _logger.LogWarning(exception, "Client disconnected with error");
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Allows clients to join a specific group (e.g., organization group)
    /// </summary>
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("User {UserId} joined group {GroupName}",
            Context.UserIdentifier, groupName);
    }

    /// <summary>
    /// Allows clients to leave a specific group
    /// </summary>
    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("User {UserId} left group {GroupName}",
            Context.UserIdentifier, groupName);
    }
}
