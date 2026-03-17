namespace Application.Common.Models;

/// <summary>
/// Lightweight realtime event payload for consultation chat updates.
/// </summary>
public record ChatMessageRealtimeDto
{
    public Guid SessionId { get; init; }
    public Guid MessageId { get; init; }
    public Guid SenderProfileId { get; init; }
    public DateTime SentAt { get; init; }
}
