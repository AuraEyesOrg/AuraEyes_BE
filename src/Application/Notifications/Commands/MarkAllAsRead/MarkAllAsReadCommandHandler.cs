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

    public MarkAllAsReadCommandHandler(
        IRepository<Notification> notificationRepository,
        ICurrentUserService currentUser)
    {
        _notificationRepository = notificationRepository;
        _currentUser = currentUser;
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

        return Result.Success();
    }
}
