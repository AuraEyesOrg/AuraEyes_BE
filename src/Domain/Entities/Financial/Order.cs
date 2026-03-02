using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Financial;

/// <summary>
/// Order entity - service orders
/// </summary>
public class Order : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; }

    // Navigation properties
    private readonly List<Payment> _payments = new();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    private Order() { } // EF Core

    public Order(Guid userId)
    {
        UserId = userId;
        Status = OrderStatus.Pending;
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed");

        Status = OrderStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void StartProcessing()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed orders can start processing");

        Status = OrderStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status != OrderStatus.Processing)
            throw new InvalidOperationException("Only processing orders can be completed");

        Status = OrderStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Completed || Status == OrderStatus.Refunded)
            throw new InvalidOperationException("Cannot cancel completed or refunded orders");

        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Refund()
    {
        if (Status != OrderStatus.Completed)
            throw new InvalidOperationException("Only completed orders can be refunded");

        Status = OrderStatus.Refunded;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddPayment(Payment payment)
    {
        _payments.Add(payment);
        UpdatedAt = DateTime.UtcNow;
    }
}
