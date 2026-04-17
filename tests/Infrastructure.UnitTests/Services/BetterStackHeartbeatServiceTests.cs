using System.Net;
using FluentAssertions;
using Infrastructure.Services;
using Infrastructure.Settings;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Infrastructure.UnitTests.Services;

public class BetterStackHeartbeatServiceTests
{
    [Fact]
    public void GetEmbedUrl_WhenBlank_ShouldReturnNull()
    {
        var service = CreateService(new BetterStackSettings
        {
            Dashboard = new BetterStackDashboardSettings { EmbedUrl = " " }
        });

        service.GetEmbedUrl().Should().BeNull();
    }

    [Fact]
    public void GetMonitorDescriptors_ShouldReturnKnownMonitors()
    {
        var service = CreateService(new BetterStackSettings());

        var descriptors = service.GetMonitorDescriptors();

        descriptors.Should().NotBeEmpty();
        descriptors.Should().Contain(d => d.Key == "daily-quota-reset");
        descriptors.Should().Contain(d => d.Key == "slot-maintenance");
    }

    [Fact]
    public async Task NotifyStartedAsync_WhenEndpointConfigured_ShouldSendRequest()
    {
        var handler = new RecordingHandler(HttpStatusCode.OK);
        var service = CreateService(
            new BetterStackSettings
            {
                Heartbeats = new BetterStackHeartbeatCollection
                {
                    DailyQuotaReset = new BetterStackHeartbeatEndpoint { StartUrl = "https://example.com/start" }
                }
            },
            handler);

        await service.NotifyStartedAsync(BetterStackMonitor.DailyQuotaReset);

        handler.RequestCount.Should().Be(1);
        handler.LastMethod.Should().Be(HttpMethod.Post);
        handler.LastUri.Should().Be("https://example.com/start");
    }

    [Fact]
    public async Task NotifySucceededAsync_WhenEndpointMissing_ShouldNotSendRequest()
    {
        var handler = new RecordingHandler(HttpStatusCode.OK);
        var service = CreateService(new BetterStackSettings(), handler);

        await service.NotifySucceededAsync(BetterStackMonitor.DailyQuotaReset);

        handler.RequestCount.Should().Be(0);
    }

    [Fact]
    public async Task NotifyFailedAsync_WhenFailUrlMissing_ShouldFallbackToPingUrl()
    {
        var handler = new RecordingHandler(HttpStatusCode.OK);
        var service = CreateService(
            new BetterStackSettings
            {
                Heartbeats = new BetterStackHeartbeatCollection
                {
                    DailyQuotaReset = new BetterStackHeartbeatEndpoint { PingUrl = "https://example.com/ping" }
                }
            },
            handler);

        await service.NotifyFailedAsync(BetterStackMonitor.DailyQuotaReset);

        handler.RequestCount.Should().Be(1);
        handler.LastUri.Should().Be("https://example.com/ping");
    }

    [Theory]
    [InlineData("https://embed.example")]
    [InlineData("https://embed.example/status")]
    public void GetEmbedUrl_WhenProvided_ShouldReturnValue(string url)
    {
        var service = CreateService(new BetterStackSettings
        {
            Dashboard = new BetterStackDashboardSettings { EmbedUrl = url }
        });

        service.GetEmbedUrl().Should().Be(url);
    }

    [Theory]
    [InlineData(BetterStackMonitor.SessionReminderWorker, "https://example.com/reminder/start")]
    [InlineData(BetterStackMonitor.ReservationExpirationWorker, "https://example.com/reservation/start")]
    [InlineData(BetterStackMonitor.ConsultationStateWorker, "https://example.com/consultation/start")]
    public async Task NotifyStartedAsync_ForWorkerMonitors_ShouldUseConfiguredStartUrl(BetterStackMonitor monitor, string url)
    {
        var handler = new RecordingHandler(HttpStatusCode.OK);
        var settings = new BetterStackSettings
        {
            Heartbeats = new BetterStackHeartbeatCollection
            {
                SessionReminderWorker = new BetterStackHeartbeatEndpoint { StartUrl = monitor == BetterStackMonitor.SessionReminderWorker ? url : null },
                ReservationExpirationWorker = new BetterStackHeartbeatEndpoint { StartUrl = monitor == BetterStackMonitor.ReservationExpirationWorker ? url : null },
                ConsultationStateWorker = new BetterStackHeartbeatEndpoint { StartUrl = monitor == BetterStackMonitor.ConsultationStateWorker ? url : null }
            }
        };
        var service = CreateService(settings, handler);

        await service.NotifyStartedAsync(monitor);

        handler.RequestCount.Should().Be(1);
        handler.LastUri.Should().Be(url);
    }

