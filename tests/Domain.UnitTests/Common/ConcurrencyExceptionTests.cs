using Domain.Common;
using FluentAssertions;

namespace Domain.UnitTests.Common;

public class ConcurrencyExceptionTests
{
    [Fact]
    public void DefaultConstructor_ShouldHaveDefaultMessage()
    {
        var ex = new ConcurrencyException();
        ex.Message.Should().Be("A concurrency conflict occurred.");
    }

    [Fact]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        var ex = new ConcurrencyException("Custom message");
        ex.Message.Should().Be("Custom message");
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_ShouldSetBoth()
    {
        var inner = new InvalidOperationException("inner");
        var ex = new ConcurrencyException("outer", inner);

        ex.Message.Should().Be("outer");
        ex.InnerException.Should().Be(inner);
    }
}
