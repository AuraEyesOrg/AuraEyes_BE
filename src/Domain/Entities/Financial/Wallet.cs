using Domain.Common;

namespace Domain.Entities.Financial;

/// <summary>
/// Wallet entity - user's digital wallet
/// One-to-one relationship with User
/// </summary>
public class Wallet : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public decimal Balance { get; private set; }

    // Navigation properties
    private readonly List<WalletTransaction> _transactions = new();
    public IReadOnlyCollection<WalletTransaction> Transactions => _transactions.AsReadOnly();

    private Wallet() { } // EF Core

    public Wallet(Guid userId, decimal initialBalance = 0)
    {
        if (initialBalance < 0)
            throw new ArgumentException("Initial balance cannot be negative", nameof(initialBalance));

        UserId = userId;
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
}
