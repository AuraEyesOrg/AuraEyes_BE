namespace Application.Common.Interfaces;

public record MeetingInfo(string MeetingLink, string? CalendarEventId);

public interface IGoogleMeetService
{
    /// <summary>
    /// Creates a Google Calendar event with an auto-generated Google Meet link.
    /// Attendees are added so they can join without "Ask to join".
    /// </summary>
    /// <param name="title">Event title.</param>
    /// <param name="startTimeUtc">Start time in UTC.</param>
    /// <param name="attendeeEmails">Emails of participants (patient + doctor). They join Meet directly.</param>
    /// <param name="durationMinutes">Duration in minutes (null = use default).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<MeetingInfo> CreateMeetingAsync(
        string title,
        DateTime startTimeUtc,
        IReadOnlyList<string>? attendeeEmails = null,
        int? durationMinutes = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a previously created Calendar event (and its Meet link).
    /// </summary>
    Task DeleteMeetingAsync(
        string calendarEventId,
        CancellationToken cancellationToken = default);
}
