namespace Application.SystemAdmin.Permissions.Common;

/// <summary>DTO for a user-specific permission override.</summary>
public class UserPermissionDto
{
    public Guid UserPermissionId { get; set; }
    public Guid UserId { get; set; }
    public Guid PermissionId { get; set; }
    public string PermissionName { get; set; } = string.Empty;
    public string PermissionDisplayName { get; set; } = string.Empty;
    public string? Category { get; set; }

    /// <summary>true = explicit grant; false = explicit revoke.</summary>
    public bool IsGranted { get; set; }

    public bool IsActive { get; set; }
    public bool IsExpired { get; set; }
    public DateTime GrantedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public Guid? GrantedBy { get; set; }
}

/// <summary>
/// Aggregated view of a user's effective permissions:
/// role-based permissions merged with user-level overrides.
/// </summary>
public class UserEffectivePermissionsDto
{
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();

    /// <summary>Permissions inherited from the user's role(s).</summary>
    public List<PermissionDto> RolePermissions { get; set; } = new();

    /// <summary>Explicit per-user overrides (grant or revoke).</summary>
    public List<UserPermissionDto> UserOverrides { get; set; } = new();

    /// <summary>Final computed set: role permissions + grants - revokes.</summary>
    public List<string> EffectivePermissionNames { get; set; } = new();
}
