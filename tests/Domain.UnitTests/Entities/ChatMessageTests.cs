using Domain.Entities.Consultation;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class ChatMessageTests
{
    private readonly Guid _conversationId = Guid.NewGuid();
    private readonly Guid _senderUserId = Guid.NewGuid();
    private const string ValidMessage = "Hello, doctor!";

    [Fact]
    public void Constructor_ValidInput_ShouldCreateChatMessage()
    {
        var message = new ChatMessage(_conversationId, _senderUserId, ValidMessage);

        message.ConversationId.Should().Be(_conversationId);
        message.SenderUserId.Should().Be(_senderUserId);
        message.Message.Should().Be(ValidMessage);
        message.IsRead.Should().BeFalse();
        message.SentAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        message.Id.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyOrWhitespaceMessage_ShouldThrow(string? invalidMessage)
    {
        var act = () => new ChatMessage(_conversationId, _senderUserId, invalidMessage!);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("message");
    }

    [Fact]
    public void Constructor_EmptyConversationId_ShouldStillCreate()
    {
        var message = new ChatMessage(Guid.Empty, _senderUserId, ValidMessage);

        message.ConversationId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void Constructor_EmptySenderUserId_ShouldStillCreate()
    {
        var message = new ChatMessage(_conversationId, Guid.Empty, ValidMessage);

        message.SenderUserId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void MarkAsRead_ShouldSetIsReadToTrue()
    {
        var message = new ChatMessage(_conversationId, _senderUserId, ValidMessage);

        message.MarkAsRead();

        message.IsRead.Should().BeTrue();
        message.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkAsRead_CalledTwice_ShouldRemainTrue()
    {
        var message = new ChatMessage(_conversationId, _senderUserId, ValidMessage);

        message.MarkAsRead();
        message.MarkAsRead();

        message.IsRead.Should().BeTrue();
    }

    [Fact]
    public void Constructor_LongMessage_ShouldCreate()
    {
        var longMessage = new string('x', 10_000);

        var message = new ChatMessage(_conversationId, _senderUserId, longMessage);

        message.Message.Should().HaveLength(10_000);
    }
}
