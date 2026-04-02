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
    Task BroadcastChatMessageAsync(Guid userId, ChatMessageRealtimeDto chatMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Broadcasts a room state change (ROOM_OPENED / ROOM_CLOSED) to multiple users.
    /// </summary>
    Task BroadcastRoomStateChangedAsync(IEnumerable<Guid> userIds, RoomStateChangedDto payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// Broadcasts typing indicator state to the other chat participant.
    /// </summary>
    Task BroadcastTypingIndicatorAsync(Guid userId, TypingIndicatorRealtimeDto payload, CancellationToken cancellationToken = default);
}
