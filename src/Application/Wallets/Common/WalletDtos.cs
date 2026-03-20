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
