using FluentAssertions;
using Infrastructure.Services;
using Infrastructure.Settings;
using Infrastructure.UnitTests.Common;
using System.Net;
using System.Text;
using Microsoft.Extensions.Options;

namespace Infrastructure.UnitTests.Services;

public class PayOSServiceTests
{
    [Theory]
    [InlineData("", "api", "chk", "ClientId")]
    [InlineData("   ", "api", "chk", "ClientId")]
    [InlineData("cid", "", "chk", "ApiKey")]
    [InlineData("cid", "   ", "chk", "ApiKey")]
    [InlineData("cid", "api", "", "ChecksumKey")]
    [InlineData("cid", "api", "   ", "ChecksumKey")]
    [InlineData("", "", "chk", "ClientId")]
    [InlineData("", "api", "", "ClientId")]
    public void Constructor_WhenRequiredSettingMissing_ShouldThrow(string clientId, string apiKey, string checksumKey, string expectedMessagePart)
    {
        var settings = new PayOSSettings
        {
            ClientId = clientId,
            ApiKey = apiKey,
            ChecksumKey = checksumKey
        };

        var act = () => CreateService(settings);

        act.Should().Throw<ArgumentNullException>()
            .WithMessage($"*{expectedMessagePart}*");
    }

    [Theory]
    [InlineData("sig-1", "payload-1")]
    [InlineData("sig-2", "payload-2")]
    [InlineData("sig-3", "{}")]
    [InlineData("sig-4", "{\"event\":\"x\"}")]
    [InlineData("sig-5", "raw")]
    [InlineData("sig-6", " ")]
    [InlineData("", "payload")]
    [InlineData("sig-8", "a=b&c=d")]
    [InlineData("sig-9", "[]")]
    [InlineData("sig-10", "null")]
    public async Task VerifyWebhookSignatureAsync_ShouldReturnTrueForCurrentImplementation(string signature, string payload)
    {
        var service = CreateService(new PayOSSettings { ClientId = "cid", ApiKey = "api", ChecksumKey = "chk" });

        var result = await service.VerifyWebhookSignatureAsync(signature, payload);

        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("bad-order")]
    [InlineData(" ")]
    [InlineData("")]
    [InlineData("not-number")]
    [InlineData("1.5")]
    [InlineData("abc123")]
    [InlineData("order-777")]
    [InlineData("x")]
    [InlineData("NaN")]
    [InlineData("++1")]
    public async Task CancelPaymentAsync_WithInvalidOrderCode_ShouldReturnFalse(string orderCode)
    {
        var service = CreateService(new PayOSSettings { ClientId = "cid", ApiKey = "api", ChecksumKey = "chk" });

        var result = await service.CancelPaymentAsync(orderCode);

        result.Should().BeFalse();
    }

    private static PayOSService CreateService(PayOSSettings settings)
        => new(
            Options.Create(settings),
            new StubHttpClientFactory(new HttpClient(new StubHandler("{}", HttpStatusCode.OK))),
            new TestLogger<PayOSService>());

    private sealed class StubHttpClientFactory(HttpClient client) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => client;
    }

    private sealed class StubHandler(string body, HttpStatusCode statusCode) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            });
    }
}
