using Application.Common.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class GoogleMeetService : IGoogleMeetService, IDisposable
{
    private const int MaxMeetLinkRetries = 3;
    private static readonly TimeSpan InitialRetryDelay = TimeSpan.FromSeconds(2);

    private readonly CalendarService _calendarService;
    private readonly GoogleMeetSettings _settings;
    private readonly ILogger<GoogleMeetService> _logger;

    public GoogleMeetService(
        IOptions<GoogleMeetSettings> settings,
        ILogger<GoogleMeetService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        _calendarService = CreateCalendarService();
    }

    public async Task<MeetingInfo> CreateMeetingAsync(
        string title,
        DateTime startTimeUtc,
        IReadOnlyList<string>? attendeeEmails = null,
        int? durationMinutes = null,
        CancellationToken cancellationToken = default)
    {
        var duration = durationMinutes ?? _settings.DefaultDurationMinutes;
        var endTimeUtc = startTimeUtc.AddMinutes(duration);

        var calendarEvent = new Event
        {
            Summary = title,
            Description = $"AURA Eyes Telemedicine • Join meeting on AURA Eyes Telemedicine",
            Start = new EventDateTime
            {
                DateTimeDateTimeOffset = new DateTimeOffset(startTimeUtc, TimeSpan.Zero),
                TimeZone = "UTC"
            },
            End = new EventDateTime
            {
                DateTimeDateTimeOffset = new DateTimeOffset(endTimeUtc, TimeSpan.Zero),
                TimeZone = "UTC"
            },
            ConferenceData = new ConferenceData
            {
                CreateRequest = new CreateConferenceRequest
                {
                    RequestId = Guid.NewGuid().ToString(),
                    ConferenceSolutionKey = new ConferenceSolutionKey
                    {
                        Type = "hangoutsMeet"
                    }
                }
            },
            GuestsCanInviteOthers = false,
            GuestsCanModify = false
        };

        if (attendeeEmails is { Count: > 0 })
        {
            calendarEvent.Attendees = attendeeEmails
                .Select(email => new EventAttendee
                {
                    Email = email,
                    ResponseStatus = "accepted"
                })
                .ToList();
        }

        try
        {
            var request = _calendarService.Events.Insert(calendarEvent, _settings.CalendarId);
            request.ConferenceDataVersion = 1;
            request.SendUpdates = EventsResource.InsertRequest.SendUpdatesEnum.All;

            var createdEvent = await request.ExecuteAsync(cancellationToken);

            var meetLink = ExtractMeetLink(createdEvent);

            if (string.IsNullOrEmpty(meetLink))
            {
                meetLink = await RetryGetMeetLinkAsync(createdEvent.Id, cancellationToken);
            }

            if (string.IsNullOrEmpty(meetLink))
            {
                await CleanupOrphanedEventAsync(createdEvent.Id, cancellationToken);

                throw new InvalidOperationException(
                    "Failed to generate Google Meet link after retries. " +
                    "The orphaned calendar event has been cleaned up. " +
                    "Ensure the Google Workspace account has Google Meet enabled.");
            }

            _logger.LogInformation(
                "Google Meet created: {MeetLink} (event {EventId}, attendees: {Attendees})",
                meetLink, createdEvent.Id, string.Join(", ", attendeeEmails ?? []));

            return new MeetingInfo(meetLink, createdEvent.Id);
        }
        catch (Google.GoogleApiException ex)
        {
            _logger.LogError(ex, "Google API error occurred while creating meeting: {Message}. Detail: {ErrorDetail}", 
                ex.Message, ex.Error?.ToString());
            
            if (ex.Error?.Errors?.Any(e => e.Reason == "authError" || e.Message.Contains("invalid_grant")) == true)
            {
                throw new InvalidOperationException("Google authentication failed (invalid_grant). Please check if the RefreshToken is valid.", ex);
            }
            
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while creating Google Meet meeting");
            throw;
        }
    }

    public async Task DeleteMeetingAsync(
        string calendarEventId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _calendarService.Events
                .Delete(_settings.CalendarId, calendarEventId)
                .ExecuteAsync(cancellationToken);

            _logger.LogInformation("Deleted Calendar event {EventId}", calendarEventId);
        }
        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Calendar event {EventId} already deleted", calendarEventId);
        }
    }

    /// <summary>
    /// Google sometimes populates conference data asynchronously.
    /// Re-fetch the event with exponential backoff to wait for the Meet link.
    /// </summary>
    private async Task<string?> RetryGetMeetLinkAsync(string eventId, CancellationToken cancellationToken)
    {
        var delay = InitialRetryDelay;

        for (var attempt = 1; attempt <= MaxMeetLinkRetries; attempt++)
        {
            _logger.LogDebug(
                "Meet link not yet available for event {EventId}, retry {Attempt}/{Max} after {Delay}ms",
                eventId, attempt, MaxMeetLinkRetries, delay.TotalMilliseconds);

            await Task.Delay(delay, cancellationToken);
            delay *= 2;

            var refreshed = await _calendarService.Events
                .Get(_settings.CalendarId, eventId)
                .ExecuteAsync(cancellationToken);

            var link = ExtractMeetLink(refreshed);
            if (!string.IsNullOrEmpty(link))
                return link;
        }

        return null;
    }

    private async Task CleanupOrphanedEventAsync(string eventId, CancellationToken cancellationToken)
    {
        try
        {
            await _calendarService.Events
                .Delete(_settings.CalendarId, eventId)
                .ExecuteAsync(cancellationToken);

            _logger.LogWarning(
                "Deleted orphaned calendar event {EventId} (no Meet link after retries)", eventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to delete orphaned calendar event {EventId}. Manual cleanup required.", eventId);
        }
    }

    private static string? ExtractMeetLink(Event calendarEvent)
    {
        return calendarEvent.ConferenceData?.EntryPoints?
            .FirstOrDefault(ep => ep.EntryPointType == "video")?.Uri;
    }

    private CalendarService CreateCalendarService()
    {
        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = _settings.ClientId,
                ClientSecret = _settings.ClientSecret
            },
            Scopes = [CalendarService.Scope.Calendar]
        });

        var credential = new UserCredential(flow, "user", new TokenResponse
        {
            RefreshToken = _settings.RefreshToken
        });

        return new CalendarService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = _settings.ApplicationName
        });
    }

    public void Dispose()
    {
        _calendarService.Dispose();
    }
}
