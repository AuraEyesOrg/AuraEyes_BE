using Domain.Entities.Financial;
using Domain.Enums;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class OrderTests
{
    [Fact]
    public void Constructor_ShouldCreatePendingOrder()
    {
        var userId = Guid.NewGuid();

        var order = new Order(userId);

        order.UserId.Should().Be(userId);
        order.Status.Should().Be(OrderStatus.Pending);
        order.Payments.Should().BeEmpty();
    }

    #region State Machine: Happy Path

    [Fact]
    public void FullLifecycle_Pending_Confirmed_Processing_Completed()
    {
        var order = new Order(Guid.NewGuid());

        order.Confirm();
        order.Status.Should().Be(OrderStatus.Confirmed);

        order.StartProcessing();
        order.Status.Should().Be(OrderStatus.Processing);

        order.Complete();
        order.Status.Should().Be(OrderStatus.Completed);
    }

    [Fact]
    public void Refund_CompletedOrder_ShouldRefund()
    {
        var order = new Order(Guid.NewGuid());
        order.Confirm();
        order.StartProcessing();
        order.Complete();

        order.Refund();

        order.Status.Should().Be(OrderStatus.Refunded);
    }

    #endregion

    #region State Machine: Invalid Transitions

    [Fact]
    public void Confirm_NonPendingOrder_ShouldThrow()
    {
        var order = new Order(Guid.NewGuid());
        order.Confirm();

        var act = () => order.Confirm();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only pending orders can be confirmed");
    }

    [Fact]
    public void StartProcessing_NonConfirmedOrder_ShouldThrow()
    {
        var order = new Order(Guid.NewGuid());

        var act = () => order.StartProcessing();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only confirmed orders can start processing");
    }

    [Fact]
    public void Complete_NonProcessingOrder_ShouldThrow()
    {
        var order = new Order(Guid.NewGuid());
        order.Confirm();

        var act = () => order.Complete();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only processing orders can be completed");
    }

    [Fact]
    public void Cancel_CompletedOrder_ShouldThrow()
    {
        var order = new Order(Guid.NewGuid());
        order.Confirm();
        order.StartProcessing();
        order.Complete();

        var act = () => order.Cancel();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot cancel completed or refunded orders");
    }

    [Fact]
    public void Cancel_RefundedOrder_ShouldThrow()
    {
        var order = new Order(Guid.NewGuid());
        order.Confirm();
        order.StartProcessing();
        order.Complete();
        order.Refund();

        var act = () => order.Cancel();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot cancel completed or refunded orders");
    }

    [Fact]
    public void Refund_NonCompletedOrder_ShouldThrow()
    {
        var order = new Order(Guid.NewGuid());

        var act = () => order.Refund();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only completed orders can be refunded");
    }

    #endregion

    #region Cancel From Various States

    [Fact]
    public void Cancel_PendingOrder_ShouldSucceed()
    {
        var order = new Order(Guid.NewGuid());

        order.Cancel();

        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_ConfirmedOrder_ShouldSucceed()
    {
        var order = new Order(Guid.NewGuid());
        order.Confirm();

        order.Cancel();

        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_ProcessingOrder_ShouldSucceed()
    {
        var order = new Order(Guid.NewGuid());
        order.Confirm();
        order.StartProcessing();

        order.Cancel();

        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    #endregion
}
