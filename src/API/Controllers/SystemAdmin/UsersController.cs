using Application.Common.Constants;
using Application.Common.Models;
using Application.SystemAdmin.Users.Queries.GetUserMetrics;
using Application.SystemAdmin.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Identity.Authorization;

namespace API.Controllers.SystemAdmin;

/// <summary>
/// System Admin User and Role Management endpoints
/// Provides centralized interface to manage users, roles, permissions, and monitor user activity.
/// </summary>
[Route("api/system-admin/[controller]")]
[AuthorizePermission(Permissions.UsersRead)]
public class UsersController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(IMediator mediator, UserManager<ApplicationUser> userManager)
    {
        _mediator = mediator;
        _userManager = userManager;
    }

    /// <summary>
    /// Get user management metrics
    /// </summary>
    /// <remarks>
    /// Screen: 3.5.2-3.5.5 View Total Users, Active Doctors, Patients Screened, Pending Approvals
    /// </remarks>
    [HttpGet("metrics")]
    [ProducesResponseType(typeof(ApiResponse<UserMetricsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMetrics()
    {
        var result = await _mediator.Send(new GetUserMetricsQuery());
        return HandleResult(result);
    }

    /// <summary>
    /// Get users with pagination and filtering
    /// </summary>
    /// <param name="searchTerm">Search by name, email, or username</param>
    /// <param name="role">Filter by role</param>
    /// <param name="status">Filter by status (Active, Pending, Suspended)</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <remarks>
    /// Screen: 3.5.1, 3.5.6-3.5.9 View User and Role Management Overview, Search/Filter Users
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<UserListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? role = null,
        [FromQuery] string? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetUsersQuery
        {
            SearchTerm = searchTerm,
            RoleFilter = role,
            StatusFilter = status,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Update user role
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Role update data</param>
    [HttpPatch("{id:guid}/role")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] UpdateUserRoleRequest request)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null || user.IsDeleted)
        {
            return NotFound(ApiResponseFactory.NotFound("User not found"));
        }

        if (request.AddRole)
        {
            var result = await _userManager.AddToRoleAsync(user, request.Role);
            if (!result.Succeeded)
                return BadRequest(ApiResponseFactory.Error("Failed to add role", result.Errors.Select(e => e.Description).ToList()));
        }
        else
        {
            var result = await _userManager.RemoveFromRoleAsync(user, request.Role);
            if (!result.Succeeded)
                return BadRequest(ApiResponseFactory.Error("Failed to remove role", result.Errors.Select(e => e.Description).ToList()));
        }

        return Ok(ApiResponseFactory.Success(new { success = true },
            $"Role '{request.Role}' {(request.AddRole ? "added to" : "removed from")} user successfully"));
    }

    /// <summary>
    /// Update user status (activate, suspend, etc.)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Status update data</param>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUserStatus(Guid id, [FromBody] UpdateUserStatusRequest request)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null || user.IsDeleted)
        {
            return NotFound(ApiResponseFactory.NotFound("User not found"));
        }

        switch (request.Action.ToLower())
        {
            case "activate":
                user.IsActive = true;
                break;
            case "suspend":
                user.IsActive = false;
                break;
            case "delete":
                user.IsDeleted = true;
                user.DeletedAt = DateTime.UtcNow;
                break;
            default:
                return BadRequest(ApiResponseFactory.Error($"Unknown action: {request.Action}"));
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return Ok(ApiResponseFactory.Success(new { success = true }, $"User {request.Action} successfully"));
    }
}

/// <summary>
/// Request model for updating user role
/// </summary>
public class UpdateUserRoleRequest
{
    public string Role { get; set; } = string.Empty;
    public bool AddRole { get; set; } = true;
}

/// <summary>
/// Request model for updating user status
/// </summary>
public class UpdateUserStatusRequest
{
    public string Action { get; set; } = string.Empty;
    public string? Reason { get; set; }
}
