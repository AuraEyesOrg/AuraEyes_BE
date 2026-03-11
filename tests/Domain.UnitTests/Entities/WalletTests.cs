using Domain.Entities.Financial;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class WalletTests
{
    [Fact]
    public void Constructor_ValidInput_ShouldCreateWallet()
    {
        var userId = Guid.NewGuid();

        var wallet = new Wallet(userId, "Patient", 100m);

        wallet.UserId.Should().Be(userId);
        wallet.OwnerType.Should().Be("Patient");
        wallet.Balance.Should().Be(100m);
        wallet.Transactions.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_DefaultBalance_ShouldBeZero()
    {
        var wallet = new Wallet(Guid.NewGuid(), "Patient");
        wallet.Balance.Should().Be(0);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Constructor_EmptyOwnerType_ShouldThrow(string? ownerType)
    {
        var act = () => new Wallet(Guid.NewGuid(), ownerType!);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*OwnerType cannot be empty*");
    }

    [Fact]
    public void Constructor_NegativeBalance_ShouldThrow()
    {
        var act = () => new Wallet(Guid.NewGuid(), "Patient", -10m);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Initial balance cannot be negative*");
    }

    [Fact]
    public void Deposit_PositiveAmount_ShouldIncreaseBalance()
    {
        var wallet = new Wallet(Guid.NewGuid(), "Patient", 100m);

        wallet.Deposit(50m, "Top up");

        wallet.Balance.Should().Be(150m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Deposit_NonPositiveAmount_ShouldThrow(decimal amount)
    {
        var wallet = new Wallet(Guid.NewGuid(), "Patient", 100m);

        var act = () => wallet.Deposit(amount, "Invalid");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Deposit amount must be positive*");
    }

    [Fact]
    public void Withdraw_SufficientBalance_ShouldDecreaseBalance()
    {
        var wallet = new Wallet(Guid.NewGuid(), "Patient", 100m);

        wallet.Withdraw(30m, "Payment");

        wallet.Balance.Should().Be(70m);
    }

    [Fact]
    public void Withdraw_InsufficientBalance_ShouldThrow()
    {
        var wallet = new Wallet(Guid.NewGuid(), "Patient", 50m);

        var act = () => wallet.Withdraw(100m, "Too much");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Insufficient balance");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Withdraw_NonPositiveAmount_ShouldThrow(decimal amount)
    {
        var wallet = new Wallet(Guid.NewGuid(), "Patient", 100m);

        var act = () => wallet.Withdraw(amount, "Invalid");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Withdrawal amount must be positive*");
    }

    [Fact]
    public void Withdraw_ExactBalance_ShouldResultInZero()
    {
        var wallet = new Wallet(Guid.NewGuid(), "Patient", 100m);

        wallet.Withdraw(100m, "Full withdrawal");

        wallet.Balance.Should().Be(0);
    }
}
