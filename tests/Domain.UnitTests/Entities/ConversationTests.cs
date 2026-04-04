using Domain.Entities.Consultation;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class ConversationTests
{
    private readonly Guid _ophthalmologistId = Guid.NewGuid();
    private readonly Guid _sessionId = Guid.NewGuid();

    [Fact]
    public void Constructor_ValidInput_ShouldCreateConversation()
    {
        var conversation = new Conversation(_ophthalmologistId, _sessionId);

        conversation.OphthalmologistId.Should().Be(_ophthalmologistId);
        conversation.ConsultationSessionId.Should().Be(_sessionId);
        conversation.Messages.Should().BeEmpty();
        conversation.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void AddMessage_SingleMessage_ShouldAddToCollection()
    {
        var conversation = new Conversation(_ophthalmologistId, _sessionId);
        var chatMessage = new ChatMessage(conversation.Id, Guid.NewGuid(), "Hello");

        conversation.AddMessage(chatMessage);

        conversation.Messages.Should().HaveCount(1);
        conversation.Messages.Should().Contain(chatMessage);
        conversation.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void AddMessage_MultipleMessages_ShouldAddAll()
    {
        var conversation = new Conversation(_ophthalmologistId, _sessionId);
        var msg1 = new ChatMessage(conversation.Id, Guid.NewGuid(), "Hello");
        var msg2 = new ChatMessage(conversation.Id, Guid.NewGuid(), "Hi there");
        var msg3 = new ChatMessage(conversation.Id, Guid.NewGuid(), "How are you?");

        conversation.AddMessage(msg1);
        conversation.AddMessage(msg2);
        conversation.AddMessage(msg3);

        conversation.Messages.Should().HaveCount(3);
        conversation.Messages.Should().ContainInOrder(msg1, msg2, msg3);
    }

    [Fact]
    public void Messages_ShouldBeReadOnlyCollection()
    {
        var conversation = new Conversation(_ophthalmologistId, _sessionId);

        conversation.Messages.Should().BeAssignableTo<IReadOnlyCollection<ChatMessage>>();
    }

    [Fact]
    public void Constructor_EmptyGuids_ShouldStillCreate()
    {
        var conversation = new Conversation(Guid.Empty, Guid.Empty);

        conversation.OphthalmologistId.Should().Be(Guid.Empty);
        conversation.ConsultationSessionId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void AddMessage_ShouldUpdateTimestamp()
    {
        var conversation = new Conversation(_ophthalmologistId, _sessionId);
        var beforeAdd = DateTime.UtcNow;

        conversation.AddMessage(new ChatMessage(conversation.Id, Guid.NewGuid(), "Test"));

        conversation.UpdatedAt.Should().NotBeNull();
        conversation.UpdatedAt.Should().BeOnOrAfter(beforeAdd);
    }
}
