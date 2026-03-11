using Application.Common.Behaviors;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Application.UnitTests.Behaviors;

public class LoggingBehaviorTests
{
    [Fact]
    public async Task Handle_ShouldLogBeforeAndAfterExecution()
    {
        // Arrange
        var logger = Substitute.For<ILogger<TestRequest>>();
        var behavior = new LoggingBehavior<TestRequest, TestResponse>(logger);
        var request = new TestRequest();
        var expectedResponse = new TestResponse("ok");

        // Act
        var result = await behavior.Handle(request, () => Task.FromResult(expectedResponse), CancellationToken.None);

        // Assert
        result.Should().Be(expectedResponse);

        // Verify two "Information" log calls were made (Handling + Handled)
        logger.Received(2).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception?>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async Task Handle_ShouldReturnResponseFromNext()
    {
        // Arrange
        var logger = Substitute.For<ILogger<TestRequest>>();
        var behavior = new LoggingBehavior<TestRequest, TestResponse>(logger);
        var expectedResponse = new TestResponse("expected");

        // Act
        var result = await behavior.Handle(new TestRequest(), () => Task.FromResult(expectedResponse), CancellationToken.None);

        // Assert
        result.Should().Be(expectedResponse);
    }

    // Test types
    public record TestRequest : IRequest<TestResponse>;
    public record TestResponse(string Value);
}
