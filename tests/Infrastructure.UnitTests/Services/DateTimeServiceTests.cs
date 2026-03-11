using FluentAssertions;
using Infrastructure.Services;

namespace Infrastructure.UnitTests.Services;

public class DateTimeServiceTests
{
    private readonly DateTimeService _sut = new();

    [Fact]
    public void Now_ShouldReturnCurrentLocalTime()
    {
        // Act
        var before = DateTime.Now;
        var result = _sut.Now;
        var after = DateTime.Now;

        // Assert
        result.Should().BeOnOrAfter(before);
        result.Should().BeOnOrBefore(after);
    }

    [Fact]
    public void UtcNow_ShouldReturnCurrentUtcTime()
    {
        // Act
        var before = DateTime.UtcNow;
        var result = _sut.UtcNow;
        var after = DateTime.UtcNow;

        // Assert
        result.Should().BeOnOrAfter(before);
        result.Should().BeOnOrBefore(after);
    }

    [Fact]
    public void Now_ShouldReturnLocalKind()
    {
        var result = _sut.Now;
        result.Kind.Should().Be(DateTimeKind.Local);
    }

    [Fact]
    public void UtcNow_ShouldReturnUtcKind()
    {
        var result = _sut.UtcNow;
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void Now_ConsecutiveCalls_ShouldBeNonDecreasing()
    {
        var first = _sut.Now;
        var second = _sut.Now;

        second.Should().BeOnOrAfter(first);
    }

    [Fact]
    public void UtcNow_ConsecutiveCalls_ShouldBeNonDecreasing()
    {
        var first = _sut.UtcNow;
        var second = _sut.UtcNow;

        second.Should().BeOnOrAfter(first);
    }
}
