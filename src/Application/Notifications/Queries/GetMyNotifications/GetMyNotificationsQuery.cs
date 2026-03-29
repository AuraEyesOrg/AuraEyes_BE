using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enums;

namespace Application.Notifications.Queries.GetMyNotifications;

/// <summary>
/// Query to get paginated notifications for the current user
/// </summary>
public record GetMyNotificationsQuery : IQuery<PaginatedNotificationsResponse>
{
    /// <summary>
    /// Page number (1-indexed)
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Optional notification type filters (OR condition)
    /// </summary>
    public IReadOnlyCollection<NotificationType>? Types { get; init; }
}

/// <summary>
/// Response containing paginated notifications and unread count
/// </summary>
public record PaginatedNotificationsResponse
{
    /// <summary>
    /// List of notifications for the current page
    /// </summary>
    public IReadOnlyList<NotificationDto> Items { get; init; } = [];

    /// <summary>
    /// Total count of unread notifications
    /// </summary>
    public int UnreadCount { get; init; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; init; }

    /// <summary>
    /// Total count of all notifications
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Whether there's a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Whether there's a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}
