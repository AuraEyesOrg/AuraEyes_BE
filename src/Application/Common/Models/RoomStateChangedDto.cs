namespace Application.Common.Models;

/// <summary>
/// Realtime event payload broadcast when a consultation room transitions state.
/// Sent to both participants so the client UI can react immediately.
/// </summary>
public record RoomStateChangedDto
{
    public Guid SessionId { get; init; }

    /// <summary>"ROOM_OPENED" or "ROOM_CLOSED"</summary>
    public string Event { get; init; } = string.Empty;

    public DateTime Timestamp { get; init; }
}
