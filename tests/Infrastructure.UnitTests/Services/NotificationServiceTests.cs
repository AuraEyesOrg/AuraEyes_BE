using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities.Platform;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Infrastructure.UnitTests.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitTests.Services;

public class NotificationServiceTests
{
    [Fact]
    public async Task SendAsync_Typed_ShouldPersistAndBroadcastAndCountUnread()
    {
        await using var context = CreateContext();
        var repo = new Repository<Notification>(context);
        var hub = new FakeNotificationHubService();
        var logger = new TestLogger<NotificationService>();
        var service = new NotificationService(repo, context, hub, logger);
        var userId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();

        await service.SendAsync(
            userId,
            "New Session",
            "Session created",
            NotificationType.NewConsultationRequest,
            new { sessionId },
            CancellationToken.None);

        var persisted = await context.Notifications.FirstAsync();
        persisted.UserId.Should().Be(userId);
        persisted.Title.Should().Be("New Session");
        persisted.ReferenceId.Should().Be(sessionId);
        hub.BroadcastedToUser.Should().HaveCount(1);
        hub.UnreadCountEvents.Should().ContainSingle(x => x.UserId == userId && x.Count == 1);
    }

    [Fact]
    public async Task SendAsync_Typed_WithAppointmentIdPayload_ShouldExtractReferenceId()
    {
        await using var context = CreateContext();
        var repo = new Repository<Notification>(context);
        var hub = new FakeNotificationHubService();
        var logger = new TestLogger<NotificationService>();
        var service = new NotificationService(repo, context, hub, logger);
        var userId = Guid.NewGuid();
        var appointmentId = Guid.NewGuid();

        await service.SendAsync(
            userId,
            "Appointment Updated",
            "Your appointment was updated",
            NotificationType.SystemAlert,
            new { appointmentId },
            CancellationToken.None);

        var persisted = await context.Notifications.FirstAsync();
        persisted.ReferenceId.Should().Be(appointmentId);
    }

    [Fact]
    public async Task SendAsync_Typed_WithMalformedStringPayload_ShouldKeepReferenceIdNull()
    {
        await using var context = CreateContext();
        var repo = new Repository<Notification>(context);
        var hub = new FakeNotificationHubService();
        var logger = new TestLogger<NotificationService>();
        var service = new NotificationService(repo, context, hub, logger);
        var userId = Guid.NewGuid();

        // payload is string => serialized JSON string, extractor should ignore
        await service.SendAsync(
            userId,
            "Text Payload",
            "payload is text",
            NotificationType.SystemAlert,
            "not-an-object-json",
            CancellationToken.None);

        var persisted = await context.Notifications.FirstAsync();
        persisted.ReferenceId.Should().BeNull();
    }

    [Theory]
    [InlineData("consultationId")]
    [InlineData("sessionId")]
    [InlineData("appointmentId")]
    [InlineData("screeningId")]
    [InlineData("aiScreeningId")]
    [InlineData("transactionId")]
    [InlineData("messageId")]
    public async Task SendAsync_Typed_ShouldExtractReferenceId_FromKnownPayloadKeys(string keyName)
    {
        await using var context = CreateContext();
        var repo = new Repository<Notification>(context);
        var hub = new FakeNotificationHubService();
        var logger = new TestLogger<NotificationService>();
        var service = new NotificationService(repo, context, hub, logger);
        var userId = Guid.NewGuid();
        var refId = Guid.NewGuid();
        var payload = new Dictionary<string, string> { [keyName] = refId.ToString() };

        await service.SendAsync(
            userId,
            "Ref Test",
            "testing known keys",
            NotificationType.SystemAlert,
            payload,
            CancellationToken.None);

        var persisted = await context.Notifications.FirstAsync();
        persisted.ReferenceId.Should().Be(refId);
    }

    [Fact]
    public async Task SendAsync_Legacy_ShouldUseDefaultTitleAndType()
    {
        await using var context = CreateContext();
        var repo = new Repository<Notification>(context);
        var hub = new FakeNotificationHubService();
        var logger = new TestLogger<NotificationService>();
        var service = new NotificationService(repo, context, hub, logger);
        var userId = Guid.NewGuid();

        await service.SendAsync(userId, "Legacy message");

        var persisted = await context.Notifications.FirstAsync();
        persisted.Title.Should().Be("Session Reminder");
        persisted.Type.Should().Be(NotificationType.NewConsultationRequest);
    }

    [Fact]
    public async Task SendAsync_Typed_WithExplicitReferenceId_ShouldPreferExplicitOverPayload()
    {
        await using var context = CreateContext();
        var repo = new Repository<Notification>(context);
        var hub = new FakeNotificationHubService();
        var service = new NotificationService(repo, context, hub, new TestLogger<NotificationService>());
        var userId = Guid.NewGuid();
        var explicitRef = Guid.NewGuid();
        var payloadRef = Guid.NewGuid();

        await service.SendAsync(
            userId,
            "Priority Ref",
            "explicit ref id wins",
            NotificationType.SystemAlert,
            new { appointmentId = payloadRef },
            CancellationToken.None,
            explicitRef);

        var saved = await context.Notifications.FirstAsync();
        saved.ReferenceId.Should().Be(explicitRef);
    }

    [Fact]
    public async Task SendAsync_Typed_ShouldBroadcastUnreadCountIncludingExistingUnreadNotifications()
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        await context.Notifications.AddRangeAsync(
            new Notification(userId, "n1", "m1", NotificationType.SystemAlert, null, null),
            new Notification(userId, "n2", "m2", NotificationType.SystemAlert, null, null));
        await context.SaveChangesAsync();

        var repo = new Repository<Notification>(context);
        var hub = new FakeNotificationHubService();
        var service = new NotificationService(repo, context, hub, new TestLogger<NotificationService>());

        await service.SendAsync(userId, "n3", "m3", NotificationType.SystemAlert, null, CancellationToken.None);

        hub.UnreadCountEvents.Should().ContainSingle(x => x.UserId == userId && x.Count == 3);
    }

    [Fact]
    public async Task SendAsync_WhenHubBroadcastFails_ShouldThrowButKeepPersistedNotification()
    {
        await using var context = CreateContext();
        var repo = new Repository<Notification>(context);
        var service = new NotificationService(
            repo,
            context,
            new ThrowingNotificationHubService(),
            new TestLogger<NotificationService>());
        var userId = Guid.NewGuid();

        var act = async () => await service.SendAsync(
            userId,
            "Hub Fail",
            "broadcast fails",
            NotificationType.SystemAlert,
            null,
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        (await context.Notifications.CountAsync()).Should().Be(1);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private sealed class FakeNotificationHubService : INotificationHubService
    {
        public List<(Guid UserId, NotificationDto Notification)> BroadcastedToUser { get; } = [];
        public List<(Guid UserId, int Count)> UnreadCountEvents { get; } = [];

        public Task BroadcastToUserAsync(Guid userId, NotificationDto notification, CancellationToken cancellationToken = default)
        {
            BroadcastedToUser.Add((userId, notification));
            return Task.CompletedTask;
        }

        public Task BroadcastUnreadCountAsync(Guid userId, int count, CancellationToken cancellationToken = default)
        {
            UnreadCountEvents.Add((userId, count));
            return Task.CompletedTask;
        }
    }

    private sealed class ThrowingNotificationHubService : INotificationHubService
    {
        public Task BroadcastToUserAsync(Guid userId, NotificationDto notification, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("hub down");

        public Task BroadcastUnreadCountAsync(Guid userId, int count, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
