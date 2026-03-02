namespace Infrastructure.Settings;

public sealed class GoogleMeetSettings
{
    public const string SectionName = "GoogleMeet";

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public string CalendarId { get; set; } = "primary";

    public string ApplicationName { get; set; } = "AURA Telemedicine";

    /// <summary>
    /// Default duration of a video call meeting in minutes.
    /// </summary>
    public int DefaultDurationMinutes { get; set; } = 30;
}
