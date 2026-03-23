using System.Net.Http.Json;
using System.Text.Json;
using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace AURA.Tests.Integration.Fixtures;

public sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly DatabaseFixture _databaseFixture;

    public ApiFactory(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
    }

    public WireMockServer AiMockServer { get; private set; } = null!;
    public WireMockServer PaymentMockServer { get; private set; } = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            var overrides = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _databaseFixture.ConnectionString,
                ["JwtSettings:SecretKey"] = "DevSecretKey_MustBeAtLeast32Characters_ForTesting123!",
                ["JwtSettings:Issuer"] = "AuraEyesAPI",
                ["JwtSettings:Audience"] = "AuraEyesClient",
                ["PayOS:ClientId"] = "integration-client",
                ["PayOS:ApiKey"] = "integration-api-key",
                ["PayOS:ChecksumKey"] = "integration-checksum-key",
                ["PayOS:DefaultReturnUrl"] = "https://localhost/payment/success",
                ["PayOS:DefaultCancelUrl"] = "https://localhost/payment/cancel"
            };

            configBuilder.AddInMemoryCollection(overrides);
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<ApplicationDbContext>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(_databaseFixture.ConnectionString);
            });

            var hostedServices = services
                .Where(d => d.ServiceType == typeof(IHostedService)
                            && d.ImplementationType is not null
                            && (d.ImplementationType.Name.Contains("SessionReminderWorker", StringComparison.OrdinalIgnoreCase)
                                || d.ImplementationType.Name.Contains("ReservationExpirationWorker", StringComparison.OrdinalIgnoreCase)
                                || d.ImplementationType.Name.Contains("Hangfire", StringComparison.OrdinalIgnoreCase)))
                .ToList();

            foreach (var descriptor in hostedServices)
            {
                services.Remove(descriptor);
            }

            services.RemoveAll<IPayOSService>();
            services.AddSingleton<IPayOSService>(_ => new WireMockPayOSService(PaymentMockServer.Urls[0]));
        });
    }

    public async Task InitializeAsync()
    {
        AiMockServer = WireMockServer.Start();
        PaymentMockServer = WireMockServer.Start();

        ConfigureAiStubs();
        ConfigurePaymentStubs();

        _ = CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();

        await _databaseFixture.InitializeRespawnerAsync();
    }

    public new async Task DisposeAsync()
    {
        AiMockServer.Stop();
        PaymentMockServer.Stop();
        AiMockServer.Dispose();
        PaymentMockServer.Dispose();

        await base.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await _databaseFixture.ResetAsync();

        // Keep seeded users but always reset 2FA flags between tests.
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.ExecuteSqlRawAsync("UPDATE \"AspNetUsers\" SET \"TwoFactorEnabled\" = FALSE");

        AiMockServer.ResetLogEntries();
        PaymentMockServer.ResetLogEntries();
    }

    public HttpClient CreateClientWithHttps()
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }

    private void ConfigureAiStubs()
    {
        AiMockServer
            .Given(Request.Create().WithPath("/mock/ai/screenings").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(202)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"requestId\":\"ai-queued-001\",\"status\":\"queued\"}"));
    }

    private void ConfigurePaymentStubs()
    {
        PaymentMockServer
            .Given(Request.Create().WithPath("/mock/payos/payment-links").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"paymentUrl\":\"https://pay.mock/checkout/ORDER-INT-001\",\"orderCode\":\"ORDER-INT-001\"}"));

        PaymentMockServer
            .Given(Request.Create().WithPath("/mock/payos/payment-status/*").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"status\":\"PAID\",\"amount\":100000,\"txnRef\":\"TXN-INT-001\"}"));
    }

    private sealed class WireMockPayOSService : IPayOSService
    {
        private readonly HttpClient _client;

        public WireMockPayOSService(string baseUrl)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
        }

        public async Task<(string PaymentUrl, string OrderCode)> CreatePaymentLinkAsync(
            Guid depositRequestId,
            decimal amountVnd,
            string description,
            string returnUrl,
            string cancelUrl)
        {
            var response = await _client.PostAsJsonAsync("/mock/payos/payment-links", new
            {
                depositRequestId,
                amountVnd,
                description,
                returnUrl,
                cancelUrl
            });

            response.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var paymentUrl = doc.RootElement.GetProperty("paymentUrl").GetString() ?? string.Empty;
            var orderCode = doc.RootElement.GetProperty("orderCode").GetString() ?? string.Empty;

            return (paymentUrl, orderCode);
        }

        public async Task<(string Status, decimal Amount, string TxnRef)> GetPaymentStatusAsync(string orderCode)
        {
            var response = await _client.GetAsync($"/mock/payos/payment-status/{orderCode}");
            response.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var status = doc.RootElement.GetProperty("status").GetString() ?? "PENDING";
            var amount = doc.RootElement.GetProperty("amount").GetDecimal();
            var txnRef = doc.RootElement.GetProperty("txnRef").GetString() ?? string.Empty;

            return (status, amount, txnRef);
        }

        public Task<bool> VerifyWebhookSignatureAsync(string signature, string payload)
        {
            return Task.FromResult(true);
        }

        public Task<bool> CancelPaymentAsync(string orderCode)
        {
            return Task.FromResult(true);
        }
    }
}
