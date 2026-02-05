namespace Infrastructure.Settings;

/// <summary>
/// PayOS configuration settings for payment processing.
/// Mapped from appsettings.json "PayOS" section.
/// </summary>
public sealed class PayOSSettings
{
    public const string SectionName = "PayOS";

    /// <summary>
    /// PayOS Client ID.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// PayOS API Key.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// PayOS Checksum Key for signature verification.
    /// </summary>
    public string ChecksumKey { get; set; } = string.Empty;

    /// <summary>
    /// Default return URL after successful payment.
    /// </summary>
    public string DefaultReturnUrl { get; set; } = string.Empty;

    /// <summary>
    /// Default cancel URL when payment is cancelled.
    /// </summary>
    public string DefaultCancelUrl { get; set; } = string.Empty;
}
