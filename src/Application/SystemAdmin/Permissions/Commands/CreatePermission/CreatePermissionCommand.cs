using Application.Common.Interfaces;
using Application.SystemAdmin.Permissions.Common;

namespace Application.SystemAdmin.Permissions.Commands.CreatePermission;

/// <summary>Command: create a new permission.</summary>
public record CreatePermissionCommand : ICommand<PermissionDto>
{
    public string Name { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Category { get; init; }
}
