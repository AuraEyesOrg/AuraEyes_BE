using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Payment entity - payment for orders
/// </summary>
public class Payment : BaseEntity
{
    public Guid OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public PaymentMethod Method { get; private set; }
    public DateTime? PaidAt { get; private set; }

    private Payment() { } // EF Core

    public Payment(Guid orderId, decimal amount, PaymentMethod method)
    {
        if (amount <= 0)
            throw new ArgumentException("Payment amount must be positive", nameof(amount));

        OrderId = orderId;
        Amount = amount;
        Method = method;
        Status = PaymentStatus.Pending;
    }

    public void StartProcessing()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending payments can start processing");

        Status = PaymentStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status != PaymentStatus.Processing && Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Cannot complete payment in current status");

        Status = PaymentStatus.Completed;
        PaidAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Fail()
    {
        Status = PaymentStatus.Failed;
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
