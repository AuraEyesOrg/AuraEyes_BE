using Domain.Entities.Financial;
using Domain.Enums;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class WithdrawalRequestTests
{
    // Helper to create a valid WithdrawalRequest for tests that don't care about BankBin
    private static WithdrawalRequest CreateRequest(
        decimal amount = 200_000m,
        string bankBin = "970415")
        => new(Guid.NewGuid(), Guid.NewGuid(), amount, "VCB", "123", "Owner", bankBin);

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
            "  970415  ",
            "  HD-001  ",
            "  payout  ");

        request.Status.Should().Be(PaymentStatus.Pending);
        request.BankName.Should().Be("Vietcombank");
        request.BankAccountNumber.Should().Be("1234567890");
        request.AccountHolderName.Should().Be("Nguyen Van A");
        request.BankBin.Should().Be("970415");
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
            "Owner",
            "970415");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Withdrawal amount must be positive*");
    }

    [Fact]
    public void StartProcessing_FromPending_ShouldSucceed()
    {
        var request = CreateRequest();

        request.StartProcessing();

        request.Status.Should().Be(PaymentStatus.Processing);
    }

    [Fact]
    public void StartProcessing_FromNonPending_ShouldThrow()
    {
        var request = CreateRequest();
        request.Cancel("cancelled");

        var act = () => request.StartProcessing();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only pending withdrawal requests can start processing.*");
    }

    [Fact]
    public void MarkCompleted_FromProcessing_ShouldSetCompletionMetadata()
    {
        var request = CreateRequest(300_000m);
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
        var request = CreateRequest(300_000m);
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
        var request = CreateRequest(100_000m);

        request.Cancel("  user canceled  ");

        request.Status.Should().Be(PaymentStatus.Cancelled);
        request.AdminNote.Should().Be("user canceled");
        request.ProcessedAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkCompleted_FromCancelled_ShouldThrow()
    {
        var request = CreateRequest(100_000m);
        request.Cancel("closed");

        var act = () => request.MarkCompleted(Guid.NewGuid());

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only pending or processing withdrawal requests can be completed.*");
    }

    [Fact]
    public void SetPayOSPayout_FromPending_ShouldSetFieldsAndProcessing()
    {
        var request = CreateRequest();

        request.SetPayOSPayout("payout_ref_001", "payos-ext-id-123", "PROCESSING", "txn-001");

        request.Status.Should().Be(PaymentStatus.Processing);
        request.PayOSReferenceId.Should().Be("payout_ref_001");
        request.ExternalPayoutId.Should().Be("payos-ext-id-123");
        request.PayOSApprovalState.Should().Be("PROCESSING");
        request.PayOSTransactionId.Should().Be("txn-001");
    }

    [Fact]
    public void SetPayOSPayout_FromNonPending_ShouldThrow()
    {
        var request = CreateRequest();
        request.StartProcessing();

        var act = () => request.SetPayOSPayout("ref", "ext-id", "PROCESSING");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Can only set PayOS payout for Pending requests*");
    }

    [Fact]
    public void UpdatePayOSApprovalState_Succeeded_ShouldMarkCompleted()
    {
        var request = CreateRequest();
        request.SetPayOSPayout("payout_ref", "ext-id", "PROCESSING");

        request.UpdatePayOSApprovalState("SUCCEEDED", "txn-final");

        request.Status.Should().Be(PaymentStatus.Completed);
        request.PayOSApprovalState.Should().Be("SUCCEEDED");
        request.PayOSTransactionId.Should().Be("txn-final");
        request.ProcessedAt.Should().NotBeNull();
    }

    [Fact]
    public void UpdatePayOSApprovalState_Failed_ShouldMarkFailed()
    {
        var request = CreateRequest();
        request.SetPayOSPayout("payout_ref", "ext-id", "PROCESSING");

        request.UpdatePayOSApprovalState("FAILED");

        request.Status.Should().Be(PaymentStatus.Failed);
        request.PayOSApprovalState.Should().Be("FAILED");
        request.ProcessedAt.Should().NotBeNull();
    }
}
