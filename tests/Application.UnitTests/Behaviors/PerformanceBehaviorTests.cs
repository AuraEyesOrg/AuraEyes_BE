using Application.Common.Behaviors;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Application.UnitTests.Behaviors;

public class PerformanceBehaviorTests
{
    [Fact]
    public async Task Handle_FastRequest_ShouldNotLogWarning()
    {
        // Arrange
        var logger = Substitute.For<ILogger<TestRequest>>();
        var behavior = new PerformanceBehavior<TestRequest, TestResponse>(logger);
        var request = new TestRequest();
        var expectedResponse = new TestResponse("ok");

        // Act
        var result = await behavior.Handle(request, () => Task.FromResult(expectedResponse), CancellationToken.None);

        // Assert
        result.Should().Be(expectedResponse);
        logger.DidNotReceive().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception?>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async Task Handle_SlowRequest_ShouldLogWarning()
    {
        // Arrange
        var logger = Substitute.For<ILogger<TestRequest>>();
        var behavior = new PerformanceBehavior<TestRequest, TestResponse>(logger);
        var request = new TestRequest();
        var expectedResponse = new TestResponse("ok");

        // Act
        var result = await behavior.Handle(request, async () =>
        {
            await Task.Delay(600); // >500ms threshold
            return expectedResponse;
        }, CancellationToken.None);

        // Assert
        result.Should().Be(expectedResponse);
        logger.Received().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception?>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    // Test types
    public record TestRequest : IRequest<TestResponse>;
    public record TestResponse(string Value);
}
