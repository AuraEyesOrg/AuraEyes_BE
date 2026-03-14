using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Permissions.Commands.RemovePermissionFromRole;

public class RemovePermissionFromRoleCommandHandler : ICommandHandler<RemovePermissionFromRoleCommand>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemovePermissionFromRoleCommandHandler> _logger;

    public RemovePermissionFromRoleCommandHandler(
        IPermissionRepository permissionRepository,
        IUnitOfWork unitOfWork,
        ILogger<RemovePermissionFromRoleCommandHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(RemovePermissionFromRoleCommand request, CancellationToken cancellationToken)
    {
        var rolePermission = await _permissionRepository.GetRolePermissionByIdAsync(
            request.RolePermissionId, cancellationToken);

        if (rolePermission is null)
            return Result.NotFound($"Role-permission assignment '{request.RolePermissionId}' not found.");

        await _permissionRepository.RemoveRolePermissionAsync(rolePermission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("RolePermission {Id} removed", request.RolePermissionId);

        return Result.Success();
    }
}
