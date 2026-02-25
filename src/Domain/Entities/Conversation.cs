using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Conversation entity - chat conversation between doctor and patient.
/// Now linked to ConsultationSession instead of ConsultationRequest.
/// </summary>
public class Conversation : BaseEntity, IAggregateRoot
{
    public Guid OphthalmologistId { get; private set; }
    public Guid ConsultationSessionId { get; private set; }

    // Navigation properties
    private readonly List<ChatMessage> _messages = new();
    public IReadOnlyCollection<ChatMessage> Messages => _messages.AsReadOnly();

    private Conversation() { } // EF Core

    public Conversation(Guid ophthalmologistId, Guid consultationSessionId)
    {
        OphthalmologistId = ophthalmologistId;
        ConsultationSessionId = consultationSessionId;
    }

    public void AddMessage(ChatMessage message)
    {
        _messages.Add(message);
        UpdatedAt = DateTime.UtcNow;
    }
}
