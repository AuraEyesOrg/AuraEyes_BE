using Domain.ValueObjects;
using FluentAssertions;

namespace Domain.UnitTests.ValueObjects;

public class AddressTests
{
    [Fact]
    public void Constructor_ValidInput_ShouldCreateAddress()
    {
        var address = new Address("123 Main St", "Springfield", "IL", "US", "62704");

        address.Street.Should().Be("123 Main St");
        address.City.Should().Be("Springfield");
        address.State.Should().Be("IL");
        address.Country.Should().Be("US");
        address.ZipCode.Should().Be("62704");
    }

    [Theory]
    [InlineData("", "city", "Street cannot be empty")]
    [InlineData("  ", "city", "Street cannot be empty")]
    [InlineData(null, "city", "Street cannot be empty")]
    public void Constructor_EmptyStreet_ShouldThrow(string? street, string city, string expectedMessage)
    {
        var act = () => new Address(street!, city, "ST", "US", "12345");
        act.Should().Throw<ArgumentException>().WithMessage($"*{expectedMessage}*");
    }

    [Theory]
    [InlineData("street", "", "City cannot be empty")]
    [InlineData("street", "  ", "City cannot be empty")]
    [InlineData("street", null, "City cannot be empty")]
    public void Constructor_EmptyCity_ShouldThrow(string street, string? city, string expectedMessage)
    {
        var act = () => new Address(street, city!, "ST", "US", "12345");
        act.Should().Throw<ArgumentException>().WithMessage($"*{expectedMessage}*");
    }

    [Fact]
    public void Equals_SameValues_ShouldBeEqual()
    {
        var a1 = new Address("123 Main St", "Springfield", "IL", "US", "62704");
        var a2 = new Address("123 Main St", "Springfield", "IL", "US", "62704");

        a1.Should().Be(a2);
        (a1 == a2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentValues_ShouldNotBeEqual()
    {
        var a1 = new Address("123 Main St", "Springfield", "IL", "US", "62704");
        var a2 = new Address("456 Oak Ave", "Springfield", "IL", "US", "62704");

        a1.Should().NotBe(a2);
        (a1 != a2).Should().BeTrue();
    }

    [Fact]
    public void ToString_ShouldFormatCorrectly()
    {
        var address = new Address("123 Main St", "Springfield", "IL", "US", "62704");
        address.ToString().Should().Be("123 Main St, Springfield, IL 62704, US");
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeSame()
    {
        var a1 = new Address("123 Main St", "Springfield", "IL", "US", "62704");
        var a2 = new Address("123 Main St", "Springfield", "IL", "US", "62704");

        a1.GetHashCode().Should().Be(a2.GetHashCode());
    }
}
