using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Permissions.Commands.RevokeUserPermission;

public class RevokeUserPermissionCommandHandler : ICommandHandler<RevokeUserPermissionCommand>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RevokeUserPermissionCommandHandler> _logger;

    public RevokeUserPermissionCommandHandler(
        IPermissionRepository permissionRepository,
        IUnitOfWork unitOfWork,
        ILogger<RevokeUserPermissionCommandHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(RevokeUserPermissionCommand request, CancellationToken cancellationToken)
    {
        var userPermission = await _permissionRepository.GetUserPermissionByIdAsync(
            request.UserPermissionId, cancellationToken);

        if (userPermission is null)
            return Result.NotFound($"User-permission override '{request.UserPermissionId}' not found.");

        if (!userPermission.IsActive)
            return Result.Conflict("This permission override is already revoked.");

        userPermission.Revoke();

        await _permissionRepository.UpdateUserPermissionAsync(userPermission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("UserPermission {Id} revoked for user {UserId}",
            userPermission.Id, userPermission.UserId);

        return Result.Success();
    }
}
