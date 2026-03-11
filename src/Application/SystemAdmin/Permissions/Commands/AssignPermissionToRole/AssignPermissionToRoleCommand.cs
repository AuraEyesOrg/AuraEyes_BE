using Application.Common.Interfaces;
using Application.SystemAdmin.Permissions.Common;

namespace Application.SystemAdmin.Permissions.Commands.AssignPermissionToRole;

/// <summary>Command: assign a permission to an Identity role.</summary>
public record AssignPermissionToRoleCommand : ICommand<RolePermissionDto>
{
    public Guid RoleId { get; init; }
    public Guid PermissionId { get; init; }
}
