using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Permissions.Common;

namespace Application.SystemAdmin.Permissions.Queries.GetRolePermissions;

/// <summary>Query: get all permissions assigned to a role.</summary>
public record GetRolePermissionsQuery(Guid RoleId) : IQuery<List<RolePermissionDto>>;
