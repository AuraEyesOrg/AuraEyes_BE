using Application.Common.Models;

namespace Application.Common.Interfaces;

/// <summary>
/// Interface for broadcasting chat realtime events via SignalR.
/// </summary>
public interface IChatHubService
{
    /// <summary>
    /// Broadcasts a realtime chat event to a specific user.
    /// </summary>
    Task BroadcastChatMessageAsync(
        Guid userId,
        Guid recipientProfileId,
        ChatMessageRealtimeDto chatMessage,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Broadcasts a room state change to multiple users.
    /// </summary>
    Task BroadcastRoomStateChangedAsync(IEnumerable<Guid> userIds, RoomStateChangedDto payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// Broadcasts a room state change to a specific role/group.
    /// </summary>
    Task BroadcastRoomStateChangedAsync(string groupName, RoomStateChangedDto payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// Broadcasts typing indicator state to the other chat participant.
    /// </summary>
    Task BroadcastTypingIndicatorAsync(Guid userId, TypingIndicatorRealtimeDto payload, CancellationToken cancellationToken = default);
}
