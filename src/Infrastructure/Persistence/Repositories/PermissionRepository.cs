using Domain.Common;
using Domain.Entities.Authorization;
using Domain.Repositories;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Concrete implementation of IPermissionRepository.
/// Extends the generic Repository for Permission (aggregate root)
/// and adds RolePermission + UserPermission management helpers.
/// </summary>
public class PermissionRepository : Repository<Permission>, IPermissionRepository
{
    public PermissionRepository(ApplicationDbContext context) : base(context) { }

    // ─── Permission ───────────────────────────────────────────────────────────

    public async Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    // ─── RolePermission ──────────────────────────────────────────────────────

    public async Task<RolePermission?> GetRolePermissionAsync(
        Guid roleId,
        Guid permissionId,
        CancellationToken cancellationToken = default)
    {
        return await _context.RolePermissions
            .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId, cancellationToken);
    }

    public async Task<IReadOnlyList<RolePermission>> GetRolePermissionsAsync(
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        return await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<RolePermission?> GetRolePermissionByIdAsync(
        Guid rolePermissionId,
        CancellationToken cancellationToken = default)
    {
        return await _context.RolePermissions
            .FirstOrDefaultAsync(rp => rp.Id == rolePermissionId, cancellationToken);
    }

    public async Task<RolePermission> AddRolePermissionAsync(
        RolePermission rolePermission,
        CancellationToken cancellationToken = default)
    {
        await _context.RolePermissions.AddAsync(rolePermission, cancellationToken);
        return rolePermission;
    }

    public Task RemoveRolePermissionAsync(
        RolePermission rolePermission,
        CancellationToken cancellationToken = default)
    {
        _context.RolePermissions.Remove(rolePermission);
        return Task.CompletedTask;
    }

    // ─── UserPermission ──────────────────────────────────────────────────────

    public async Task<UserPermission?> GetUserPermissionAsync(
        Guid userId,
        Guid permissionId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserPermissions
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId, cancellationToken);
    }

    public async Task<UserPermission?> GetUserPermissionByIdAsync(
        Guid userPermissionId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserPermissions
            .FirstOrDefaultAsync(up => up.Id == userPermissionId, cancellationToken);
    }

    public async Task<IReadOnlyList<UserPermission>> GetUserPermissionsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserPermissions
            .Where(up => up.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Guid>> GetUserRoleIdsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserPermission> AddUserPermissionAsync(
        UserPermission userPermission,
        CancellationToken cancellationToken = default)
    {
        await _context.UserPermissions.AddAsync(userPermission, cancellationToken);
        return userPermission;
    }

    public Task UpdateUserPermissionAsync(
        UserPermission userPermission,
        CancellationToken cancellationToken = default)
    {
        _context.UserPermissions.Update(userPermission);
        return Task.CompletedTask;
    }
}
