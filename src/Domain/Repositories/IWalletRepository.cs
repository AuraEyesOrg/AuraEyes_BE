using Domain.Common;
using Domain.Entities.Financial;

namespace Domain.Repositories;

/// <summary>
/// Repository interface for Wallet aggregate root.
/// Contains domain-specific query methods beyond basic CRUD.
/// </summary>
public interface IWalletRepository : IRepository<Wallet>
{
    /// <summary>
    /// Get wallet by UserId.
    /// </summary>
    Task<Wallet?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Platform treasury wallet (<c>OwnerType == "System"</c>).
    /// </summary>
    Task<Wallet?> GetSystemWalletAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get wallet with transactions included.
    /// </summary>
    Task<Wallet?> GetByIdWithTransactionsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get wallet with transactions by UserId.
    /// </summary>
    Task<Wallet?> GetByUserIdWithTransactionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if wallet exists for a specific user.
    /// </summary>
    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Explicitly add a new WalletTransaction to the context.
    /// Use this instead of relying on navigation-property cascade
    /// to avoid EF marking new entities as Modified.
    /// </summary>
    Task AddTransactionAsync(WalletTransaction transaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get paginated transactions for a wallet.
    /// </summary>
    Task<(IReadOnlyList<WalletTransaction> Items, int TotalCount)> GetTransactionsPagedAsync(
        Guid walletId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get summary statistics for a specific month.
    /// </summary>
    Task<(decimal TotalDeposits, decimal TotalSpent, int TransactionsCount)> GetMonthlyStatsAsync(
        Guid walletId,
        int year,
        int month,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get paginated wallet transactions across all wallets for system-admin cashflow views.
    /// </summary>
    Task<(IReadOnlyList<(WalletTransaction Transaction, Wallet Wallet)> Items, int TotalCount)> GetCashflowTransactionsPagedAsync(
        string? ownerType = null,
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a salary transaction has already been processed for a user in a given month.
    /// </summary>
    Task<bool> HasSalaryBeenPaidAsync(Guid userId, int year, int month, CancellationToken cancellationToken = default);
}
