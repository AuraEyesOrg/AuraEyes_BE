using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Financial;

/// <summary>
/// Wallet Transaction entity - immutable ledger entry.
/// ReferenceType (Booking | AiQuota | Payout | ...) + ReferenceId enable
/// tracing one booking event to multiple balance movements (platform split, doctor share).
/// </summary>
public class WalletTransaction : BaseEntity
{
    public Guid WalletId { get; private set; }
    public decimal Amount { get; private set; }
    public TransactionType TransactionType { get; private set; }
    public string? Description { get; private set; }

    /// <summary>"Booking" | "AiQuota" | "Payout" | "Deposit" | ...</summary>
    public string? ReferenceType { get; private set; }

    /// <summary>ID of the source entity (ScheduleId, OrderId, etc.).</summary>
    public Guid? ReferenceId { get; private set; }

    private WalletTransaction() { } // EF Core

    public WalletTransaction(Guid walletId, decimal amount, TransactionType transactionType,
        string? description = null, string? referenceType = null, Guid? referenceId = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Transaction amount must be positive", nameof(amount));

        WalletId = walletId;
        Amount = amount;
        TransactionType = transactionType;
        Description = description;
        ReferenceType = referenceType;
        ReferenceId = referenceId;
    }
}
