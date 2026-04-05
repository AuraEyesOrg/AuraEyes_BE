using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities.Platform;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Infrastructure.UnitTests.Services;

public class NotificationServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task SendAsync_WithPayload_ShouldPersistAndBroadcast()
    {
        await using var ctx = CreateContext();
        var repo = new Repository<Notification>(ctx);
        var hub = Substitute.For<INotificationHubService>();
        var logger = Substitute.For<ILogger<NotificationService>>();
        var sut = new NotificationService(repo, ctx, hub, logger);
        var userId = Guid.NewGuid();

        await sut.SendAsync(
            userId,
            "T",
            "M",
            NotificationType.AiScreeningCompleted,
            new { screeningId = Guid.NewGuid().ToString() },
            CancellationToken.None);

        (await ctx.Notifications.CountAsync()).Should().Be(1);
        await hub.Received(1).BroadcastToUserAsync(userId, Arg.Any<NotificationDto>(), Arg.Any<CancellationToken>());
        await hub.Received(1).BroadcastUnreadCountAsync(userId, Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendAsync_WithSessionIdInPayload_ShouldSetReferenceId()
    {
        await using var ctx = CreateContext();
        var repo = new Repository<Notification>(ctx);
        var hub = Substitute.For<INotificationHubService>();
        var logger = Substitute.For<ILogger<NotificationService>>();
        var sut = new NotificationService(repo, ctx, hub, logger);
        var userId = Guid.NewGuid();
        var refId = Guid.NewGuid();

        await sut.SendAsync(
            userId,
            "T",
            "M",
            NotificationType.NewConsultationRequest,
            new { sessionId = refId.ToString() },
            CancellationToken.None);

        var n = await ctx.Notifications.FirstAsync();
        n.ReferenceId.Should().Be(refId);
    }

    [Fact]
    public async Task SendAsync_WithoutPayload_ShouldPersistWithoutPayload()
    {
        await using var ctx = CreateContext();
        var repo = new Repository<Notification>(ctx);
        var hub = Substitute.For<INotificationHubService>();
        var logger = Substitute.For<ILogger<NotificationService>>();
        var sut = new NotificationService(repo, ctx, hub, logger);
        var userId = Guid.NewGuid();

        await sut.SendAsync(userId, "T", "M", NotificationType.NewConsultationRequest, null, CancellationToken.None);

        var n = await ctx.Notifications.FirstAsync();
        n.Payload.Should().BeNull();
    }

    [Fact]
    public async Task SendAsync_NestedPayloadWithoutGuidKeys_ShouldLeaveReferenceIdNull()
    {
        await using var ctx = CreateContext();
        var repo = new Repository<Notification>(ctx);
        var hub = Substitute.For<INotificationHubService>();
        var logger = Substitute.For<ILogger<NotificationService>>();
        var sut = new NotificationService(repo, ctx, hub, logger);
        var userId = Guid.NewGuid();

        await sut.SendAsync(
            userId,
            "T",
            "M",
            NotificationType.NewConsultationRequest,
            new { x = new { nested = true } },
            CancellationToken.None);

        var n = await ctx.Notifications.FirstAsync();
        n.ReferenceId.Should().BeNull();
    }

#pragma warning disable CS0618
    [Fact]
    public async Task LegacySendAsync_ShouldRouteToTypedNotification()
    {
        await using var ctx = CreateContext();
        var repo = new Repository<Notification>(ctx);
        var hub = Substitute.For<INotificationHubService>();
        var logger = Substitute.For<ILogger<NotificationService>>();
        var sut = new NotificationService(repo, ctx, hub, logger);
        var userId = Guid.NewGuid();

        await sut.SendAsync(userId, "Hello legacy", CancellationToken.None);

        var n = await ctx.Notifications.FirstAsync();
        n.Type.Should().Be(NotificationType.NewConsultationRequest);
        n.Message.Should().Be("Hello legacy");
    }
#pragma warning restore CS0618
}
