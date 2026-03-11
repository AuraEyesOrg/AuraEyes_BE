using Application.Common.Interfaces;

namespace Application.SystemAdmin.Permissions.Commands.DeletePermission;

/// <summary>
/// Command: soft-delete (deactivate) a permission.
/// The permission record is kept for audit; IsActive becomes false.
/// </summary>
public record DeletePermissionCommand(Guid PermissionId) : ICommand;
