using Domain.Entities.Financial;
using Domain.Enums;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class WithdrawalRequestTests
{
    [Fact]
    public void Constructor_ValidInput_ShouldTrimValuesAndSetPending()
    {
        var request = new WithdrawalRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            500_000m,
            "  Vietcombank  ",
            "  1234567890  ",
            "  Nguyen Van A  ",
            "  HD-001  ",
            "  payout  ");

        request.Status.Should().Be(PaymentStatus.Pending);
        request.BankName.Should().Be("Vietcombank");
        request.BankAccountNumber.Should().Be("1234567890");
        request.AccountHolderName.Should().Be("Nguyen Van A");
        request.ContractNumber.Should().Be("HD-001");
        request.Note.Should().Be("payout");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_NonPositiveAmount_ShouldThrow(decimal amount)
    {
        var act = () => new WithdrawalRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            amount,
            "VCB",
            "123",
            "Owner");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Withdrawal amount must be positive*");
    }

    [Fact]
    public void StartProcessing_FromPending_ShouldSucceed()
    {
        var request = new WithdrawalRequest(Guid.NewGuid(), Guid.NewGuid(), 200_000m, "VCB", "123", "Owner");

        request.StartProcessing();

        request.Status.Should().Be(PaymentStatus.Processing);
    }

    [Fact]
    public void StartProcessing_FromNonPending_ShouldThrow()
    {
        var request = new WithdrawalRequest(Guid.NewGuid(), Guid.NewGuid(), 200_000m, "VCB", "123", "Owner");
        request.Cancel("cancelled");

        var act = () => request.StartProcessing();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only pending withdrawal requests can start processing.*");
    }

    [Fact]
    public void MarkCompleted_FromProcessing_ShouldSetCompletionMetadata()
    {
        var request = new WithdrawalRequest(Guid.NewGuid(), Guid.NewGuid(), 300_000m, "VCB", "123", "Owner");
        request.StartProcessing();
        var adminId = Guid.NewGuid();

        request.MarkCompleted(adminId, "  TRX001  ", "  done  ");

        request.Status.Should().Be(PaymentStatus.Completed);
        request.ProcessedByAdminId.Should().Be(adminId);
        request.TransferReference.Should().Be("TRX001");
        request.AdminNote.Should().Be("done");
        request.ProcessedAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkRejected_FromPending_ShouldSetFailedAndAdminNote()
    {
        var request = new WithdrawalRequest(Guid.NewGuid(), Guid.NewGuid(), 300_000m, "VCB", "123", "Owner");
        var adminId = Guid.NewGuid();

        request.MarkRejected(adminId, "  invalid account  ");

        request.Status.Should().Be(PaymentStatus.Failed);
        request.ProcessedByAdminId.Should().Be(adminId);
        request.AdminNote.Should().Be("invalid account");
        request.ProcessedAt.Should().NotBeNull();
    }

    [Fact]
    public void Cancel_FromPending_ShouldSetCancelledAndReason()
    {
        var request = new WithdrawalRequest(Guid.NewGuid(), Guid.NewGuid(), 100_000m, "VCB", "123", "Owner");

        request.Cancel("  user canceled  ");

        request.Status.Should().Be(PaymentStatus.Cancelled);
        request.AdminNote.Should().Be("user canceled");
        request.ProcessedAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkCompleted_FromCancelled_ShouldThrow()
    {
        var request = new WithdrawalRequest(Guid.NewGuid(), Guid.NewGuid(), 100_000m, "VCB", "123", "Owner");
        request.Cancel("closed");

        var act = () => request.MarkCompleted(Guid.NewGuid());

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only pending or processing withdrawal requests can be completed.*");
    }
}
