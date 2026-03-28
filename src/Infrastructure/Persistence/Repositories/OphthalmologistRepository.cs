using Domain.Entities.Users;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Ophthalmologist aggregate root.
/// </summary>
public class OphthalmologistRepository : Repository<Ophthalmologist>, IOphthalmologistRepository
{
    public OphthalmologistRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Ophthalmologist?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Certificates)
            .FirstOrDefaultAsync(o => o.UserId == userId, cancellationToken);
    }

    public async Task<Ophthalmologist?> GetByIdWithCertificatesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Certificates)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Ophthalmologist> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm = null,
        bool? isVerified = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        // Apply verification filter
        if (isVerified.HasValue)
        {
            query = query.Where(o => o.IsVerified == isVerified.Value);
        }

        // Note: searchTerm would typically search on User's FullName or Email
        // This requires joining with ApplicationUser which is in a different table
        // For now, we filter by Bio if searchTerm is provided
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(o => o.Bio != null && o.Bio.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(o => o.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Ophthalmologist>> GetVerifiedAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(o => o.IsVerified)
            .OrderByDescending(o => o.YearsOfExperience)
            .ToListAsync(cancellationToken);
    }
}
