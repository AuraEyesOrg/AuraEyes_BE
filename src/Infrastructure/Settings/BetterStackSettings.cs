namespace Infrastructure.Settings;

public sealed class BetterStackSettings
{
    public const string SectionName = "BetterStack";

    public BetterStackDashboardSettings Dashboard { get; set; } = new();

    public BetterStackHeartbeatCollection Heartbeats { get; set; } = new();
}

public class BetterStackDashboardSettings
{
    public string? EmbedUrl { get; set; }
}

public class BetterStackHeartbeatCollection
{
    public BetterStackHeartbeatEndpoint DailyQuotaReset { get; set; } = new();
    public BetterStackHeartbeatEndpoint SlotMaintenance { get; set; } = new();
    public BetterStackHeartbeatEndpoint SessionReminderWorker { get; set; } = new();
    public BetterStackHeartbeatEndpoint ReservationExpirationWorker { get; set; } = new();
    public BetterStackHeartbeatEndpoint ConsultationStateWorker { get; set; } = new();
}

public class BetterStackHeartbeatEndpoint
{
    public string? PingUrl { get; set; }
    public string? StartUrl { get; set; }
    public string? FailUrl { get; set; }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(PingUrl) ||
        !string.IsNullOrWhiteSpace(StartUrl) ||
        !string.IsNullOrWhiteSpace(FailUrl);
}