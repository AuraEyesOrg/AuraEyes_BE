using Application.Common.Interfaces;
using Domain.Entities.Platform;
using Domain.Common;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Persists notifications to the database.
/// Can be extended to push via SignalR, FCM, etc.
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IRepository<Notification> _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IRepository<Notification> notificationRepository,
        IUnitOfWork unitOfWork,
        ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task SendAsync(Guid userId, string message, CancellationToken cancellationToken = default)
    {
        var notification = new Notification(userId, "Session Reminder", message);

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Notification sent to user {UserId}: {Message}", userId, message);
    }
}
