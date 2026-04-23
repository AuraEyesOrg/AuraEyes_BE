using Domain.Common;
using Domain.Enums.Network;

namespace Domain.Entities.Network.InternalChat;

public class InternalGroupMember : BaseEntity
{
    public Guid GroupId { get; private set; }
    public Guid MemberId { get; private set; }
    public AuthorType MemberType { get; private set; }
    
    // Navigation property
    public InternalGroupChat Group { get; private set; } = null!;

    private InternalGroupMember() { } // EF Core
    
    internal InternalGroupMember(Guid groupId, Guid memberId, AuthorType memberType)
    {
        GroupId = groupId;
        MemberId = memberId;
        MemberType = memberType;
    }
}
