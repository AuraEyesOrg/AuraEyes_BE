using Application.Common.Interfaces;

namespace Application.SystemAdmin.Permissions.Commands.RemovePermissionFromRole;

/// <summary>Command: remove a permission assignment from a role.</summary>
public record RemovePermissionFromRoleCommand(Guid RolePermissionId) : ICommand;
