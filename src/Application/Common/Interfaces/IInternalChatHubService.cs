using Application.Network.InternalChat.Queries.GetGroups;
using Application.Network.InternalChat.Queries.GetMessages;

namespace Application.Common.Interfaces;

/// <summary>
/// Interface for broadcasting internal group chat realtime events via SignalR.
/// </summary>
public interface IInternalChatHubService
{
    /// <summary>
    /// Broadcasts a new internal group message to all members of the group.
    /// </summary>
    Task BroadcastMessageAsync(Guid groupId, InternalGroupMessageDto message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Broadcasts a notification about a new group created or user added.
    /// </summary>
    Task BroadcastGroupUpdateAsync(Guid groupId, string action, CancellationToken cancellationToken = default);
}