    [Fact]
    public async Task NotifyStartedAsync_WhenStartMissing_ShouldFallbackToPing()
    {
        var handler = new RecordingHandler(HttpStatusCode.OK);
        var service = CreateService(
            new BetterStackSettings
            {
                Heartbeats = new BetterStackHeartbeatCollection
                {
                    SlotMaintenance = new BetterStackHeartbeatEndpoint { PingUrl = "https://example.com/slot/ping" }
                }
            },
            handler);

        await service.NotifyStartedAsync(BetterStackMonitor.SlotMaintenance);

        handler.RequestCount.Should().Be(1);
        handler.LastUri.Should().Be("https://example.com/slot/ping");
    }

    [Fact]
    public async Task NotifyFailedAsync_WhenNoUrlsConfigured_ShouldSkipRequest()
    {
        var handler = new RecordingHandler(HttpStatusCode.OK);
        var service = CreateService(new BetterStackSettings(), handler);

        await service.NotifyFailedAsync(BetterStackMonitor.ConsultationStateWorker);

        handler.RequestCount.Should().Be(0);
    }

    [Fact]
    public void GetMonitorDescriptors_ShouldIncludeAllEnumMonitors()
    {
        var service = CreateService(new BetterStackSettings());

        var descriptors = service.GetMonitorDescriptors();

        descriptors.Should().HaveCount(Enum.GetValues<BetterStackMonitor>().Length);
    }

    [Fact]
    public async Task NotifySucceededAsync_WhenPingConfigured_ShouldSendPingRequest()
    {
        var handler = new RecordingHandler(HttpStatusCode.OK);
        var service = CreateService(
            new BetterStackSettings
            {
                Heartbeats = new BetterStackHeartbeatCollection
                {
                    SessionReminderWorker = new BetterStackHeartbeatEndpoint { PingUrl = "https://example.com/reminder/ping" }
                }
            },
            handler);

        await service.NotifySucceededAsync(BetterStackMonitor.SessionReminderWorker);

        handler.RequestCount.Should().Be(1);
        handler.LastMethod.Should().Be(HttpMethod.Post);
        handler.LastUri.Should().Be("https://example.com/reminder/ping");
    }

    [Theory]
    [InlineData(BetterStackMonitor.DailyQuotaReset, "daily-quota-reset")]
    [InlineData(BetterStackMonitor.SlotMaintenance, "slot-maintenance")]
    [InlineData(BetterStackMonitor.SessionReminderWorker, "session-reminder-worker")]
    [InlineData(BetterStackMonitor.ReservationExpirationWorker, "reservation-expiration-worker")]
    [InlineData(BetterStackMonitor.ConsultationStateWorker, "consultation-state-worker")]
    public void GetMonitorDescriptors_ShouldMapMonitorToExpectedKey(BetterStackMonitor monitor, string expectedKey)
    {
        var service = CreateService(new BetterStackSettings());
        var descriptors = service.GetMonitorDescriptors();

        descriptors.Should().ContainSingle(d => d.Monitor == monitor && d.Key == expectedKey);
    }

    [Theory]
    [InlineData(BetterStackMonitor.DailyQuotaReset)]
    [InlineData(BetterStackMonitor.SlotMaintenance)]
    [InlineData(BetterStackMonitor.SessionReminderWorker)]
    [InlineData(BetterStackMonitor.ReservationExpirationWorker)]
    [InlineData(BetterStackMonitor.ConsultationStateWorker)]
    public async Task NotifyFailedAsync_WhenFailConfigured_ShouldUseFailUrl(BetterStackMonitor monitor)
    {
        var handler = new RecordingHandler(HttpStatusCode.OK);
        var failUrl = $"https://example.com/{monitor}/fail";
        var settings = new BetterStackSettings
        {
            Heartbeats = new BetterStackHeartbeatCollection
            {
                DailyQuotaReset = monitor == BetterStackMonitor.DailyQuotaReset ? new BetterStackHeartbeatEndpoint { FailUrl = failUrl } : new BetterStackHeartbeatEndpoint(),
                SlotMaintenance = monitor == BetterStackMonitor.SlotMaintenance ? new BetterStackHeartbeatEndpoint { FailUrl = failUrl } : new BetterStackHeartbeatEndpoint(),
                SessionReminderWorker = monitor == BetterStackMonitor.SessionReminderWorker ? new BetterStackHeartbeatEndpoint { FailUrl = failUrl } : new BetterStackHeartbeatEndpoint(),
                ReservationExpirationWorker = monitor == BetterStackMonitor.ReservationExpirationWorker ? new BetterStackHeartbeatEndpoint { FailUrl = failUrl } : new BetterStackHeartbeatEndpoint(),
                ConsultationStateWorker = monitor == BetterStackMonitor.ConsultationStateWorker ? new BetterStackHeartbeatEndpoint { FailUrl = failUrl } : new BetterStackHeartbeatEndpoint()
            }
        };
        var service = CreateService(settings, handler);

        await service.NotifyFailedAsync(monitor);

        handler.RequestCount.Should().Be(1);
        handler.LastUri.Should().Be(failUrl);
    }

