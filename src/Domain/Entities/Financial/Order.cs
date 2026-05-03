using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Financial;

/// <summary>
/// Order entity - service orders
/// </summary>
public class Order : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public Guid? AppointmentId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal? DepositAmount { get; private set; }
    public string? Description { get; private set; }
    public OrderStatus Status { get; private set; }

    public decimal PaidAmount => _payments
        .Where(p => p.Status == PaymentStatus.Completed)
        .Sum(p => p.Amount);

    // Navigation properties
    private readonly List<Payment> _payments = new();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    private Order() { } // EF Core

    public Order(Guid userId, decimal totalAmount, decimal? depositAmount = null, string? description = null, Guid? appointmentId = null)
    {
        UserId = userId;
        TotalAmount = totalAmount;
        DepositAmount = depositAmount;
        Description = description;
        AppointmentId = appointmentId;
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
        if (Status != OrderStatus.Processing && Status != OrderStatus.Confirmed && Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot complete order in current status");

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

    /// <summary>
    /// Whether reception can check in the linked clinic appointment (tiền cọc satisfied when required).
    /// </summary>
    public bool IsClinicDepositSatisfiedForCheckIn()
    {
        if (!DepositAmount.HasValue || DepositAmount.Value <= 0m)
            return true;

        if (PaidAmount >= DepositAmount.Value)
            return true;

        if (Status != OrderStatus.Pending)
            return true;

        return false;
    }
}
