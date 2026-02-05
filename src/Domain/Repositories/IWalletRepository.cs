using Domain.Common;
using Domain.Entities;

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
    /// Get paginated transactions for a wallet.
    /// </summary>
    Task<(IReadOnlyList<WalletTransaction> Items, int TotalCount)> GetTransactionsPagedAsync(
        Guid walletId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
}
