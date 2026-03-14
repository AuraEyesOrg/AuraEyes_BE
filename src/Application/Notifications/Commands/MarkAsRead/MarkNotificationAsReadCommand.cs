using Application.Common.Interfaces;

namespace Application.Notifications.Commands.MarkAsRead;

/// <summary>
/// Command to mark a single notification as read
/// </summary>
public record MarkNotificationAsReadCommand : ICommand
{
    /// <summary>
    /// The notification ID to mark as read
    /// </summary>
    public Guid NotificationId { get; init; }
}
