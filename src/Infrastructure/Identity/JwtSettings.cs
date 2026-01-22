namespace Infrastructure.Identity;

/// <summary>
/// JWT configuration settings.
/// Bound from appsettings.json section "JwtSettings".
/// </summary>
public class JwtSettings
{
    public const string SectionName = "JwtSettings";
    
    /// <summary>
    /// Secret key for signing tokens. Must be at least 32 characters.
    /// In production, use Azure Key Vault or similar.
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Token issuer (your API).
    /// </summary>
    public string Issuer { get; set; } = string.Empty;
    
    /// <summary>
    /// Token audience (your client apps).
    /// </summary>
    public string Audience { get; set; } = string.Empty;
    
    /// <summary>
    /// Access token expiry in minutes. Default: 15 minutes.
    /// Short-lived for security.
    /// </summary>
    public int AccessTokenExpiryMinutes { get; set; } = 15;
    
    /// <summary>
    /// Refresh token expiry in days. Default: 7 days.
    /// </summary>
    public int RefreshTokenExpiryDays { get; set; } = 7;
    
    /// <summary>
    /// Whether to validate issuer. Default: true.
    /// </summary>
    public bool ValidateIssuer { get; set; } = true;
    
    /// <summary>
    /// Whether to validate audience. Default: true.
    /// </summary>
    public bool ValidateAudience { get; set; } = true;
    
    /// <summary>
    /// Whether to validate token lifetime. Default: true.
    /// </summary>
    public bool ValidateLifetime { get; set; } = true;
    
    /// <summary>
    /// Clock skew for token validation. Default: 0 (strict).
    /// </summary>
    public int ClockSkewSeconds { get; set; } = 0;
}
