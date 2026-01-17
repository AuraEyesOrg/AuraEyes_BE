using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class OphthalmologistRepository : Repository<Ophthalmologist>, IOphthalmologistRepository
{
    public OphthalmologistRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Ophthalmologist?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(o => o.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Ophthalmologist>> GetVerifiedOphthalmologistsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(o => o.IsVerified)
            .OrderByDescending(o => o.YearsOfExperience)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Ophthalmologist>> GetByExperienceRangeAsync(int minYears, int maxYears, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(o => o.YearsOfExperience >= minYears && o.YearsOfExperience <= maxYears)
            .OrderByDescending(o => o.YearsOfExperience)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(o => o.UserId == userId, cancellationToken);
    }
}
