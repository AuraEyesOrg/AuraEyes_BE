using Application.Common.Interfaces;

namespace Application.Notifications.Commands.MarkAllAsRead;

/// <summary>
/// Command to mark all notifications as read for the current user
/// </summary>
public record MarkAllAsReadCommand : ICommand;
