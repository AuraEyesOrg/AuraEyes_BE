using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Financial;

/// <summary>
/// Withdrawal request entity for manual bank transfer payouts handled by system admin.
/// </summary>
public class WithdrawalRequest : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public Guid WalletId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string BankName { get; private set; } = string.Empty;
    public string BankAccountNumber { get; private set; } = string.Empty;
    public string AccountHolderName { get; private set; } = string.Empty;
    public string? ContractNumber { get; private set; }
    public string? Note { get; private set; }
    public string? AdminNote { get; private set; }
    public string? TransferReference { get; private set; }
    public Guid? ProcessedByAdminId { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    // Navigation property
    public Wallet Wallet { get; private set; } = null!;

    private WithdrawalRequest() { } // EF Core

    public WithdrawalRequest(
        Guid userId,
        Guid walletId,
        decimal amount,
        string bankName,
        string bankAccountNumber,
        string accountHolderName,
        string? contractNumber = null,
        string? note = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Withdrawal amount must be positive", nameof(amount));
        if (string.IsNullOrWhiteSpace(bankName))
            throw new ArgumentException("Bank name is required", nameof(bankName));
        if (string.IsNullOrWhiteSpace(bankAccountNumber))
            throw new ArgumentException("Bank account number is required", nameof(bankAccountNumber));
        if (string.IsNullOrWhiteSpace(accountHolderName))
            throw new ArgumentException("Account holder name is required", nameof(accountHolderName));

        UserId = userId;
        WalletId = walletId;
        Amount = amount;
        Status = PaymentStatus.Pending;
        BankName = bankName.Trim();
        BankAccountNumber = bankAccountNumber.Trim();
        AccountHolderName = accountHolderName.Trim();
        ContractNumber = string.IsNullOrWhiteSpace(contractNumber) ? null : contractNumber.Trim();
        Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
    }

    public void MarkCompleted(Guid adminUserId, string? transferReference = null, string? adminNote = null)
    {
        if (Status != PaymentStatus.Pending && Status != PaymentStatus.Processing)
            throw new InvalidOperationException("Only pending or processing withdrawal requests can be completed.");

        Status = PaymentStatus.Completed;
        ProcessedByAdminId = adminUserId;
        ProcessedAt = DateTime.UtcNow;
        TransferReference = string.IsNullOrWhiteSpace(transferReference) ? null : transferReference.Trim();
        AdminNote = string.IsNullOrWhiteSpace(adminNote) ? null : adminNote.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkRejected(Guid adminUserId, string? adminNote = null)
    {
        if (Status != PaymentStatus.Pending && Status != PaymentStatus.Processing)
            throw new InvalidOperationException("Only pending or processing withdrawal requests can be rejected.");

        Status = PaymentStatus.Failed;
        ProcessedByAdminId = adminUserId;
        ProcessedAt = DateTime.UtcNow;
        AdminNote = string.IsNullOrWhiteSpace(adminNote) ? null : adminNote.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void StartProcessing()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending withdrawal requests can start processing.");

        Status = PaymentStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string? reason = null)
    {
        if (Status != PaymentStatus.Pending && Status != PaymentStatus.Processing)
            throw new InvalidOperationException("Only pending or processing requests can be cancelled.");

        Status = PaymentStatus.Cancelled;
        AdminNote = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        ProcessedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
