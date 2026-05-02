using Domain.Entities.Platform;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class LeavePolicyRepository : Repository<LeavePolicy>, ILeavePolicyRepository
{
    public LeavePolicyRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IReadOnlyList<LeavePolicy> Items, int TotalCount)> GetPagedAsync(
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.OrderByDescending(p => p.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
