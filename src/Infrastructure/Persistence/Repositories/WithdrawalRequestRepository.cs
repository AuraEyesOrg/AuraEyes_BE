using Domain.Entities.Financial;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for WithdrawalRequest aggregate root.
/// </summary>
public class WithdrawalRequestRepository : Repository<WithdrawalRequest>, IWithdrawalRequestRepository
{
    public WithdrawalRequestRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IReadOnlyList<WithdrawalRequest> Items, int TotalCount)> GetByUserIdPagedAsync(
        Guid userId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(IReadOnlyList<WithdrawalRequest> Items, int TotalCount)> GetPagedAsync(
        PaymentStatus? status = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        query = query.OrderByDescending(x => x.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> HasPendingRequestAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(
            x => x.UserId == userId && (x.Status == PaymentStatus.Pending || x.Status == PaymentStatus.Processing),
            cancellationToken);
    }
}
