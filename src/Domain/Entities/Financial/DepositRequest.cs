using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Financial;

/// <summary>
/// DepositRequest entity - deposit request for wallet top-up via PayOS
/// </summary>
public class DepositRequest : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public Guid WalletId { get; private set; }

    /// <summary>
    /// Amount in VND
    /// </summary>
    public decimal Amount { get; private set; }

    public PaymentMethod PaymentMethod { get; private set; }
    public PaymentStatus Status { get; private set; }

    /// <summary>
    /// PayOS order code or other provider transaction ID
    /// </summary>
    public string? PaymentOrderCode { get; private set; }

    /// <summary>
    /// PayOS checkout URL
    /// </summary>
    public string? PaymentUrl { get; private set; }

    /// <summary>
    /// Provider transaction reference
    /// </summary>
    public string? ProviderTxnRef { get; private set; }

    /// <summary>
    /// Provider response data (JSON)
    /// </summary>
    public string? ProviderResponse { get; private set; }

    public DateTime? CompletedAt { get; private set; }
    public string? FailureReason { get; private set; }

    /// <summary>
    /// Return URL after payment
    /// </summary>
    public string? ReturnUrl { get; private set; }

    /// <summary>
    /// Cancel URL if payment is cancelled
    /// </summary>
    public string? CancelUrl { get; private set; }

    /// <summary>
    /// Description for payment
    /// </summary>
    public string? Description { get; private set; }

    // Navigation properties
    public Wallet Wallet { get; private set; } = null!;

    private DepositRequest() { } // EF Core

    public DepositRequest(
        Guid userId,
        Guid walletId,
        decimal amount,
        PaymentMethod paymentMethod,
        string? returnUrl = null,
        string? cancelUrl = null,
        string? description = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Deposit amount must be positive", nameof(amount));

        UserId = userId;
        WalletId = walletId;
        Amount = amount;
        PaymentMethod = paymentMethod;
        Status = PaymentStatus.Pending;
        ReturnUrl = returnUrl;
        CancelUrl = cancelUrl;
        Description = description;
    }

    /// <summary>
    /// Set payment link information from PayOS
    /// </summary>
    public void SetPaymentLink(string paymentUrl, string orderCode)
    {
        PaymentUrl = paymentUrl;
        PaymentOrderCode = orderCode;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Mark deposit as processing
    /// </summary>
    public void StartProcessing()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending deposits can start processing");

        Status = PaymentStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Mark deposit as completed
    /// </summary>
    public void Complete(string? providerTxnRef = null, string? providerResponse = null)
    {
        if (Status != PaymentStatus.Pending && Status != PaymentStatus.Processing)
            throw new InvalidOperationException("Cannot complete deposit in current status");

        Status = PaymentStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        ProviderTxnRef = providerTxnRef;
        ProviderResponse = providerResponse;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Mark deposit as failed
    /// </summary>
    public void Fail(string? reason = null)
    {
        Status = PaymentStatus.Failed;
        FailureReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancel the deposit request
    /// </summary>
    public void Cancel(string? reason = null)
    {
        if (Status == PaymentStatus.Completed || Status == PaymentStatus.Refunded)
            throw new InvalidOperationException("Cannot cancel completed or refunded deposits");

        Status = PaymentStatus.Cancelled;
        FailureReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }
}
