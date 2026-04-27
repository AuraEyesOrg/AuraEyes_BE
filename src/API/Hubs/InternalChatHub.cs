using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

/// <summary>
/// SignalR Hub for internal collaboration chat.
/// Provides real-time messaging and notifications for clinic staff and ophthalmologists.
/// </summary>
[Authorize]
public class InternalChatHub : Hub
{
    private readonly ILogger<InternalChatHub> _logger;

    public InternalChatHub(ILogger<InternalChatHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        _logger.LogInformation("InternalChatHub client connected: ConnectionId={ConnectionId}, UserId={UserId}",
            Context.ConnectionId, userId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        _logger.LogInformation("InternalChatHub client disconnected: ConnectionId={ConnectionId}, UserId={UserId}",
            Context.ConnectionId, userId);

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Join a specific group chat's SignalR group.
    /// </summary>
    public async Task JoinGroup(string groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
        _logger.LogInformation("User {UserId} joined group {GroupId}", Context.UserIdentifier, groupId);
    }

    /// <summary>
    /// Leave a specific group chat's SignalR group.
    /// </summary>
    public async Task LeaveGroup(string groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
        _logger.LogInformation("User {UserId} left group {GroupId}", Context.UserIdentifier, groupId);
    }
}
