using Application.Common.Interfaces;

namespace Application.SystemAdmin.Permissions.Commands.RevokeUserPermission;

/// <summary>Command: revoke (deactivate) a user-specific permission override.</summary>
public record RevokeUserPermissionCommand(Guid UserPermissionId) : ICommand;
