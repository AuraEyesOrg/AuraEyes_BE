using Application.Notifications.Commands.MarkAllAsRead;
using Application.Notifications.Commands.MarkAsRead;
using Application.Notifications.Queries.GetMyNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

/// <summary>
/// Controller for managing user notifications
/// </summary>
[Authorize]
public class NotificationsController : BaseApiController
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get paginated notifications for the current user
    /// </summary>
    /// <param name="pageNumber">Page number (1-indexed, default: 1)</param>
    /// <param name="pageSize">Number of items per page (default: 10)</param>
    /// <returns>Paginated list of notifications with unread count</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get my notifications",
        Description = "Returns paginated notifications for the authenticated user along with the total unread count")]
    [ProducesResponseType(typeof(PaginatedNotificationsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyNotifications(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetMyNotificationsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get only the unread count for notifications badge
    /// </summary>
    /// <returns>The count of unread notifications</returns>
    [HttpGet("unread-count")]
    [SwaggerOperation(
        Summary = "Get unread notification count",
        Description = "Returns only the unread notification count for badge display")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUnreadCount()
    {
        var query = new GetMyNotificationsQuery
        {
            PageNumber = 1,
            PageSize = 1
        };

        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return HandleResult(result);

        return Ok(new { UnreadCount = result.Data!.UnreadCount });
    }

    /// <summary>
    /// Mark a specific notification as read
    /// </summary>
    /// <param name="id">The notification ID</param>
    [HttpPost("{id:guid}/mark-read")]
    [SwaggerOperation(
        Summary = "Mark notification as read",
        Description = "Marks a specific notification as read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var command = new MarkNotificationAsReadCommand
        {
            NotificationId = id
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    [HttpPost("mark-all-read")]
    [SwaggerOperation(
        Summary = "Mark all notifications as read",
        Description = "Marks all unread notifications for the current user as read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var command = new MarkAllAsReadCommand();
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }
}
