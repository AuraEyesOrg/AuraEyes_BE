using Application.Common.Constants;
using Application.Common.Models;
using Application.SystemAdmin.Users.Queries.GetUserMetrics;
using Application.SystemAdmin.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.SystemAdmin;

/// <summary>
/// System Admin User & Role Management endpoints
/// Provides centralized interface to manage users, roles, permissions, and monitor user activity.
/// </summary>
[Route("api/system-admin/[controller]")]
[Authorize(Policy = Policies.SystemAdminOnly)]
public class UsersController : BaseApiController
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
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
    /// Screen: 3.5.1, 3.5.6-3.5.9 View User & Role Management Overview, Search/Filter Users
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
    /// Update user role (mock endpoint)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Role update data</param>
    /// <remarks>
    /// Screen: 3.5.11 Assign or Update User Role
    /// Note: Mock implementation - will connect to IdentityService when ready
    /// </remarks>
    [HttpPatch("{id:guid}/role")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public IActionResult UpdateUserRole(Guid id, [FromBody] UpdateUserRoleRequest request)
    {
        // Mock response
        return Ok(ApiResponseFactory.Success(new { success = true }, $"Role '{request.Role}' {(request.AddRole ? "added to" : "removed from")} user successfully"));
    }

    /// <summary>
    /// Update user status (mock endpoint)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Status update data</param>
    /// <remarks>
    /// Screen: 3.5.12 Change User Status (Activate, Suspend, Approve)
    /// Note: Mock implementation - will connect to IdentityService when ready
    /// </remarks>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public IActionResult UpdateUserStatus(Guid id, [FromBody] UpdateUserStatusRequest request)
    {
        // Mock response
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
