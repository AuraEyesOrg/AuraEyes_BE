namespace Infrastructure.Settings;

/// <summary>
/// SMTP configuration settings for email sending.
/// Mapped from appsettings.json "Smtp" section.
/// </summary>
public sealed class SmtpSettings
{
    public const string SectionName = "Smtp";

    /// <summary>
    /// SMTP server host address (e.g., smtp.gmail.com).
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// SMTP server port (typically 587 for StartTLS, 465 for SSL).
    /// </summary>
    public int Port { get; set; } = 587;

    /// <summary>
    /// SMTP authentication username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// SMTP authentication password (App Password for Gmail).
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Use SSL/TLS connection (port 465).
    /// </summary>
    public bool UseSsl { get; set; } = false;

    /// <summary>
    /// Use StartTLS (port 587). Takes precedence over UseSsl when true.
    /// </summary>
    public bool UseStartTls { get; set; } = true;

    /// <summary>
    /// Sender display name shown in email clients.
    /// </summary>
    public string FromName { get; set; } = "Hệ thống Aura – Quản lý khám sàng lọc mắt";

    /// <summary>
    /// Sender email address.
    /// </summary>
    public string FromEmail { get; set; } = string.Empty;
}