    [Theory]
    [InlineData(BetterStackMonitor.DailyQuotaReset)]
    [InlineData(BetterStackMonitor.SlotMaintenance)]
    [InlineData(BetterStackMonitor.SessionReminderWorker)]
    [InlineData(BetterStackMonitor.ReservationExpirationWorker)]
    [InlineData(BetterStackMonitor.ConsultationStateWorker)]
    public async Task NotifyStartedAsync_WhenNoUrlsConfigured_ShouldSkipRequest(BetterStackMonitor monitor)
    {
        var handler = new RecordingHandler(HttpStatusCode.OK);
        var service = CreateService(new BetterStackSettings(), handler);

        await service.NotifyStartedAsync(monitor);

        handler.RequestCount.Should().Be(0);
    }

    [Theory]
    [InlineData(BetterStackMonitor.DailyQuotaReset)]
    [InlineData(BetterStackMonitor.SlotMaintenance)]
    [InlineData(BetterStackMonitor.SessionReminderWorker)]
    [InlineData(BetterStackMonitor.ReservationExpirationWorker)]
    [InlineData(BetterStackMonitor.ConsultationStateWorker)]
    public async Task NotifySucceededAsync_WhenNoUrlsConfigured_ShouldSkipRequest(BetterStackMonitor monitor)
    {
        var handler = new RecordingHandler(HttpStatusCode.OK);
        var service = CreateService(new BetterStackSettings(), handler);

        await service.NotifySucceededAsync(monitor);

        handler.RequestCount.Should().Be(0);
    }

    [Theory]
    [InlineData(BetterStackMonitor.DailyQuotaReset)]
    [InlineData(BetterStackMonitor.SlotMaintenance)]
    [InlineData(BetterStackMonitor.SessionReminderWorker)]
    [InlineData(BetterStackMonitor.ReservationExpirationWorker)]
    [InlineData(BetterStackMonitor.ConsultationStateWorker)]
    public async Task NotifyStartedAsync_WhenOnlyPingConfigured_ShouldFallbackToPingForAnyMonitor(BetterStackMonitor monitor)
    {
        var handler = new RecordingHandler(HttpStatusCode.OK);
        var pingUrl = $"https://example.com/{monitor}/ping";
        var settings = new BetterStackSettings
        {
            Heartbeats = new BetterStackHeartbeatCollection
            {
                DailyQuotaReset = monitor == BetterStackMonitor.DailyQuotaReset ? new BetterStackHeartbeatEndpoint { PingUrl = pingUrl } : new BetterStackHeartbeatEndpoint(),
                SlotMaintenance = monitor == BetterStackMonitor.SlotMaintenance ? new BetterStackHeartbeatEndpoint { PingUrl = pingUrl } : new BetterStackHeartbeatEndpoint(),
                SessionReminderWorker = monitor == BetterStackMonitor.SessionReminderWorker ? new BetterStackHeartbeatEndpoint { PingUrl = pingUrl } : new BetterStackHeartbeatEndpoint(),
                ReservationExpirationWorker = monitor == BetterStackMonitor.ReservationExpirationWorker ? new BetterStackHeartbeatEndpoint { PingUrl = pingUrl } : new BetterStackHeartbeatEndpoint(),
                ConsultationStateWorker = monitor == BetterStackMonitor.ConsultationStateWorker ? new BetterStackHeartbeatEndpoint { PingUrl = pingUrl } : new BetterStackHeartbeatEndpoint()
            }
        };
        var service = CreateService(settings, handler);

        await service.NotifyStartedAsync(monitor);

        handler.RequestCount.Should().Be(1);
        handler.LastUri.Should().Be(pingUrl);
    }

