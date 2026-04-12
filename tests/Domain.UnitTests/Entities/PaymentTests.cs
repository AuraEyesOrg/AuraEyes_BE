using Domain.Entities.Financial;
using Domain.Enums;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class PaymentTests
{
    private readonly Guid _orderId = Guid.NewGuid();

    private Payment CreatePendingPayment()
        => new(_orderId, 100m, PaymentMethod.CreditCard);

    private Payment CreateProcessingPayment()
    {
        var payment = CreatePendingPayment();
        payment.StartProcessing();
        return payment;
    }

    private Payment CreateCompletedPayment()
    {
        var payment = CreateProcessingPayment();
        payment.Complete();
        return payment;
    }

    #region Constructor

    [Fact]
    public void Constructor_ValidInput_ShouldCreatePendingPayment()
    {
        var payment = new Payment(_orderId, 250.50m, PaymentMethod.VNPay);

        payment.OrderId.Should().Be(_orderId);
        payment.Amount.Should().Be(250.50m);
        payment.Method.Should().Be(PaymentMethod.VNPay);
        payment.Status.Should().Be(PaymentStatus.Pending);
        payment.PaidAt.Should().BeNull();
        payment.Id.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100.50)]
    public void Constructor_ZeroOrNegativeAmount_ShouldThrow(decimal invalidAmount)
    {
        var act = () => new Payment(_orderId, invalidAmount, PaymentMethod.Wallet);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("amount");
    }

    [Fact]
    public void Constructor_SmallPositiveAmount_ShouldCreate()
    {
        var payment = new Payment(_orderId, 0.01m, PaymentMethod.Cash);

        payment.Amount.Should().Be(0.01m);
    }

    #endregion

    #region StartProcessing

    [Fact]
    public void StartProcessing_PendingPayment_ShouldTransitionToProcessing()
    {
        var payment = CreatePendingPayment();

        payment.StartProcessing();

        payment.Status.Should().Be(PaymentStatus.Processing);
        payment.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void StartProcessing_CompletedPayment_ShouldThrow()
    {
        var payment = CreateCompletedPayment();

        var act = () => payment.StartProcessing();

        act.Should().Throw<InvalidOperationException>();
    }

    #endregion

    #region Complete

    [Fact]
    public void Complete_ProcessingPayment_ShouldTransitionToCompleted()
    {
        var payment = CreateProcessingPayment();

        payment.Complete();

        payment.Status.Should().Be(PaymentStatus.Completed);
        payment.PaidAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Complete_PendingPayment_ShouldTransitionToCompleted()
    {
        var payment = CreatePendingPayment();

        payment.Complete();

        payment.Status.Should().Be(PaymentStatus.Completed);
        payment.PaidAt.Should().NotBeNull();
    }

    [Fact]
    public void Complete_FailedPayment_ShouldThrow()
    {
        var payment = CreatePendingPayment();
        payment.Fail();

        var act = () => payment.Complete();

        act.Should().Throw<InvalidOperationException>();
    }

    #endregion

    #region Fail

    [Fact]
    public void Fail_PendingPayment_ShouldTransitionToFailed()
    {
        var payment = CreatePendingPayment();

        payment.Fail();

        payment.Status.Should().Be(PaymentStatus.Failed);
    }

    [Fact]
    public void Fail_ProcessingPayment_ShouldTransitionToFailed()
    {
        var payment = CreateProcessingPayment();

        payment.Fail();

        payment.Status.Should().Be(PaymentStatus.Failed);
    }

    #endregion

    #region Refund

    [Fact]
    public void Refund_CompletedPayment_ShouldTransitionToRefunded()
    {
        var payment = CreateCompletedPayment();

        payment.Refund();

        payment.Status.Should().Be(PaymentStatus.Refunded);
    }

    [Fact]
    public void Refund_PendingPayment_ShouldThrow()
    {
        var payment = CreatePendingPayment();

        var act = () => payment.Refund();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Refund_ProcessingPayment_ShouldThrow()
    {
        var payment = CreateProcessingPayment();

        var act = () => payment.Refund();

        act.Should().Throw<InvalidOperationException>();
    }

    #endregion

    #region Cancel

    [Fact]
    public void Cancel_PendingPayment_ShouldTransitionToCancelled()
    {
        var payment = CreatePendingPayment();

        payment.Cancel();

        payment.Status.Should().Be(PaymentStatus.Cancelled);
    }

    [Fact]
    public void Cancel_CompletedPayment_ShouldThrow()
    {
        var payment = CreateCompletedPayment();

        var act = () => payment.Cancel();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cancel_RefundedPayment_ShouldThrow()
    {
        var payment = CreateCompletedPayment();
        payment.Refund();

        var act = () => payment.Cancel();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cancel_ProcessingPayment_ShouldTransitionToCancelled()
    {
        var payment = CreateProcessingPayment();

        payment.Cancel();

        payment.Status.Should().Be(PaymentStatus.Cancelled);
    }

    #endregion

    #region Lifecycle

    [Fact]
    public void FullLifecycle_PendingToRefunded_ShouldTransitionCorrectly()
    {
        var payment = CreatePendingPayment();
        payment.Status.Should().Be(PaymentStatus.Pending);

        payment.StartProcessing();
        payment.Status.Should().Be(PaymentStatus.Processing);

        payment.Complete();
        payment.Status.Should().Be(PaymentStatus.Completed);

        payment.Refund();
        payment.Status.Should().Be(PaymentStatus.Refunded);
    }

    #endregion
}
