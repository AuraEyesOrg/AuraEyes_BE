using Application.Common.Constants;
using Application.Common.Models;
using Application.SystemAdmin.Permissions.Commands.AssignPermissionToRole;
using Application.SystemAdmin.Permissions.Commands.CreatePermission;
using Application.SystemAdmin.Permissions.Commands.DeletePermission;
using Application.SystemAdmin.Permissions.Commands.GrantPermissionToUser;
using Application.SystemAdmin.Permissions.Commands.RemovePermissionFromRole;
using Application.SystemAdmin.Permissions.Commands.RevokeUserPermission;
using Application.SystemAdmin.Permissions.Commands.UpdatePermission;
using Application.SystemAdmin.Permissions.Common;
using Application.SystemAdmin.Permissions.Queries.GetPermissionById;
using Application.SystemAdmin.Permissions.Queries.GetPermissions;
using Application.SystemAdmin.Permissions.Queries.GetRolePermissions;
using Application.SystemAdmin.Permissions.Queries.GetUserPermissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.SystemAdmin;

/// <summary>
/// Permission Management — CRUD for permissions, role assignments, and user overrides.
/// All endpoints require SystemAdmin role.
/// 
/// Authorization model:
///   Permissions    → granular capability flags (e.g. "users:read", "screening:approve")
///   RolePermission → default permissions inherited by every user in a role (RBAC base)
///   UserPermission → per-user overrides: grant extras OR explicitly revoke role permissions
/// </summary>
[Route("api/system-admin/[controller]")]
[Authorize(Policy = Policies.SystemAdminOnly)]
public class PermissionsController : BaseApiController
{
    private readonly IMediator _mediator;

    public PermissionsController(IMediator mediator) => _mediator = mediator;

    // =========================================================================
    // PERMISSIONS (CRUD)
    // =========================================================================

    /// <summary>List all permissions with optional search/filter and pagination.</summary>
    /// <param name="searchTerm">Search by name, displayName, or description.</param>
    /// <param name="category">Filter by category (exact match, case-insensitive).</param>
    /// <param name="isActive">Filter by active status.</param>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 20, max: 100).</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PermissionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPermissions(
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? category = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetPermissionsQuery
        {
            SearchTerm = searchTerm,
            Category = category,
            IsActive = isActive,
            PageNumber = pageNumber,
            PageSize = Math.Min(pageSize, 100)
        };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>Get a single permission by ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PermissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPermissionById(Guid id)
    {
        var result = await _mediator.Send(new GetPermissionByIdQuery(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new permission.
    /// Name must be unique and follow the pattern: letters, digits, underscores, dots, colons
    /// (e.g. "users:read", "screening.approve").
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PermissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result, "Permission created successfully.");
    }

    /// <summary>
    /// Update a permission's display name, description, and category.
    /// The permission name (unique key) cannot be changed.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PermissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePermission(Guid id, [FromBody] UpdatePermissionRequest request)
    {
        var command = new UpdatePermissionCommand
        {
            PermissionId = id,
            DisplayName = request.DisplayName,
            Description = request.Description,
            Category = request.Category
        };
        var result = await _mediator.Send(command);
        return HandleResult(result, "Permission updated successfully.");
    }

    /// <summary>
    /// Deactivate (soft-delete) a permission.
    /// Deactivated permissions remain in the database for audit but cannot be assigned.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePermission(Guid id)
    {
        var result = await _mediator.Send(new DeletePermissionCommand(id));
        return HandleResult(result, "Permission deactivated successfully.");
    }

    // =========================================================================
    // ROLE PERMISSIONS
    // =========================================================================

    /// <summary>
    /// Get all permissions assigned to an Identity role.
    /// Use the role's GUID ID (visible in GET /api/system-admin/users).
    /// </summary>
    [HttpGet("roles/{roleId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<RolePermissionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRolePermissions(Guid roleId)
    {
        var result = await _mediator.Send(new GetRolePermissionsQuery(roleId));
        return HandleResult(result);
    }

    /// <summary>
    /// Assign a permission to a role.
    /// Every user in this role will inherit the permission.
    /// </summary>
    [HttpPost("roles")]
    [ProducesResponseType(typeof(ApiResponse<RolePermissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AssignPermissionToRole([FromBody] AssignPermissionToRoleCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result, "Permission assigned to role successfully.");
    }

    /// <summary>
    /// Remove a permission from a role.
    /// Pass the <c>rolePermissionId</c> returned by GET /roles/{roleId}.
    /// </summary>
    [HttpDelete("roles/{rolePermissionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemovePermissionFromRole(Guid rolePermissionId)
    {
        var result = await _mediator.Send(new RemovePermissionFromRoleCommand(rolePermissionId));
        return HandleResult(result, "Permission removed from role successfully.");
    }

    // =========================================================================
    // USER PERMISSIONS (per-user overrides)
    // =========================================================================

    /// <summary>
    /// Get effective permissions for a specific user.
    /// Returns role-based permissions, per-user overrides, AND the final computed set.
    /// </summary>
    [HttpGet("users/{userId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserEffectivePermissionsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserPermissions(Guid userId)
    {
        var result = await _mediator.Send(new GetUserPermissionsQuery(userId));
        return HandleResult(result);
    }

    /// <summary>
    /// Grant or explicitly revoke a permission for a specific user.
    /// <list type="bullet">
    ///   <item><description><c>isGranted: true</c> — adds a permission the user's role doesn't have.</description></item>
    ///   <item><description><c>isGranted: false</c> — removes a permission the user would inherit from their role.</description></item>
    /// </list>
    /// </summary>
    [HttpPost("users")]
    [ProducesResponseType(typeof(ApiResponse<UserPermissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> GrantPermissionToUser([FromBody] GrantPermissionToUserCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result, "User permission override created successfully.");
    }

    /// <summary>
    /// Revoke a per-user permission override.
    /// Pass the <c>userPermissionId</c> returned by GET /users/{userId}.
    /// The user will fall back to their role-based permissions.
    /// </summary>
    [HttpPatch("users/{userPermissionId:guid}/revoke")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RevokeUserPermission(Guid userPermissionId)
    {
        var result = await _mediator.Send(new RevokeUserPermissionCommand(userPermissionId));
        return HandleResult(result, "User permission override revoked successfully.");
    }
}

/// <summary>Request body for PUT /permissions/{id}.</summary>
public record UpdatePermissionRequest(
    string DisplayName,
    string? Description,
    string? Category);
