using Domain.Entities.Financial;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Wallet aggregate root.
/// </summary>
public class WalletRepository : Repository<Wallet>, IWalletRepository
{
    public WalletRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Wallet?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);
    }

    public async Task<Wallet?> GetSystemWalletAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(w => w.OwnerType == "System", cancellationToken);
    }

    public async Task<Wallet?> GetByIdWithTransactionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(w => w.Transactions)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<Wallet?> GetByUserIdWithTransactionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(w => w.Transactions)
            .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);
    }

    public async Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(w => w.UserId == userId, cancellationToken);
    }

    public async Task AddTransactionAsync(WalletTransaction transaction, CancellationToken cancellationToken = default)
    {
        await _context.Set<WalletTransaction>().AddAsync(transaction, cancellationToken);
    }

    public async Task<(IReadOnlyList<WalletTransaction> Items, int TotalCount)> GetTransactionsPagedAsync(
        Guid walletId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _context.WalletTransactions
            .Where(t => t.WalletId == walletId)
            .OrderByDescending(t => t.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(decimal TotalDeposits, decimal TotalSpent, int TransactionsCount)> GetMonthlyStatsAsync(
        Guid walletId,
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddMonths(1);

        var monthTransactions = await _context.WalletTransactions
            .Where(t => t.WalletId == walletId && t.CreatedAt >= startDate && t.CreatedAt < endDate)
            .ToListAsync(cancellationToken);

        var transactionsCount = monthTransactions.Count;

        // Deposits, refunds, bonuses, and transfers (e.g. consultation credit to doctor) are positive inflow
        var totalDeposits = monthTransactions
            .Where(t => t.TransactionType == Domain.Enums.TransactionType.Deposit ||
                        t.TransactionType == Domain.Enums.TransactionType.Refund ||
                        t.TransactionType == Domain.Enums.TransactionType.Bonus ||
                        t.TransactionType == Domain.Enums.TransactionType.Transfer)
            .Sum(t => t.Amount);

        // Payments, Withdrawals are negative flow
        var totalSpent = monthTransactions
            .Where(t => t.TransactionType == Domain.Enums.TransactionType.Payment ||
                        t.TransactionType == Domain.Enums.TransactionType.Withdrawal)
            .Sum(t => t.Amount);

        return (totalDeposits, totalSpent, transactionsCount);
    }
}
