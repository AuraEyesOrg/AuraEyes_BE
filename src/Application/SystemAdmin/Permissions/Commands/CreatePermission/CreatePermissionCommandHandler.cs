using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Permissions.Common;
using Domain.Common;
using Domain.Entities.Authorization;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Permissions.Commands.CreatePermission;

public class CreatePermissionCommandHandler : ICommandHandler<CreatePermissionCommand, PermissionDto>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreatePermissionCommandHandler> _logger;

    public CreatePermissionCommandHandler(
        IPermissionRepository permissionRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreatePermissionCommandHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PermissionDto>> Handle(
        CreatePermissionCommand request,
        CancellationToken cancellationToken)
    {
        // Check uniqueness
        var existing = await _permissionRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existing is not null)
            return Result<PermissionDto>.Conflict($"Permission '{request.Name}' already exists.");

        var permission = new Permission(
            request.Name,
            request.DisplayName,
            request.Description,
            request.Category);

        await _permissionRepository.AddAsync(permission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Permission '{Name}' created with ID {Id}", permission.Name, permission.Id);

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
