using Application.Common.Interfaces;
using Application.SystemAdmin.Permissions.Common;

namespace Application.SystemAdmin.Permissions.Commands.GrantPermissionToUser;

/// <summary>
/// Command: grant or explicitly revoke a permission for a specific user.
/// IsGranted = true  → grant extra permission on top of role.
/// IsGranted = false → explicitly revoke a permission the role already has.
/// </summary>
public record GrantPermissionToUserCommand : ICommand<UserPermissionDto>
{
    public Guid UserId { get; init; }
    public Guid PermissionId { get; init; }
    public bool IsGranted { get; init; } = true;
    public DateTime? ExpiresAt { get; init; }
}
