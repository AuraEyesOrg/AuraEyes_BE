using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

/// <summary>
/// SignalR Hub for realtime consultation chat events.
/// </summary>
[Authorize]
public class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(ILogger<ChatHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        _logger.LogInformation(
            "ChatHub client connected: ConnectionId={ConnectionId}, UserId={UserId}",
            Context.ConnectionId,
            userId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        _logger.LogInformation(
            "ChatHub client disconnected: ConnectionId={ConnectionId}, UserId={UserId}",
            Context.ConnectionId,
            userId);

        if (exception != null)
        {
            _logger.LogWarning(exception, "ChatHub client disconnected with error");
        }

        await base.OnDisconnectedAsync(exception);
    }
}
