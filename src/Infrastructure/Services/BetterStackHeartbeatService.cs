using Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public enum BetterStackMonitor
{
    DailyQuotaReset,
    SlotMaintenance,
    SessionReminderWorker,
    ReservationExpirationWorker,
    ConsultationStateWorker
}

public record BetterStackMonitorDescriptor(
    BetterStackMonitor Monitor,
    string Key,
    string DisplayName,
    string Category,
    bool Configured);

public interface IBetterStackHeartbeatService
{
    Task NotifyStartedAsync(BetterStackMonitor monitor, CancellationToken cancellationToken = default);
    Task NotifySucceededAsync(BetterStackMonitor monitor, CancellationToken cancellationToken = default);
    Task NotifyFailedAsync(BetterStackMonitor monitor, CancellationToken cancellationToken = default);
    IReadOnlyList<BetterStackMonitorDescriptor> GetMonitorDescriptors();
    string? GetEmbedUrl();
}

public class BetterStackHeartbeatService : IBetterStackHeartbeatService
{
    private static readonly IReadOnlyDictionary<BetterStackMonitor, (string Key, string Name, string Category)> MonitorMeta
        = new Dictionary<BetterStackMonitor, (string Key, string Name, string Category)>
        {
            [BetterStackMonitor.DailyQuotaReset] = ("daily-quota-reset", "Daily quota reset", "hangfire"),
            [BetterStackMonitor.SlotMaintenance] = ("slot-maintenance", "Slot maintenance", "hangfire"),
            [BetterStackMonitor.SessionReminderWorker] = ("session-reminder-worker", "Session reminder worker", "worker"),
            [BetterStackMonitor.ReservationExpirationWorker] = ("reservation-expiration-worker", "Reservation expiration worker", "worker"),
            [BetterStackMonitor.ConsultationStateWorker] = ("consultation-state-worker", "Consultation state worker", "worker")
        };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<BetterStackHeartbeatService> _logger;
    private readonly BetterStackSettings _settings;

    public BetterStackHeartbeatService(
        IHttpClientFactory httpClientFactory,
        IOptions<BetterStackSettings> settings,
        ILogger<BetterStackHeartbeatService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _settings = settings.Value ?? new BetterStackSettings();
    }

    public string? GetEmbedUrl() => string.IsNullOrWhiteSpace(_settings.Dashboard.EmbedUrl)
        ? null
        : _settings.Dashboard.EmbedUrl;

    public IReadOnlyList<BetterStackMonitorDescriptor> GetMonitorDescriptors()
    {
        return Enum.GetValues<BetterStackMonitor>()
            .Select(monitor =>
            {
                var meta = MonitorMeta[monitor];
                return new BetterStackMonitorDescriptor(
                    monitor,
                    meta.Key,
                    meta.Name,
                    meta.Category,
                    ResolveEndpoint(monitor).IsConfigured);
            })
            .ToList();
    }

    public Task NotifyStartedAsync(BetterStackMonitor monitor, CancellationToken cancellationToken = default) =>
        NotifyAsync(monitor, HeartbeatEvent.Start, cancellationToken);

    public Task NotifySucceededAsync(BetterStackMonitor monitor, CancellationToken cancellationToken = default) =>
        NotifyAsync(monitor, HeartbeatEvent.Success, cancellationToken);

    public Task NotifyFailedAsync(BetterStackMonitor monitor, CancellationToken cancellationToken = default) =>
        NotifyAsync(monitor, HeartbeatEvent.Fail, cancellationToken);

    private async Task NotifyAsync(
        BetterStackMonitor monitor,
        HeartbeatEvent eventType,
        CancellationToken cancellationToken)
    {
        var endpoint = ResolveEndpoint(monitor);
        var (key, _, _) = MonitorMeta[monitor];
        var targetUrl = ResolveUrl(endpoint, eventType);

        if (string.IsNullOrWhiteSpace(targetUrl))
        {
            return;
        }

        try
        {
            var client = _httpClientFactory.CreateClient(nameof(BetterStackHeartbeatService));
            using var request = new HttpRequestMessage(HttpMethod.Post, targetUrl);
            using var response = await client.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "BetterStack heartbeat returned non-success status {StatusCode} for {MonitorKey} ({EventType})",
                    (int)response.StatusCode,
                    key,
                    eventType);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Failed to send BetterStack heartbeat for {MonitorKey} ({EventType})",
                key,
                eventType);
        }
    }

    private BetterStackHeartbeatEndpoint ResolveEndpoint(BetterStackMonitor monitor)
    {
        return monitor switch
        {
            BetterStackMonitor.DailyQuotaReset => _settings.Heartbeats.DailyQuotaReset,
            BetterStackMonitor.SlotMaintenance => _settings.Heartbeats.SlotMaintenance,
            BetterStackMonitor.SessionReminderWorker => _settings.Heartbeats.SessionReminderWorker,
            BetterStackMonitor.ReservationExpirationWorker => _settings.Heartbeats.ReservationExpirationWorker,
            BetterStackMonitor.ConsultationStateWorker => _settings.Heartbeats.ConsultationStateWorker,
            _ => new BetterStackHeartbeatEndpoint()
        };
    }

    private static string? ResolveUrl(BetterStackHeartbeatEndpoint endpoint, HeartbeatEvent eventType)
    {
        return eventType switch
        {
            HeartbeatEvent.Start => endpoint.StartUrl ?? endpoint.PingUrl,
            HeartbeatEvent.Success => endpoint.PingUrl,
            HeartbeatEvent.Fail => endpoint.FailUrl ?? endpoint.PingUrl,
            _ => endpoint.PingUrl
        };
    }

    private enum HeartbeatEvent
    {
        Start,
        Success,
        Fail
    }
}