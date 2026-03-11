using Domain.Common;
using Domain.Entities.Authorization;

namespace Domain.Repositories;

/// <summary>
/// Repository interface for Permission aggregate.
/// Also exposes methods for managing RolePermission and UserPermission (child entities).
/// </summary>
public interface IPermissionRepository : IRepository<Permission>
{
    // ─── Permission ───────────────────────────────────────────────────────────

    /// <summary>Find permission by unique name (case-insensitive).</summary>
    Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    // ─── RolePermission ──────────────────────────────────────────────────────

    /// <summary>Get an existing role-permission assignment (null if not found).</summary>
    Task<RolePermission?> GetRolePermissionAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default);

    /// <summary>Get all permission assignments for a specific role.</summary>
    Task<IReadOnlyList<RolePermission>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>Get a single role-permission record by its own ID.</summary>
    Task<RolePermission?> GetRolePermissionByIdAsync(Guid rolePermissionId, CancellationToken cancellationToken = default);

    /// <summary>Persist a new role-permission assignment.</summary>
    Task<RolePermission> AddRolePermissionAsync(RolePermission rolePermission, CancellationToken cancellationToken = default);

    /// <summary>Delete a role-permission assignment (hard delete — it's a join record).</summary>
    Task RemoveRolePermissionAsync(RolePermission rolePermission, CancellationToken cancellationToken = default);

    // ─── UserPermission ──────────────────────────────────────────────────────

    /// <summary>Get an existing user-permission override (null if not found).</summary>
    Task<UserPermission?> GetUserPermissionAsync(Guid userId, Guid permissionId, CancellationToken cancellationToken = default);

    /// <summary>Get a single user-permission record by its own ID.</summary>
    Task<UserPermission?> GetUserPermissionByIdAsync(Guid userPermissionId, CancellationToken cancellationToken = default);

    /// <summary>Get all user-specific permission overrides for a user.</summary>
    Task<IReadOnlyList<UserPermission>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Get all role assignments for a user (via ASP.NET Identity UserRoles).</summary>
    Task<IReadOnlyList<Guid>> GetUserRoleIdsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Persist a new user-permission override.</summary>
    Task<UserPermission> AddUserPermissionAsync(UserPermission userPermission, CancellationToken cancellationToken = default);

    /// <summary>Update an existing user-permission record.</summary>
    Task UpdateUserPermissionAsync(UserPermission userPermission, CancellationToken cancellationToken = default);
}
