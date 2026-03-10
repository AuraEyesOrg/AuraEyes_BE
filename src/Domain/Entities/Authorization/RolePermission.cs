using Domain.Common;

namespace Domain.Entities.Authorization;

/// <summary>
/// RolePermission - maps default permissions to a role (RBAC base layer).
/// </summary>
public class RolePermission : BaseEntity
{
    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }

    private RolePermission() { } // EF Core

    public RolePermission(Guid roleId, Guid permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }
}
