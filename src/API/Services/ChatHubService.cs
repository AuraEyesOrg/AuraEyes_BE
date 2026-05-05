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
        Guid recipientProfileId,
        ChatMessageRealtimeDto chatMessage,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Attempting to broadcast chat message to UserId={UserId}, RecipientProfileId={RecipientProfileId} for SessionId={SessionId}",
                userId,
                recipientProfileId,
                chatMessage.SessionId);

            var userChannel = userId.ToString();
            var profileGroupName = $"profile_{recipientProfileId}";
            var groupName = $"session_{chatMessage.SessionId}";

            // Prefer direct user delivery so message is received even when session-group join/rejoin is delayed.
            await _hubContext.Clients
                .User(userChannel)
                .SendAsync("ReceiveChatMessage", chatMessage, cancellationToken);

            // Fallback to profile group to handle user-id claim mismatches across clients.
            await _hubContext.Clients
                .Group(profileGroupName)
                .SendAsync("ReceiveChatMessage", chatMessage, cancellationToken);

            // Keep group delivery as compatibility fallback for existing clients joined by session.
            await _hubContext.Clients
                .Group(groupName)
                .SendAsync("ReceiveChatMessage", chatMessage, cancellationToken);

            _logger.LogInformation(
                "Chat message broadcast SUCCESS to UserId={UserId}, ProfileGroup={ProfileGroup}, SessionGroup={GroupName} for SessionId={SessionId}, MessageId={MessageId}",
                userId,
                profileGroupName,
                groupName,
                chatMessage.SessionId,
                chatMessage.MessageId);
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
    public async Task BroadcastRoomStateChangedAsync(
        string groupName,
        RoomStateChangedDto payload,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients
                .Group(groupName)
                .SendAsync("RoomStateChanged", payload, cancellationToken);

            _logger.LogInformation(
                "Room state {Event} broadcast for session {SessionId} to group {GroupName}",
                payload.Event, payload.SessionId, groupName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to broadcast {Event} for session {SessionId} to group {GroupName}",
                payload.Event, payload.SessionId, groupName);
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
