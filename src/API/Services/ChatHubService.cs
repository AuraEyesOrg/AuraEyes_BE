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

            _logger.LogInformation(
                "Chat message broadcast to UserId={UserId} for SessionId={SessionId}, MessageId={MessageId}",
                userId, chatMessage.SessionId, chatMessage.MessageId);
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

    /// <inheritdoc />
    public async Task BroadcastTypingIndicatorAsync(
        Guid userId,
        TypingIndicatorRealtimeDto payload,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients
                .User(userId.ToString())
                .SendAsync("TypingIndicatorChanged", payload, cancellationToken);

            _logger.LogDebug(
                "Typing indicator broadcast to UserId={UserId} for SessionId={SessionId}, IsTyping={IsTyping}",
                userId,
                payload.SessionId,
                payload.IsTyping);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to broadcast typing indicator event to user {UserId}",
                userId);
            throw;
        }
    }
}
