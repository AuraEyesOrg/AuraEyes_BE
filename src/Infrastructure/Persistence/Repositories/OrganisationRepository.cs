using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Organisation aggregate root.
/// </summary>
public class OrganisationRepository : Repository<Organisation>, IOrganisationRepository
{
    public OrganisationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Organisation?> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(o => o.OwnerId == ownerId && !o.IsDeleted, cancellationToken);
    }

    public async Task<(IReadOnlyList<Organisation> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm = null,
        OrgType? orgType = null,
        bool? isActive = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(o => !o.IsDeleted);

        // Apply search filter on Name, Address, or LicenseNumber
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(o =>
                o.Name.ToLower().Contains(term) ||
                (o.Address != null && o.Address.ToLower().Contains(term)) ||
                (o.LicenseNumber != null && o.LicenseNumber.ToLower().Contains(term)));
        }

        // Apply organisation type filter
        if (orgType.HasValue)
        {
            query = query.Where(o => o.OrgType == orgType.Value);
        }

        // Apply active status filter (using IsDeleted as active indicator)
        if (isActive.HasValue)
        {
            query = query.Where(o => o.IsDeleted == !isActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> ExistsByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(o => o.OwnerId == ownerId && !o.IsDeleted, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(o => o.Name.ToLower() == name.ToLower() && !o.IsDeleted);
        
        if (excludeId.HasValue)
        {
            query = query.Where(o => o.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> ExistsByLicenseNumberAsync(string licenseNumber, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(o => o.LicenseNumber != null && 
                                       o.LicenseNumber.ToLower() == licenseNumber.ToLower() && 
                                       !o.IsDeleted);
        
        if (excludeId.HasValue)
        {
            query = query.Where(o => o.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Organisation>> GetByOrgTypeAsync(OrgType orgType, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(o => o.OrgType == orgType && !o.IsDeleted)
            .OrderBy(o => o.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<OrgType, int>> GetCountByOrgTypeAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(o => !o.IsDeleted)
            .GroupBy(o => o.OrgType)
            .Select(g => new { OrgType = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.OrgType, x => x.Count, cancellationToken);
    }

    public async Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(o => !o.IsDeleted, cancellationToken);
    }
}
