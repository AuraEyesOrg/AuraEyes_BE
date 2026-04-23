using Domain.Common;
using Domain.Enums.Network;

namespace Domain.Entities.Network.InternalChat;

public class InternalGroupMessage : BaseEntity, IAggregateRoot
{
    public Guid GroupId { get; private set; }
    public Guid SenderId { get; private set; }
    public AuthorType SenderType { get; private set; }
    public string Content { get; private set; } = string.Empty;
    
    // Navigation property
    public InternalGroupChat Group { get; private set; } = null!;

    private InternalGroupMessage() { } // EF Core
    
    public InternalGroupMessage(Guid groupId, Guid senderId, AuthorType senderType, string content)
    {
        GroupId = groupId;
        SenderId = senderId;
        SenderType = senderType;
        Content = content;
    }
}
