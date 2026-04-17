using Domain.Enums;

namespace Application.Wallets.Common;

/// <summary>
/// DTO for Wallet information.
/// </summary>
public class WalletDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public decimal TotalDepositsThisMonth { get; set; }
    public decimal TotalSpentThisMonth { get; set; }
    public int TransactionsThisMonth { get; set; }
}

/// <summary>
/// DTO for Wallet Transaction information.
/// </summary>
public class WalletTransactionDto
{
    public Guid Id { get; set; }
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType TransactionType { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
}

/// <summary>
/// DTO for Deposit Request information.
/// </summary>
public class DepositRequestDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; }
    public string? PaymentOrderCode { get; set; }
    public string? PaymentUrl { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? FailureReason { get; set; }
}

/// <summary>
/// Response after creating a deposit request.
/// </summary>
public class CreateDepositResponse
{
    public Guid DepositRequestId { get; set; }
    public string PaymentUrl { get; set; } = string.Empty;
    public string OrderCode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Response after verifying a payment.
/// </summary>
public class VerifyPaymentResponse
{
    public Guid DepositRequestId { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public decimal? NewBalance { get; set; }
}

/// <summary>
/// DTO for Withdrawal Request information (includes PayOS Payout fields).
/// </summary>
public class WithdrawalRequestDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string BankAccountNumber { get; set; } = string.Empty;
    public string AccountHolderName { get; set; } = string.Empty;
    public string? ContractNumber { get; set; }
    public string? Note { get; set; }
    public string? AdminNote { get; set; }
    public string? TransferReference { get; set; }
    public Guid? ProcessedByAdminId { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    // ── PayOS Payout fields ──────────────────────────────────────────────────

    /// <summary>Mã BIN ngân hàng PayOS.</summary>
    public string BankBin { get; set; } = string.Empty;

    /// <summary>ID lệnh chi trả về từ PayOS.</summary>
    public string? ExternalPayoutId { get; set; }

    /// <summary>Mã tham chiếu nội bộ gửi lên PayOS.</summary>
    public string? PayOSReferenceId { get; set; }

    /// <summary>ID giao dịch chi tiết bên trong PayOS.</summary>
    public string? PayOSTransactionId { get; set; }

    /// <summary>Trạng thái phê duyệt PayOS: PROCESSING | SUCCEEDED | FAILED.</summary>
    public string? PayOSApprovalState { get; set; }

    /// <summary>Phí giao dịch từ PayOS.</summary>
    public decimal? Fee { get; set; }
}

/// <summary>
/// DTO for admin withdrawal queue rows (includes doctor info + PayOS fields).
/// </summary>
public class AdminWithdrawalRequestDto : WithdrawalRequestDto
{
    public string DoctorFullName { get; set; } = string.Empty;
    public string DoctorEmail { get; set; } = string.Empty;
}

// ────────────────────────────────────────────────────────────────────────────
// PayOS Payout DTOs
// ────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Alias kept for backward-compat — WithdrawalRequestDto now includes all PayOS fields.
/// </summary>
public class WithdrawalRequestDetailDto : WithdrawalRequestDto { }

/// <summary>
/// Response sau khi trigger lệnh chi PayOS thành công.
/// </summary>
public class PayoutViaPayOSResponse
{
    public Guid WithdrawalRequestId { get; set; }

    /// <summary>ID lệnh chi PayOS.</summary>
    public string ExternalPayoutId { get; set; } = string.Empty;

    /// <summary>Mã tham chiếu nội bộ gửi đến PayOS.</summary>
    public string PayOSReferenceId { get; set; } = string.Empty;

    /// <summary>Trạng thái phê duyệt: PROCESSING | SUCCEEDED | FAILED.</summary>
    public string ApprovalState { get; set; } = string.Empty;

    /// <summary>Trạng thái hiện tại của WithdrawalRequest trong hệ thống.</summary>
    public string WithdrawalStatus { get; set; } = string.Empty;

    /// <summary>Danh sách giao dịch chi tiết từ PayOS.</summary>
    public List<PayOSPayoutTransactionDto> Transactions { get; set; } = new();
}

/// <summary>
/// Response khi lấy trạng thái lệnh chi từ PayOS.
/// </summary>
public class PayoutStatusResponse
{
    public Guid WithdrawalRequestId { get; set; }
    public string ExternalPayoutId { get; set; } = string.Empty;
    public string PayOSReferenceId { get; set; } = string.Empty;
    public string ApprovalState { get; set; } = string.Empty;
    public string WithdrawalStatus { get; set; } = string.Empty;
    public List<PayOSPayoutTransactionDto> Transactions { get; set; } = new();
}

/// <summary>
/// DTO giao dịch chi tiết từ PayOS Payout.
/// </summary>
public class PayOSPayoutTransactionDto
{
    public string Id { get; set; } = string.Empty;
    public long Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ToBin { get; set; } = string.Empty;
    public string ToAccountNumber { get; set; } = string.Empty;
    public string ToAccountName { get; set; } = string.Empty;

    /// <summary>Trạng thái: PROCESSING | SUCCEEDED | FAILED.</summary>
    public string State { get; set; } = string.Empty;
}

/// <summary>
/// Response thông tin số dư tài khoản chi PayOS.
/// </summary>
public class PayoutAccountBalanceResponse
{
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public long Balance { get; set; }
}
