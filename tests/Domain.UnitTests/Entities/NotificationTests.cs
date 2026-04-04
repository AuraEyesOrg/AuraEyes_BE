using Domain.Entities.Platform;
using Domain.Enums;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class NotificationTests
{
    private readonly Guid _userId = Guid.NewGuid();

    private Notification CreateValidNotification(
        string title = "Test Title",
        string message = "Test message body")
    {
        return new Notification(_userId, title, message, NotificationType.SystemAlert);
    }

    #region Constructor

    [Fact]
    public void Constructor_ValidInput_ShouldCreateNotification()
    {
        var refId = Guid.NewGuid();
        var payload = "{\"key\":\"value\"}";

        var notification = new Notification(
            _userId,
            "Screening Complete",
            "Your AI screening is done.",
            NotificationType.AiScreeningCompleted,
            refId,
            payload);

        notification.UserId.Should().Be(_userId);
        notification.Title.Should().Be("Screening Complete");
        notification.Message.Should().Be("Your AI screening is done.");
        notification.Type.Should().Be(NotificationType.AiScreeningCompleted);
        notification.ReferenceId.Should().Be(refId);
        notification.IsRead.Should().BeFalse();
        notification.Payload.Should().Be(payload);
    }

    [Fact]
    public void Constructor_MinimalInput_ShouldUseDefaults()
    {
        var notification = new Notification(_userId, "Title", "Message", NotificationType.SystemAlert);

        notification.ReferenceId.Should().BeNull();
        notification.Payload.Should().BeNull();
        notification.IsRead.Should().BeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyTitle_ShouldThrow(string? title)
    {
        var act = () => new Notification(_userId, title!, "msg", NotificationType.SystemAlert);

        act.Should().Throw<ArgumentException>().WithParameterName("title");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyMessage_ShouldThrow(string? message)
    {
        var act = () => new Notification(_userId, "Title", message!, NotificationType.SystemAlert);

        act.Should().Throw<ArgumentException>().WithParameterName("message");
    }

    #endregion

    #region MarkAsRead / MarkAsUnread

    [Fact]
    public void MarkAsRead_ShouldSetIsReadTrue()
    {
        var notification = CreateValidNotification();

        notification.MarkAsRead();

        notification.IsRead.Should().BeTrue();
        notification.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkAsUnread_ShouldSetIsReadFalse()
    {
        var notification = CreateValidNotification();
        notification.MarkAsRead();

        notification.MarkAsUnread();

        notification.IsRead.Should().BeFalse();
        notification.UpdatedAt.Should().NotBeNull();
    }

    #endregion
}
