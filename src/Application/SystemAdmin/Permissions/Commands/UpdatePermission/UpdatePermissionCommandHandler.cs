using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Permissions.Common;
using Domain.Common;
using Domain.Entities.Authorization;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Permissions.Commands.UpdatePermission;

public class UpdatePermissionCommandHandler : ICommandHandler<UpdatePermissionCommand, PermissionDto>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePermissionCommandHandler> _logger;

    public UpdatePermissionCommandHandler(
        IPermissionRepository permissionRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdatePermissionCommandHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PermissionDto>> Handle(
        UpdatePermissionCommand request,
        CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);
        if (permission is null)
            return Result<PermissionDto>.NotFound($"Permission '{request.PermissionId}' not found.");

        permission.Update(request.DisplayName, request.Description, request.Category);

        await _permissionRepository.UpdateAsync(permission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Permission '{Name}' (ID {Id}) updated", permission.Name, permission.Id);

        return Result<PermissionDto>.Success(ToDto(permission));
    }

    private static PermissionDto ToDto(Permission p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        DisplayName = p.DisplayName,
        Description = p.Description,
        Category = p.Category,
        IsActive = p.IsActive,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt
    };
}
