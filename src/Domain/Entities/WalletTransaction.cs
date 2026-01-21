using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Wallet Transaction entity - transaction history for wallet
/// </summary>
public class WalletTransaction : BaseEntity
{
    public Guid WalletId { get; private set; }
    public decimal Amount { get; private set; }
    public TransactionType TransactionType { get; private set; }
    public string? Description { get; private set; }

    private WalletTransaction() { } // EF Core

    public WalletTransaction(Guid walletId, decimal amount, TransactionType transactionType, string? description = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Transaction amount must be positive", nameof(amount));

        WalletId = walletId;
        Amount = amount;
        TransactionType = transactionType;
        Description = description;
    }
}
