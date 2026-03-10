using Application.Common.Interfaces;
using Application.SystemAdmin.Permissions.Common;

namespace Application.SystemAdmin.Permissions.Commands.UpdatePermission;

/// <summary>Command: update an existing permission's metadata.</summary>
public record UpdatePermissionCommand : ICommand<PermissionDto>
{
    public Guid PermissionId { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Category { get; init; }
}
