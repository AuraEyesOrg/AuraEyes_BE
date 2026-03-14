using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Permissions.Common;
using Application.SystemAdmin.Permissions.Queries.GetUserPermissions;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.QueryHandlers.SystemAdmin;

/// <summary>
/// Handles GetUserPermissionsQuery.
/// Computes effective permissions: role-based + user overrides (grant/revoke).
/// </summary>
public class GetUserPermissionsQueryHandler : IQueryHandler<GetUserPermissionsQuery, UserEffectivePermissionsDto>
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUserPermissionsQueryHandler(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<Result<UserEffectivePermissionsDto>> Handle(
        GetUserPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            return Result<UserEffectivePermissionsDto>.NotFound($"User '{request.UserId}' not found.");

        var roles = await _userManager.GetRolesAsync(user);

        // 1. Role IDs for this user
        var roleIds = await _context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == request.UserId)
            .Select(ur => ur.RoleId)
            .ToListAsync(cancellationToken);

        // 2. Permissions from those roles (distinct)
        var rolePermissions = await _context.RolePermissions
            .AsNoTracking()
            .Where(rp => roleIds.Contains(rp.RoleId) && !rp.IsDeleted)
            .Join(
                _context.Permissions.Where(p => !p.IsDeleted && p.IsActive),
                rp => rp.PermissionId,
                p => p.Id,
                (_, p) => new PermissionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    DisplayName = p.DisplayName,
                    Description = p.Description,
                    Category = p.Category,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
            .Distinct()
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Name)
            .ToListAsync(cancellationToken);

        // 3. User-specific overrides (active, non-expired)
        var userOverrides = await _context.UserPermissions
            .AsNoTracking()
            .Where(up => up.UserId == request.UserId && !up.IsDeleted)
            .Join(
                _context.Permissions.Where(p => !p.IsDeleted),
                up => up.PermissionId,
                p => p.Id,
                (up, p) => new UserPermissionDto
                {
                    UserPermissionId = up.Id,
                    UserId = up.UserId,
                    PermissionId = p.Id,
                    PermissionName = p.Name,
                    PermissionDisplayName = p.DisplayName,
                    Category = p.Category,
                    IsGranted = up.IsGranted,
                    IsActive = up.IsActive,
                    IsExpired = up.ExpiresAt != null && up.ExpiresAt < DateTime.UtcNow,
                    GrantedAt = up.GrantedAt,
                    ExpiresAt = up.ExpiresAt,
                    GrantedBy = up.GrantedBy
                })
            .OrderBy(up => up.PermissionName)
            .ToListAsync(cancellationToken);

        // 4. Compute effective permissions
        //    Start with role permissions; apply user overrides
        var effectiveNames = new HashSet<string>(
            rolePermissions.Select(p => p.Name),
            StringComparer.OrdinalIgnoreCase);

        foreach (var uo in userOverrides.Where(u => u.IsActive && !u.IsExpired))
        {
            if (uo.IsGranted)
                effectiveNames.Add(uo.PermissionName);
            else
                effectiveNames.Remove(uo.PermissionName);
        }

        return Result<UserEffectivePermissionsDto>.Success(new UserEffectivePermissionsDto
        {
            UserId = request.UserId,
            UserEmail = user.Email ?? string.Empty,
            Roles = roles.ToList(),
            RolePermissions = rolePermissions,
            UserOverrides = userOverrides,
            EffectivePermissionNames = effectiveNames.OrderBy(n => n).ToList()
        });
    }
}