    [Theory]
    [InlineData(BetterStackMonitor.DailyQuotaReset)]
    [InlineData(BetterStackMonitor.SlotMaintenance)]
    [InlineData(BetterStackMonitor.SessionReminderWorker)]
    [InlineData(BetterStackMonitor.ReservationExpirationWorker)]
    [InlineData(BetterStackMonitor.ConsultationStateWorker)]
    public async Task NotifySucceededAsync_WhenPingConfigured_ShouldUsePostForEachMonitor(BetterStackMonitor monitor)
    {
        var handler = new RecordingHandler(HttpStatusCode.OK);
        var pingUrl = $"https://example.com/{monitor}/success";
        var settings = new BetterStackSettings
        {
            Heartbeats = new BetterStackHeartbeatCollection
            {
                DailyQuotaReset = monitor == BetterStackMonitor.DailyQuotaReset ? new BetterStackHeartbeatEndpoint { PingUrl = pingUrl } : new BetterStackHeartbeatEndpoint(),
                SlotMaintenance = monitor == BetterStackMonitor.SlotMaintenance ? new BetterStackHeartbeatEndpoint { PingUrl = pingUrl } : new BetterStackHeartbeatEndpoint(),
                SessionReminderWorker = monitor == BetterStackMonitor.SessionReminderWorker ? new BetterStackHeartbeatEndpoint { PingUrl = pingUrl } : new BetterStackHeartbeatEndpoint(),
                ReservationExpirationWorker = monitor == BetterStackMonitor.ReservationExpirationWorker ? new BetterStackHeartbeatEndpoint { PingUrl = pingUrl } : new BetterStackHeartbeatEndpoint(),
                ConsultationStateWorker = monitor == BetterStackMonitor.ConsultationStateWorker ? new BetterStackHeartbeatEndpoint { PingUrl = pingUrl } : new BetterStackHeartbeatEndpoint()
            }
        };
        var service = CreateService(settings, handler);

        await service.NotifySucceededAsync(monitor);

        handler.RequestCount.Should().Be(1);
        handler.LastMethod.Should().Be(HttpMethod.Post);
        handler.LastUri.Should().Be(pingUrl);
    }

