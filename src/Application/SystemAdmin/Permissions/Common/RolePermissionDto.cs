namespace Application.SystemAdmin.Permissions.Common;

/// <summary>DTO for a role → permission assignment.</summary>
public class RolePermissionDto
{
    public Guid RolePermissionId { get; set; }
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public Guid PermissionId { get; set; }
    public string PermissionName { get; set; } = string.Empty;
    public string PermissionDisplayName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public DateTime AssignedAt { get; set; }
}
