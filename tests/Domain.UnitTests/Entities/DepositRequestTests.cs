using Domain.Entities.Financial;
using Domain.Enums;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class DepositRequestTests
{
    [Fact]
    public void Constructor_ValidInput_ShouldCreatePendingDeposit()
    {
        var userId = Guid.NewGuid();
        var walletId = Guid.NewGuid();

        var deposit = new DepositRequest(
            userId,
            walletId,
            200_000m,
            PaymentMethod.PayOS,
            "https://return.local",
            "https://cancel.local",
            "Top up");

        deposit.UserId.Should().Be(userId);
        deposit.WalletId.Should().Be(walletId);
        deposit.Amount.Should().Be(200_000m);
        deposit.PaymentMethod.Should().Be(PaymentMethod.PayOS);
        deposit.Status.Should().Be(PaymentStatus.Pending);
        deposit.ReturnUrl.Should().Be("https://return.local");
        deposit.CancelUrl.Should().Be("https://cancel.local");
        deposit.Description.Should().Be("Top up");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_NonPositiveAmount_ShouldThrow(decimal amount)
    {
        var act = () => new DepositRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            amount,
            PaymentMethod.PayOS);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Deposit amount must be positive*");
    }

    [Fact]
    public void SetPaymentLink_ShouldSetOrderCodeAndUrl()
    {
        var deposit = new DepositRequest(Guid.NewGuid(), Guid.NewGuid(), 100_000m, PaymentMethod.PayOS);

        deposit.SetPaymentLink("https://pay.local", "ORDER-123");

        deposit.PaymentUrl.Should().Be("https://pay.local");
        deposit.PaymentOrderCode.Should().Be("ORDER-123");
        deposit.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void StartProcessing_FromPending_ShouldSucceed()
    {
        var deposit = new DepositRequest(Guid.NewGuid(), Guid.NewGuid(), 100_000m, PaymentMethod.PayOS);

        deposit.StartProcessing();

        deposit.Status.Should().Be(PaymentStatus.Processing);
    }

    [Fact]
    public void StartProcessing_FromNonPending_ShouldThrow()
    {
        var deposit = new DepositRequest(Guid.NewGuid(), Guid.NewGuid(), 100_000m, PaymentMethod.PayOS);
        deposit.Fail("failed once");

        var act = () => deposit.StartProcessing();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only pending deposits can start processing");
    }

    [Fact]
    public void Complete_FromPending_ShouldSetCompletedStatusAndMetadata()
    {
        var deposit = new DepositRequest(Guid.NewGuid(), Guid.NewGuid(), 100_000m, PaymentMethod.PayOS);

        deposit.Complete("TXN-1", "ok");

        deposit.Status.Should().Be(PaymentStatus.Completed);
        deposit.ProviderTxnRef.Should().Be("TXN-1");
        deposit.ProviderResponse.Should().Be("ok");
        deposit.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public void Complete_FromCancelled_ShouldThrow()
    {
        var deposit = new DepositRequest(Guid.NewGuid(), Guid.NewGuid(), 100_000m, PaymentMethod.PayOS);
        deposit.Cancel("user canceled");

        var act = () => deposit.Complete();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot complete deposit in current status");
    }

    [Fact]
    public void Cancel_CompletedDeposit_ShouldThrow()
    {
        var deposit = new DepositRequest(Guid.NewGuid(), Guid.NewGuid(), 100_000m, PaymentMethod.PayOS);
        deposit.Complete();

        var act = () => deposit.Cancel("too late");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot cancel completed or refunded deposits");
    }

    [Fact]
    public void Fail_ShouldSetFailedStatusAndReason()
    {
        var deposit = new DepositRequest(Guid.NewGuid(), Guid.NewGuid(), 100_000m, PaymentMethod.PayOS);

        deposit.Fail("expired");

        deposit.Status.Should().Be(PaymentStatus.Failed);
        deposit.FailureReason.Should().Be("expired");
    }
}
