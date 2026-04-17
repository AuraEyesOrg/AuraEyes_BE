using Domain.Common;

namespace Domain.Entities.Financial;

/// <summary>
/// Wallet entity - digital wallet for any actor in the system.
/// OwnerType distinguishes Patient | Ophthalmologist | Organisation | System wallets.
/// </summary>
public class Wallet : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }

    /// <summary>"Patient" | "Ophthalmologist" | "Organisation" | "System"</summary>
    public string OwnerType { get; private set; } = string.Empty;

    public decimal Balance { get; private set; }

    // Navigation properties
    private readonly List<WalletTransaction> _transactions = new();
    public IReadOnlyCollection<WalletTransaction> Transactions => _transactions.AsReadOnly();

    private Wallet() { } // EF Core

    public Wallet(Guid userId, string ownerType, decimal initialBalance = 0)
    {
        if (string.IsNullOrWhiteSpace(ownerType))
            throw new ArgumentException("OwnerType cannot be empty", nameof(ownerType));
        if (initialBalance < 0)
            throw new ArgumentException("Initial balance cannot be negative", nameof(initialBalance));

        UserId = userId;
        OwnerType = ownerType;
        Balance = initialBalance;
    }

    public void Deposit(decimal amount, string description)
    {
        if (amount <= 0)
            throw new ArgumentException("Deposit amount must be positive", nameof(amount));

        Balance += amount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Withdraw(decimal amount, string description)
    {
        if (amount <= 0)
            throw new ArgumentException("Withdrawal amount must be positive", nameof(amount));

        if (Balance < amount)
            throw new InvalidOperationException("Insufficient balance");

        Balance -= amount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddTransaction(WalletTransaction transaction)
    {
        _transactions.Add(transaction);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetOwnerType(string ownerType)
    {
        if (string.IsNullOrWhiteSpace(ownerType))
            throw new ArgumentException("OwnerType cannot be empty", nameof(ownerType));

        OwnerType = ownerType;
        UpdatedAt = DateTime.UtcNow;
    }
}
