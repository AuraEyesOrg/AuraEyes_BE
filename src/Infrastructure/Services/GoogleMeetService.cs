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
        int? durationMinutes = null,
        CancellationToken cancellationToken = default)
    {
        var duration = durationMinutes ?? _settings.DefaultDurationMinutes;
        var endTimeUtc = startTimeUtc.AddMinutes(duration);

        var calendarEvent = new Event
        {
            Summary = title,
            Description = $"AURA Telemedicine – {title}",
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
            }
        };

        var request = _calendarService.Events.Insert(calendarEvent, _settings.CalendarId);
        request.ConferenceDataVersion = 1;

        var createdEvent = await request.ExecuteAsync(cancellationToken);

        var meetLink = createdEvent.ConferenceData?.EntryPoints?
            .FirstOrDefault(ep => ep.EntryPointType == "video")?.Uri;

        if (string.IsNullOrEmpty(meetLink))
        {
            _logger.LogWarning(
                "Calendar event {EventId} created but no Meet link generated",
                createdEvent.Id);

            throw new InvalidOperationException(
                "Failed to generate Google Meet link. Ensure the Gmail account has Google Meet enabled.");
        }

        _logger.LogInformation(
            "Google Meet created: {MeetLink} (event {EventId})",
            meetLink, createdEvent.Id);

        return new MeetingInfo(meetLink, createdEvent.Id);
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
