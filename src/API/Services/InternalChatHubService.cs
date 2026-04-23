using API.Hubs;
using Application.Common.Interfaces;
using Application.Network.InternalChat.Queries.GetGroups;
using Application.Network.InternalChat.Queries.GetMessages;
using Microsoft.AspNetCore.SignalR;

namespace API.Services;

/// <summary>
/// SignalR hub service implementation for broadcasting internal group chat events.
/// </summary>
public class InternalChatHubService : IInternalChatHubService
{
    private readonly IHubContext<InternalChatHub> _hubContext;
    private readonly ILogger<InternalChatHubService> _logger;

    public InternalChatHubService(
        IHubContext<InternalChatHub> hubContext,
        ILogger<InternalChatHubService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task BroadcastMessageAsync(
        Guid groupId,
        InternalGroupMessageDto message,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Broadcast to the SignalR group corresponding to the chat group
            await _hubContext.Clients
                .Group(groupId.ToString())
                .SendAsync("ReceiveInternalMessage", message, cancellationToken);

            _logger.LogInformation(
                "Internal group message broadcast to group {GroupId}, MessageId={MessageId}",
                groupId, message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to broadcast internal group message to group {GroupId}",
                groupId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task BroadcastGroupUpdateAsync(
        Guid groupId,
        string action,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients
                .Group(groupId.ToString())
                .SendAsync("GroupUpdated", new { groupId, action }, cancellationToken);

            _logger.LogInformation(
                "Internal group update broadcast to group {GroupId}, Action={Action}",
                groupId, action);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to broadcast internal group update to group {GroupId}",
                groupId);
            throw;
        }
    }
}
