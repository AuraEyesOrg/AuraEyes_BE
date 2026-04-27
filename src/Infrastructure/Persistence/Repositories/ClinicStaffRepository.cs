using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for <see cref="ClinicStaff"/> aggregate root.
/// Inherits GetByIdAsync, AddAsync, UpdateAsync, DeleteAsync, GetAllAsync from Repository&lt;T&gt;.
/// </summary>
public class ClinicStaffRepository : Repository<ClinicStaff>, IClinicStaffRepository
{
    public ClinicStaffRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ClinicStaff?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<ClinicStaff>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.IsActive)
            .OrderBy(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<ClinicStaff>> GetBySubRoleAsync(
        ClinicStaffRole subRole,
        CancellationToken cancellationToken = default)
    {
        var roleName = subRole.ToString();
        return await _dbSet
            .Where(s => s.IsActive && s.SubRoles.Contains(roleName))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(s => s.UserId == userId, cancellationToken);
    }

    /// <inheritdoc />
    public void Remove(ClinicStaff clinicStaff)
    {
        _dbSet.Remove(clinicStaff);
    }
}
