using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Platform;
using Microsoft.EntityFrameworkCore;

namespace Application.Notifications.Commands.MarkAllAsRead;

/// <summary>
/// Handler for MarkAllAsReadCommand
/// </summary>
public class MarkAllAsReadCommandHandler : ICommandHandler<MarkAllAsReadCommand>
{
    private readonly IRepository<Notification> _notificationRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationHubService _notificationHubService;

    public MarkAllAsReadCommandHandler(
        IRepository<Notification> notificationRepository,
        ICurrentUserService currentUser,
        INotificationHubService notificationHubService)
    {
        _notificationRepository = notificationRepository;
        _currentUser = currentUser;
        _notificationHubService = notificationHubService;
    }

    public async Task<Result> Handle(
        MarkAllAsReadCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Result.Unauthorized("User is not authenticated.");

        var userId = _currentUser.UserId.Value;

        await _notificationRepository
            .Query()
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(n => n.IsRead, true)
                    .SetProperty(n => n.UpdatedAt, DateTime.UtcNow),
                cancellationToken);

        await _notificationHubService.BroadcastUnreadCountAsync(userId, 0, cancellationToken);

        return Result.Success();
    }
}
