namespace Application.Common.Interfaces;

public record MeetingInfo(string MeetingLink, string? CalendarEventId);

public interface IGoogleMeetService
{
    /// <summary>
    /// Creates a Google Calendar event with an auto-generated Google Meet link.
    /// </summary>
    /// <param name="title">Event title.</param>
    /// <param name="startTimeUtc">Start time in UTC.</param>
    /// <param name="durationMinutes">Duration in minutes (null = use default).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated Meet link and optional Calendar event ID.</returns>
    Task<MeetingInfo> CreateMeetingAsync(
        string title,
        DateTime startTimeUtc,
        int? durationMinutes = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a previously created Calendar event (and its Meet link).
    /// </summary>
    Task DeleteMeetingAsync(
        string calendarEventId,
        CancellationToken cancellationToken = default);
}
