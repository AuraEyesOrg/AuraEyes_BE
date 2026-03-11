using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Permissions.Commands.DeletePermission;

public class DeletePermissionCommandHandler : ICommandHandler<DeletePermissionCommand>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletePermissionCommandHandler> _logger;

    public DeletePermissionCommandHandler(
        IPermissionRepository permissionRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeletePermissionCommandHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeletePermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);
        if (permission is null)
            return Result.NotFound($"Permission '{request.PermissionId}' not found.");

        // Soft-delete via domain method
        permission.Deactivate();

        await _permissionRepository.UpdateAsync(permission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Permission '{Name}' (ID {Id}) deactivated", permission.Name, permission.Id);

        return Result.Success();
    }
}
