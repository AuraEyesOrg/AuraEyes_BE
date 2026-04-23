using Domain.Common;
using Domain.Enums.Network;

namespace Domain.Entities.Network.InternalChat;

public enum InternalGroupType
{
    General,
    ClinicalCase
}

public class InternalGroupChat : BaseEntity, IAggregateRoot
{
    public string? Name { get; private set; }
    public InternalGroupType Type { get; private set; }
    
    public Guid? ConsultationSessionId { get; private set; }
    
    public string? MeetingLink { get; private set; }
    public string? CalendarEventId { get; private set; }
    
    public Guid CreatorId { get; private set; }
    public AuthorType CreatorType { get; private set; }
    
    private readonly List<InternalGroupMember> _members = new();
    public IReadOnlyCollection<InternalGroupMember> Members => _members.AsReadOnly();
    
    private readonly List<InternalGroupMessage> _messages = new();
    public IReadOnlyCollection<InternalGroupMessage> Messages => _messages.AsReadOnly();

    private InternalGroupChat() { } // EF Core
    
    public InternalGroupChat(string? name, InternalGroupType type, Guid creatorId, AuthorType creatorType, Guid? consultationSessionId = null)
    {
        Name = name;
        Type = type;
        CreatorId = creatorId;
        CreatorType = creatorType;
        ConsultationSessionId = consultationSessionId;
    }
    
    public void SetMeetingInfo(string meetingLink, string? calendarEventId = null)
    {
        MeetingLink = meetingLink;
        CalendarEventId = calendarEventId;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void AddMember(Guid memberId, AuthorType memberType)
    {
        if (!_members.Any(m => m.MemberId == memberId))
        {
            _members.Add(new InternalGroupMember(Id, memberId, memberType));
            UpdatedAt = DateTime.UtcNow;
        }
    }
    
    public void RemoveMember(Guid memberId)
    {
        var member = _members.FirstOrDefault(m => m.MemberId == memberId);
        if (member != null)
        {
            _members.Remove(member);
            UpdatedAt = DateTime.UtcNow;
        }
    }
    
    public void AddMessage(InternalGroupMessage message)
    {
        _messages.Add(message);
        UpdatedAt = DateTime.UtcNow;
    }
}