    [Theory]
    [InlineData(BetterStackMonitor.DailyQuotaReset)]
    [InlineData(BetterStackMonitor.SlotMaintenance)]
    [InlineData(BetterStackMonitor.SessionReminderWorker)]
    [InlineData(BetterStackMonitor.ReservationExpirationWorker)]
    [InlineData(BetterStackMonitor.ConsultationStateWorker)]
    public async Task NotifyFailedAsync_WhenOnlyPingConfigured_ShouldFallbackToPingForEachMonitor(BetterStackMonitor monitor)
    {
        var handler = new RecordingHandler(HttpStatusCode.OK);
        var pingUrl = $"https://example.com/{monitor}/failed";
        var settings = new BetterStackSettings
        {
            Heartbeats = new BetterStackHeartbeatCollection
            {
                DailyQuotaReset = monitor == BetterStackMonitor.DailyQuotaReset ? new BetterStackHeartbeatEndpoint { PingUrl = pingUrl } : new BetterStackHeartbeatEndpoint(),
                SlotMaintenance = monitor == BetterStackMonitor.SlotMaintenance ? new BetterStackHeartbeatEndpoint { PingUrl = pingUrl } : new BetterStackHeartbeatEndpoint(),
                SessionReminderWorker = monitor == BetterStackMonitor.SessionReminderWorker ? new BetterStackHeartbeatEndpoint { PingUrl = pingUrl } : new BetterStackHeartbeatEndpoint(),
                ReservationExpirationWorker = monitor == BetterStackMonitor.ReservationExpirationWorker ? new BetterStackHeartbeatEndpoint { PingUrl = pingUrl } : new BetterStackHeartbeatEndpoint(),
                ConsultationStateWorker = monitor == BetterStackMonitor.ConsultationStateWorker ? new BetterStackHeartbeatEndpoint { PingUrl = pingUrl } : new BetterStackHeartbeatEndpoint()
            }
        };
        var service = CreateService(settings, handler);

        await service.NotifyFailedAsync(monitor);

        handler.RequestCount.Should().Be(1);
        handler.LastMethod.Should().Be(HttpMethod.Post);
        handler.LastUri.Should().Be(pingUrl);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\r\n")]
    [InlineData(" \t ")]
    [InlineData("   \r\n")]
    [InlineData("\n")]
    [InlineData("\r")]
    public void GetEmbedUrl_WhenWhitespaceOrNull_ShouldReturnNull(string? embedUrl)
    {
        var service = CreateService(new BetterStackSettings
        {
            Dashboard = new BetterStackDashboardSettings { EmbedUrl = embedUrl }
        });

        service.GetEmbedUrl().Should().BeNull();
    }

    [Theory]
    [InlineData(BetterStackMonitor.DailyQuotaReset, 1)]
    [InlineData(BetterStackMonitor.SlotMaintenance, 2)]
    [InlineData(BetterStackMonitor.SessionReminderWorker, 3)]
    [InlineData(BetterStackMonitor.ReservationExpirationWorker, 4)]
    [InlineData(BetterStackMonitor.ConsultationStateWorker, 5)]
    [InlineData(BetterStackMonitor.DailyQuotaReset, 6)]
    [InlineData(BetterStackMonitor.SlotMaintenance, 7)]
    [InlineData(BetterStackMonitor.SessionReminderWorker, 8)]
    [InlineData(BetterStackMonitor.ReservationExpirationWorker, 9)]
    [InlineData(BetterStackMonitor.ConsultationStateWorker, 10)]
    public async Task NotifyStartedAsync_WhenBothStartAndPingConfigured_ShouldPreferStartUrl(BetterStackMonitor monitor, int caseId)
    {
        var handler = new RecordingHandler(HttpStatusCode.OK);
        var startUrl = $"https://example.com/{monitor}/{caseId}/start-preferred";
        var pingUrl = $"https://example.com/{monitor}/{caseId}/ping-fallback";
        var settings = new BetterStackSettings
        {
            Heartbeats = new BetterStackHeartbeatCollection
            {
                DailyQuotaReset = monitor == BetterStackMonitor.DailyQuotaReset ? new BetterStackHeartbeatEndpoint { StartUrl = startUrl, PingUrl = pingUrl } : new BetterStackHeartbeatEndpoint(),
                SlotMaintenance = monitor == BetterStackMonitor.SlotMaintenance ? new BetterStackHeartbeatEndpoint { StartUrl = startUrl, PingUrl = pingUrl } : new BetterStackHeartbeatEndpoint(),
                SessionReminderWorker = monitor == BetterStackMonitor.SessionReminderWorker ? new BetterStackHeartbeatEndpoint { StartUrl = startUrl, PingUrl = pingUrl } : new BetterStackHeartbeatEndpoint(),
                ReservationExpirationWorker = monitor == BetterStackMonitor.ReservationExpirationWorker ? new BetterStackHeartbeatEndpoint { StartUrl = startUrl, PingUrl = pingUrl } : new BetterStackHeartbeatEndpoint(),
                ConsultationStateWorker = monitor == BetterStackMonitor.ConsultationStateWorker ? new BetterStackHeartbeatEndpoint { StartUrl = startUrl, PingUrl = pingUrl } : new BetterStackHeartbeatEndpoint()
            }
        };
        var service = CreateService(settings, handler);

        await service.NotifyStartedAsync(monitor);

        handler.RequestCount.Should().Be(1);
        handler.LastUri.Should().Be(startUrl);
    }

    private static BetterStackHeartbeatService CreateService(
        BetterStackSettings settings,
        HttpMessageHandler? handler = null)
    {
        var clientFactory = new FakeHttpClientFactory(new HttpClient(handler ?? new RecordingHandler(HttpStatusCode.OK)));
        return new BetterStackHeartbeatService(
            clientFactory,
            Options.Create(settings),
            NullLogger<BetterStackHeartbeatService>.Instance);
    }

    private sealed class FakeHttpClientFactory(HttpClient httpClient) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => httpClient;
    }

    private sealed class RecordingHandler(HttpStatusCode statusCode) : HttpMessageHandler
    {
        public int RequestCount { get; private set; }
        public string? LastUri { get; private set; }
        public HttpMethod? LastMethod { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestCount++;
            LastUri = request.RequestUri?.ToString();
            LastMethod = request.Method;
            return Task.FromResult(new HttpResponseMessage(statusCode));
        }
    }
}
