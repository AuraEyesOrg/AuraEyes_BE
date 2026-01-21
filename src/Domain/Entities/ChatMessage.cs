using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Chat Message entity - individual message in a conversation
/// </summary>
public class ChatMessage : BaseEntity
{
    public Guid ConversationId { get; private set; }
    public Guid SenderUserId { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public bool IsRead { get; private set; }
    public DateTime SentAt { get; private set; }

    private ChatMessage() { } // EF Core

    public ChatMessage(Guid conversationId, Guid senderUserId, string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be empty", nameof(message));

        ConversationId = conversationId;
        SenderUserId = senderUserId;
        Message = message;
        IsRead = false;
        SentAt = DateTime.UtcNow;
    }

    public void MarkAsRead()
    {
        IsRead = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
