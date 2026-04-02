namespace Application.Common.Models;

/// <summary>
/// Realtime payload for typing indicator updates in consultation chat.
/// </summary>
public record TypingIndicatorRealtimeDto
{
    public Guid SessionId { get; init; }
    public Guid SenderProfileId { get; init; }
    public bool IsTyping { get; init; }
    public DateTime Timestamp { get; init; }
}
