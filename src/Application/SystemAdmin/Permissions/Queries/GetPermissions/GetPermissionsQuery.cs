using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Permissions.Common;

namespace Application.SystemAdmin.Permissions.Queries.GetPermissions;

/// <summary>
/// Query: list all permissions with optional filtering and pagination.
/// </summary>
public record GetPermissionsQuery : IQuery<PagedResult<PermissionDto>>
{
    public string? SearchTerm { get; init; }
    public string? Category { get; init; }
    public bool? IsActive { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
