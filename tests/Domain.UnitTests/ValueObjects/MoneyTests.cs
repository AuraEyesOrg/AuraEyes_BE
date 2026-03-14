using Domain.ValueObjects;
using FluentAssertions;

namespace Domain.UnitTests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Constructor_ValidInput_ShouldCreateMoney()
    {
        var money = new Money(100.50m, "usd");

        money.Amount.Should().Be(100.50m);
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Constructor_NegativeAmount_ShouldThrow()
    {
        var act = () => new Money(-1, "USD");
        act.Should().Throw<ArgumentException>().WithMessage("*Amount cannot be negative*");
    }

    [Fact]
    public void Constructor_ZeroAmount_ShouldSucceed()
    {
        var money = new Money(0, "USD");
        money.Amount.Should().Be(0);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Constructor_EmptyCurrency_ShouldThrow(string? currency)
    {
        var act = () => new Money(10, currency!);
        act.Should().Throw<ArgumentException>().WithMessage("*Currency cannot be empty*");
    }

    [Fact]
    public void Currency_ShouldBeUpperCase()
    {
        var money = new Money(10, "eur");
        money.Currency.Should().Be("EUR");
    }

    [Fact]
    public void Addition_SameCurrency_ShouldAdd()
    {
        var a = new Money(10, "USD");
        var b = new Money(20, "USD");

        var result = a + b;

        result.Amount.Should().Be(30);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Addition_DifferentCurrency_ShouldThrow()
    {
        var a = new Money(10, "USD");
        var b = new Money(20, "EUR");

        var act = () => a + b;
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot add money with different currencies");
    }

    [Fact]
    public void Subtraction_SameCurrency_ShouldSubtract()
    {
        var a = new Money(30, "USD");
        var b = new Money(10, "USD");

        var result = a - b;

        result.Amount.Should().Be(20);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Subtraction_DifferentCurrency_ShouldThrow()
    {
        var a = new Money(30, "USD");
        var b = new Money(10, "EUR");

        var act = () => a - b;
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot subtract money with different currencies");
    }

    [Fact]
    public void Equals_SameValues_ShouldBeEqual()
    {
        var m1 = new Money(100, "USD");
        var m2 = new Money(100, "USD");

        m1.Should().Be(m2);
        (m1 == m2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentAmount_ShouldNotBeEqual()
    {
        var m1 = new Money(100, "USD");
        var m2 = new Money(200, "USD");

        (m1 != m2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentCurrency_ShouldNotBeEqual()
    {
        var m1 = new Money(100, "USD");
        var m2 = new Money(100, "EUR");

        (m1 != m2).Should().BeTrue();
    }

    [Fact]
    public void ToString_ShouldFormatCorrectly()
    {
        var money = new Money(1234.56m, "USD");
        money.ToString().Should().Be("1,234.56 USD");
    }
}
