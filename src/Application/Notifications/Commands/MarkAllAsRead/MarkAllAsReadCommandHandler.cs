using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Platform;

namespace Application.Notifications.Commands.MarkAllAsRead;

/// <summary>
/// Handler for MarkAllAsReadCommand
/// </summary>
public class MarkAllAsReadCommandHandler : ICommandHandler<MarkAllAsReadCommand>
{
    private readonly IRepository<Notification> _notificationRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public MarkAllAsReadCommandHandler(
        IRepository<Notification> notificationRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        MarkAllAsReadCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Result.Unauthorized("User is not authenticated.");

        var userId = _currentUser.UserId.Value;

        // Get all unread notifications for user
        var unreadNotifications = await _notificationRepository.FindAsync(
            n => n.UserId == userId && !n.IsRead,
            cancellationToken);

        if (unreadNotifications.Count == 0)
            return Result.Success();

        // Mark each as read
        foreach (var notification in unreadNotifications)
        {
            notification.MarkAsRead();
            await _notificationRepository.UpdateAsync(notification, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
