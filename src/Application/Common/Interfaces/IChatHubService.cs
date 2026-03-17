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
    /// <param name="userId">Target user ID</param>
    /// <param name="chatMessage">Realtime chat payload</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task BroadcastChatMessageAsync(Guid userId, ChatMessageRealtimeDto chatMessage, CancellationToken cancellationToken = default);
}
