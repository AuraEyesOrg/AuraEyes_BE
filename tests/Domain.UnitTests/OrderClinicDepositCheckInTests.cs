using Domain.Entities.Financial;
using Domain.Enums;
using FluentAssertions;

namespace Domain.UnitTests;

public class OrderClinicDepositCheckInTests
{
    [Fact]
    public void IsClinicDepositSatisfiedForCheckIn_NoDepositRequired_ReturnsTrue()
    {
        var order = new Order(Guid.NewGuid(), 100m, depositAmount: null);

        order.IsClinicDepositSatisfiedForCheckIn().Should().BeTrue();
    }

    [Fact]
    public void IsClinicDepositSatisfiedForCheckIn_DepositPendingAndUnpaid_ReturnsFalse()
    {
        var order = new Order(Guid.NewGuid(), 100m, depositAmount: 30m);

        order.IsClinicDepositSatisfiedForCheckIn().Should().BeFalse();
    }

    [Fact]
    public void IsClinicDepositSatisfiedForCheckIn_PaidAmountCoversDeposit_ReturnsTrue()
    {
        var order = new Order(Guid.NewGuid(), 100m, depositAmount: 30m);
        var payment = new Payment(order.Id, 30m, PaymentMethod.Cash);
        order.AddPayment(payment);
        payment.Complete();

        order.IsClinicDepositSatisfiedForCheckIn().Should().BeTrue();
    }

    [Fact]
    public void IsClinicDepositSatisfiedForCheckIn_OrderConfirmed_ReturnsTrue()
    {
        var order = new Order(Guid.NewGuid(), 100m, depositAmount: 30m);
        order.Confirm();

        order.IsClinicDepositSatisfiedForCheckIn().Should().BeTrue();
    }
}
