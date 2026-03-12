using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Platform;

namespace Application.Notifications.Queries.GetMyNotifications;

/// <summary>
/// Handler for GetMyNotificationsQuery
/// </summary>
public class GetMyNotificationsQueryHandler : IQueryHandler<GetMyNotificationsQuery, PaginatedNotificationsResponse>
{
    private readonly IRepository<Notification> _notificationRepository;
    private readonly ICurrentUserService _currentUser;

    public GetMyNotificationsQueryHandler(
        IRepository<Notification> notificationRepository,
        ICurrentUserService currentUser)
    {
        _notificationRepository = notificationRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<PaginatedNotificationsResponse>> Handle(
        GetMyNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Result<PaginatedNotificationsResponse>.Unauthorized("User is not authenticated.");

        var userId = _currentUser.UserId.Value;

        // Get all notifications for user
        var allNotifications = await _notificationRepository.FindAsync(
            n => n.UserId == userId, 
            cancellationToken);

        // Order by CreatedAt descending
        var orderedNotifications = allNotifications
            .OrderByDescending(n => n.CreatedAt)
            .ToList();

        // Get total count
        var totalCount = orderedNotifications.Count;

        // Get unread count
        var unreadCount = orderedNotifications.Count(n => !n.IsRead);

        // Calculate pagination
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
        var pageNumber = Math.Max(1, Math.Min(request.PageNumber, Math.Max(1, totalPages)));

        // Get paginated items
        var items = orderedNotifications
            .Skip((pageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                UserId = n.UserId,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                IsRead = n.IsRead,
                Payload = n.Payload,
                CreatedAt = n.CreatedAt
            })
            .ToList();

        return Result<PaginatedNotificationsResponse>.Success(new PaginatedNotificationsResponse
        {
            Items = items,
            UnreadCount = unreadCount,
            PageNumber = pageNumber,
            TotalPages = totalPages,
            TotalCount = totalCount
        });
    }
}
