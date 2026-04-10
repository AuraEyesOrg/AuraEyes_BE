using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Financial;

/// <summary>
/// Withdrawal request entity supporting both manual admin payouts and automated PayOS Payout API.
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

    /// <summary>Mã BIN ngân hàng dùng cho PayOS Payout (ví dụ: 970415 = Vietinbank).</summary>
    public string BankBin { get; private set; } = string.Empty;

    /// <summary>
    /// Mã tham chiếu gửi đến PayOS Payout API (referenceId).
    /// Được sinh tự động khi tạo lệnh chi qua PayOS.
    /// </summary>
    public string? PayOSReferenceId { get; private set; }

    /// <summary>
    /// ID lệnh chi trả về từ PayOS (id field trong response).
    /// </summary>
    public string? ExternalPayoutId { get; private set; }

    /// <summary>
    /// ID giao dịch chi tiết bên trong PayOS (transactions[].id).
    /// </summary>
    public string? PayOSTransactionId { get; private set; }

    /// <summary>
    /// Trạng thái phê duyệt từ PayOS: PROCESSING | SUCCEEDED | FAILED.
    /// </summary>
    public string? PayOSApprovalState { get; private set; }

    /// <summary>Phí giao dịch ước tính hoặc thực tế từ PayOS.</summary>
    public decimal? Fee { get; private set; }

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
        string bankBin = "",
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
        BankBin = bankBin.Trim();
        ContractNumber = string.IsNullOrWhiteSpace(contractNumber) ? null : contractNumber.Trim();
        Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
    }

    /// <summary>
    /// Gắn thông tin lệnh chi PayOS sau khi gọi API thành công.
    /// </summary>
    public void SetPayOSPayout(
        string payOSReferenceId,
        string externalPayoutId,
        string approvalState,
        string? transactionId = null,
        decimal? fee = null)
    {
        if (string.IsNullOrWhiteSpace(payOSReferenceId))
            throw new ArgumentException("PayOS reference ID is required", nameof(payOSReferenceId));
        if (string.IsNullOrWhiteSpace(externalPayoutId))
            throw new ArgumentException("External payout ID is required", nameof(externalPayoutId));

        PayOSReferenceId = payOSReferenceId.Trim();
        ExternalPayoutId = externalPayoutId.Trim();
        PayOSApprovalState = approvalState.Trim();
        PayOSTransactionId = string.IsNullOrWhiteSpace(transactionId) ? null : transactionId.Trim();
        Fee = fee;
        Status = PaymentStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cập nhật trạng thái từ PayOS webhook hoặc polling.
    /// </summary>
    public void UpdatePayOSApprovalState(string approvalState, string? transactionId = null)
    {
        PayOSApprovalState = approvalState.Trim();

        if (!string.IsNullOrWhiteSpace(transactionId))
            PayOSTransactionId = transactionId.Trim();

        if (approvalState.Equals("SUCCEEDED", StringComparison.OrdinalIgnoreCase))
        {
            Status = PaymentStatus.Completed;
            ProcessedAt = DateTime.UtcNow;
        }
        else if (approvalState.Equals("FAILED", StringComparison.OrdinalIgnoreCase))
        {
            Status = PaymentStatus.Failed;
            ProcessedAt = DateTime.UtcNow;
        }

        UpdatedAt = DateTime.UtcNow;
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
