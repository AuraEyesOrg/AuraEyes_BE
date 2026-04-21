namespace Infrastructure.Settings;

/// <summary>
/// Rate limiting configuration settings.
/// </summary>
public sealed class RateLimitingSettings
{
    public const string SectionName = "RateLimiting";

    public int? GlobalPermitLimit { get; set; }
    public int? GlobalWindowSeconds { get; set; }
    public int? GlobalQueueLimit { get; set; }

    public int? ReadPermitLimit { get; set; } = 120;
    public int? ReadWindowSeconds { get; set; } = 60;
    public int? ReadQueueLimit { get; set; } = 0;

    public int? WritePermitLimit { get; set; } = 60;
    public int? WriteWindowSeconds { get; set; } = 60;
    public int? WriteQueueLimit { get; set; } = 0;

    public int? SensitivePermitLimit { get; set; } = 8;
    public int? SensitiveWindowSeconds { get; set; } = 60;
    public int? SensitiveQueueLimit { get; set; } = 0;

    public int? AuthPermitLimit { get; set; }
    public int? AuthWindowSeconds { get; set; }
    public int? AuthQueueLimit { get; set; }

    public int ActualReadPermitLimit => ReadPermitLimit ?? GlobalPermitLimit ?? 240;
    public int ActualReadWindowSeconds => ReadWindowSeconds ?? GlobalWindowSeconds ?? 60;
    public int ActualReadQueueLimit => ReadQueueLimit ?? GlobalQueueLimit ?? 0;

    public int ActualWritePermitLimit => WritePermitLimit ?? GlobalPermitLimit ?? 80;
    public int ActualWriteWindowSeconds => WriteWindowSeconds ?? GlobalWindowSeconds ?? 60;
    public int ActualWriteQueueLimit => WriteQueueLimit ?? GlobalQueueLimit ?? 0;

    public int ActualSensitivePermitLimit => SensitivePermitLimit ?? AuthPermitLimit ?? 8;
    public int ActualSensitiveWindowSeconds => SensitiveWindowSeconds ?? AuthWindowSeconds ?? 60;
    public int ActualSensitiveQueueLimit => SensitiveQueueLimit ?? AuthQueueLimit ?? 0;
}
