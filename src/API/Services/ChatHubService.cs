using API.Hubs;
using Application.Common.Interfaces;
using Application.Common.Models;
using Microsoft.AspNetCore.SignalR;

namespace API.Services;

/// <summary>
/// SignalR hub service implementation for broadcasting chat events.
/// </summary>
public class ChatHubService : IChatHubService
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly ILogger<ChatHubService> _logger;

    public ChatHubService(
        IHubContext<ChatHub> hubContext,
        ILogger<ChatHubService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task BroadcastChatMessageAsync(
        Guid userId,
        ChatMessageRealtimeDto chatMessage,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients
                .User(userId.ToString())
                .SendAsync("ReceiveChatMessage", chatMessage, cancellationToken);

            _logger.LogDebug(
                "Chat message event broadcast to user {UserId} for session {SessionId}",
                userId, chatMessage.SessionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to broadcast chat message event to user {UserId}",
                userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task BroadcastRoomStateChangedAsync(
        IEnumerable<Guid> userIds,
        RoomStateChangedDto payload,
        CancellationToken cancellationToken = default)
    {
        var ids = userIds.Select(id => id.ToString()).ToList();
        if (ids.Count == 0) return;

        try
        {
            await _hubContext.Clients
                .Users(ids)
                .SendAsync("RoomStateChanged", payload, cancellationToken);

            _logger.LogInformation(
                "Room state {Event} broadcast for session {SessionId} to {Count} user(s)",
                payload.Event, payload.SessionId, ids.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to broadcast {Event} for session {SessionId}",
                payload.Event, payload.SessionId);
            throw;
        }
    }
}
