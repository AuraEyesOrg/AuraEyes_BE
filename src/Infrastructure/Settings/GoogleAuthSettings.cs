namespace Infrastructure.Settings;

/// <summary>
/// Google OAuth authentication settings for ID token validation.
/// </summary>
public class GoogleAuthSettings
{
    public const string SectionName = "GoogleAuth";

    /// <summary>
    /// Google OAuth Client ID used to validate ID tokens.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;
}
