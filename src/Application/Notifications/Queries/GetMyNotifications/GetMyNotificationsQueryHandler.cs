using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Platform;
using Microsoft.EntityFrameworkCore;

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

        var baseQuery = _notificationRepository
            .Query()
            .AsNoTracking()
            .Where(n => n.UserId == userId);

        if (request.Types is { Count: > 0 })
        {
            var selectedTypes = request.Types;
            baseQuery = baseQuery.Where(n => selectedTypes.Contains(n.Type));
        }

        var totalCount = await baseQuery.CountAsync(cancellationToken);
        var unreadCount = await baseQuery.CountAsync(n => !n.IsRead, cancellationToken);

        // Calculate pagination
        var totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var maxPage = Math.Max(1, totalPages);
        var pageNumber = Math.Max(1, Math.Min(request.PageNumber, maxPage));

        // Get paginated items
        var items = await baseQuery
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                UserId = n.UserId,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                ReferenceId = n.ReferenceId,
                IsRead = n.IsRead,
                Payload = n.Payload,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync(cancellationToken);

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
