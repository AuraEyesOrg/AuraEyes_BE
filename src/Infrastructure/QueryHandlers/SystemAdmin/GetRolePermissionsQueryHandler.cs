using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Permissions.Common;
using Application.SystemAdmin.Permissions.Queries.GetRolePermissions;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.QueryHandlers.SystemAdmin;

/// <summary>Handles GetRolePermissionsQuery — returns all permissions assigned to a role.</summary>
public class GetRolePermissionsQueryHandler : IQueryHandler<GetRolePermissionsQuery, List<RolePermissionDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public GetRolePermissionsQueryHandler(
        ApplicationDbContext context,
        RoleManager<ApplicationRole> roleManager)
    {
        _context = context;
        _roleManager = roleManager;
    }

    public async Task<Result<List<RolePermissionDto>>> Handle(
        GetRolePermissionsQuery request,
        CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());
        if (role is null)
            return Result<List<RolePermissionDto>>.NotFound($"Role '{request.RoleId}' not found.");

        var items = await _context.RolePermissions
            .AsNoTracking()
            .Where(rp => rp.RoleId == request.RoleId && !rp.IsDeleted)
            .Join(
                _context.Permissions.Where(p => !p.IsDeleted),
                rp => rp.PermissionId,
                p => p.Id,
                (rp, p) => new RolePermissionDto
                {
                    RolePermissionId = rp.Id,
                    RoleId = rp.RoleId,
                    RoleName = role.Name ?? string.Empty,
                    PermissionId = p.Id,
                    PermissionName = p.Name,
                    PermissionDisplayName = p.DisplayName,
                    Category = p.Category,
                    AssignedAt = rp.CreatedAt
                })
            .OrderBy(x => x.Category)
            .ThenBy(x => x.PermissionName)
            .ToListAsync(cancellationToken);

        return Result<List<RolePermissionDto>>.Success(items);
    }
}
