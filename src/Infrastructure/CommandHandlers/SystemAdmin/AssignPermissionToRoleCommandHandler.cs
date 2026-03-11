using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Permissions.Commands.AssignPermissionToRole;
using Application.SystemAdmin.Permissions.Common;
using Domain.Common;
using Domain.Entities.Authorization;
using Domain.Repositories;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Infrastructure.CommandHandlers.SystemAdmin;

/// <summary>
/// Handler in Infrastructure layer because it requires RoleManager (Identity).
/// Assigns a permission to a role — all users in that role will inherit it.
/// </summary>
public class AssignPermissionToRoleCommandHandler : ICommandHandler<AssignPermissionToRoleCommand, RolePermissionDto>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignPermissionToRoleCommandHandler> _logger;

    public AssignPermissionToRoleCommandHandler(
        IPermissionRepository permissionRepository,
        RoleManager<ApplicationRole> roleManager,
        IUnitOfWork unitOfWork,
        ILogger<AssignPermissionToRoleCommandHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<RolePermissionDto>> Handle(
        AssignPermissionToRoleCommand request,
        CancellationToken cancellationToken)
    {
        // Validate role exists
        var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());
        if (role is null)
            return Result<RolePermissionDto>.NotFound($"Role '{request.RoleId}' not found.");

        // Validate permission exists and is active
        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);
        if (permission is null)
            return Result<RolePermissionDto>.NotFound($"Permission '{request.PermissionId}' not found.");

        if (!permission.IsActive)
            return Result<RolePermissionDto>.Failure($"Permission '{permission.Name}' is inactive and cannot be assigned.");

        // Prevent duplicate assignment
        var existing = await _permissionRepository.GetRolePermissionAsync(
            request.RoleId, request.PermissionId, cancellationToken);

        if (existing is not null)
            return Result<RolePermissionDto>.Conflict(
                $"Permission '{permission.Name}' is already assigned to role '{role.Name}'.");

        var rolePermission = new RolePermission(request.RoleId, request.PermissionId);
        await _permissionRepository.AddRolePermissionAsync(rolePermission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Permission '{Permission}' assigned to role '{Role}'",
            permission.Name, role.Name);

        return Result<RolePermissionDto>.Success(new RolePermissionDto
        {
            RolePermissionId = rolePermission.Id,
            RoleId = request.RoleId,
            RoleName = role.Name ?? string.Empty,
            PermissionId = permission.Id,
            PermissionName = permission.Name,
            PermissionDisplayName = permission.DisplayName,
            Category = permission.Category,
            AssignedAt = rolePermission.CreatedAt
        });
    }
}
