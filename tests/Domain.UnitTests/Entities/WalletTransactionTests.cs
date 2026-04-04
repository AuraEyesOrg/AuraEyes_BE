using Domain.Entities.Financial;
using Domain.Enums;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class WalletTransactionTests
{
    [Fact]
    public void Constructor_ValidInput_ShouldCreateTransaction()
    {
        var walletId = Guid.NewGuid();
        var referenceId = Guid.NewGuid();

        var tx = new WalletTransaction(
            walletId,
            120_000m,
            TransactionType.Deposit,
            "Deposit success",
            "Deposit",
            referenceId);

        tx.WalletId.Should().Be(walletId);
        tx.Amount.Should().Be(120_000m);
        tx.TransactionType.Should().Be(TransactionType.Deposit);
        tx.Description.Should().Be("Deposit success");
        tx.ReferenceType.Should().Be("Deposit");
        tx.ReferenceId.Should().Be(referenceId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5000)]
    public void Constructor_NonPositiveAmount_ShouldThrow(decimal amount)
    {
        var act = () => new WalletTransaction(
            Guid.NewGuid(),
            amount,
            TransactionType.Payment,
            "invalid");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Transaction amount must be positive*");
    }

    [Fact]
    public void Constructor_OptionalFieldsNull_ShouldAllowCreation()
    {
        var tx = new WalletTransaction(Guid.NewGuid(), 50_000m, TransactionType.Payment);

        tx.Description.Should().BeNull();
        tx.ReferenceType.Should().BeNull();
        tx.ReferenceId.Should().BeNull();
    }
}
