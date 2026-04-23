using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Financial;

/// <summary>
/// Payment entity - payment for orders (supports PayOS)
/// </summary>
public class Payment : BaseEntity, IAggregateRoot
{
    public Guid OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public PaymentMethod Method { get; private set; }
    public DateTime? PaidAt { get; private set; }

    // PayOS specific fields
    public string? PaymentOrderCode { get; private set; }
    public string? PaymentUrl { get; private set; }
    public string? ProviderTxnRef { get; private set; }
    public string? ProviderResponse { get; private set; }
    public string? Description { get; private set; }

    private Payment() { } // EF Core

    public Payment(Guid orderId, decimal amount, PaymentMethod method, string? description = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Payment amount must be positive", nameof(amount));

        OrderId = orderId;
        Amount = amount;
        Method = method;
        Description = description;
        Status = PaymentStatus.Pending;
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

    public void StartProcessing()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending payments can start processing");

        Status = PaymentStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete(string? providerTxnRef = null, string? providerResponse = null)
    {
        if (Status != PaymentStatus.Processing && Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Cannot complete payment in current status");

        Status = PaymentStatus.Completed;
        PaidAt = DateTime.UtcNow;
        ProviderTxnRef = providerTxnRef;
        ProviderResponse = providerResponse;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Fail(string? reason = null)
    {
        Status = PaymentStatus.Failed;
        Description = string.IsNullOrEmpty(Description) ? reason : $"{Description} | Error: {reason}";
        UpdatedAt = DateTime.UtcNow;
    }

    public void Refund()
    {
        if (Status != PaymentStatus.Completed)
            throw new InvalidOperationException("Only completed payments can be refunded");

        Status = PaymentStatus.Refunded;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == PaymentStatus.Completed || Status == PaymentStatus.Refunded)
            throw new InvalidOperationException("Cannot cancel completed or refunded payments");

        Status = PaymentStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}
