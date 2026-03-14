using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Permissions.Commands.GrantPermissionToUser;
using Application.SystemAdmin.Permissions.Common;
using Domain.Common;
using Domain.Entities.Authorization;
using Domain.Repositories;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Infrastructure.CommandHandlers.SystemAdmin;

/// <summary>
/// Handler in Infrastructure layer because it requires UserManager (Identity).
/// Creates a per-user permission override (grant or explicit revoke).
/// </summary>
public class GrantPermissionToUserCommandHandler : ICommandHandler<GrantPermissionToUserCommand, UserPermissionDto>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GrantPermissionToUserCommandHandler> _logger;

    public GrantPermissionToUserCommandHandler(
        IPermissionRepository permissionRepository,
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        ILogger<GrantPermissionToUserCommandHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _userManager = userManager;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<UserPermissionDto>> Handle(
        GrantPermissionToUserCommand request,
        CancellationToken cancellationToken)
    {
        // Validate user exists
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            return Result<UserPermissionDto>.NotFound($"User '{request.UserId}' not found.");

        // Validate permission exists and is active
        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);
        if (permission is null)
            return Result<UserPermissionDto>.NotFound($"Permission '{request.PermissionId}' not found.");

        if (!permission.IsActive)
            return Result<UserPermissionDto>.Failure($"Permission '{permission.Name}' is inactive.");

        // Validate expiry
        if (request.ExpiresAt.HasValue && request.ExpiresAt.Value <= DateTime.UtcNow)
            return Result<UserPermissionDto>.Failure("Expiry date must be in the future.");

        // Prevent duplicate override
        var existing = await _permissionRepository.GetUserPermissionAsync(
            request.UserId, request.PermissionId, cancellationToken);

        if (existing is not null)
            return Result<UserPermissionDto>.Conflict(
                $"A permission override for '{permission.Name}' already exists for this user. " +
                "Revoke it first before creating a new one.");

        var userPermission = new UserPermission(
            request.UserId,
            request.PermissionId,
            request.IsGranted,
            grantedBy: _currentUserService.UserId,
            expiresAt: request.ExpiresAt.HasValue
                ? DateTime.SpecifyKind(request.ExpiresAt.Value, DateTimeKind.Utc)
                : null);

        await _permissionRepository.AddUserPermissionAsync(userPermission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Permission '{Permission}' {Action} for user {UserId} by {AdminId}",
            permission.Name,
            request.IsGranted ? "granted" : "explicitly revoked",
            request.UserId,
            _currentUserService.UserId);

        return Result<UserPermissionDto>.Success(new UserPermissionDto
        {
            UserPermissionId = userPermission.Id,
            UserId = userPermission.UserId,
            PermissionId = permission.Id,
            PermissionName = permission.Name,
            PermissionDisplayName = permission.DisplayName,
            Category = permission.Category,
            IsGranted = userPermission.IsGranted,
            IsActive = userPermission.IsActive,
            IsExpired = userPermission.IsExpired,
            GrantedAt = userPermission.GrantedAt,
            ExpiresAt = userPermission.ExpiresAt,
            GrantedBy = userPermission.GrantedBy
        });
    }
}
