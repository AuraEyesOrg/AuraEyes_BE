using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Platform;
using Microsoft.EntityFrameworkCore;

namespace Application.Notifications.Commands.MarkAsRead;

/// <summary>
/// Handler for MarkNotificationAsReadCommand
/// </summary>
public class MarkNotificationAsReadCommandHandler : ICommandHandler<MarkNotificationAsReadCommand>
{
    private readonly IRepository<Notification> _notificationRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationHubService _notificationHubService;

    public MarkNotificationAsReadCommandHandler(
        IRepository<Notification> notificationRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork,
        INotificationHubService notificationHubService)
    {
        _notificationRepository = notificationRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _notificationHubService = notificationHubService;
    }

    public async Task<Result> Handle(
        MarkNotificationAsReadCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Result.Unauthorized("User is not authenticated.");

        var userId = _currentUser.UserId.Value;

        var notification = await _notificationRepository.GetByIdAsync(
            request.NotificationId, cancellationToken);

        if (notification is null)
            return Result.NotFound($"Notification {request.NotificationId} not found.");

        // Security check: ensure user owns this notification
        if (notification.UserId != userId)
            return Result.Forbidden("You cannot mark this notification as read.");

        // Mark as read
        notification.MarkAsRead();
        await _notificationRepository.UpdateAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var unreadCount = await _notificationRepository
            .Query()
            .AsNoTracking()
            .CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);

        await _notificationHubService.BroadcastUnreadCountAsync(userId, unreadCount, cancellationToken);

        return Result.Success();
    }
}
