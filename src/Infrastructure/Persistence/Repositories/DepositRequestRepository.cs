using Domain.Entities.Financial;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for DepositRequest aggregate root.
/// </summary>
public class DepositRequestRepository : Repository<DepositRequest>, IDepositRequestRepository
{
    public DepositRequestRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<DepositRequest?> GetByOrderCodeAsync(string orderCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(d => d.Wallet)
            .FirstOrDefaultAsync(d => d.PaymentOrderCode == orderCode, cancellationToken);
    }

    public async Task<(IReadOnlyList<DepositRequest> Items, int TotalCount)> GetByUserIdPagedAsync(
        Guid userId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<DepositRequest>> GetByStatusAsync(
        PaymentStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(d => d.Status == status)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DepositRequest>> GetExpiredPendingRequestsAsync(
        TimeSpan olderThan,
        CancellationToken cancellationToken = default)
    {
        var cutoffTime = DateTime.UtcNow - olderThan;

        return await _dbSet
            .Where(d => d.Status == PaymentStatus.Pending && d.CreatedAt < cutoffTime)
            .ToListAsync(cancellationToken);
    }
}
