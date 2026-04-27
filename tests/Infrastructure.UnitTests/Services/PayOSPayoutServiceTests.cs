using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Common.Interfaces;
using Application.Common.Models;
using FluentAssertions;
using Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace Infrastructure.UnitTests.Services;

public class PayOSPayoutServiceTests
{
    private readonly Mock<HttpMessageHandler> _handlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<IOptions<PayOSSettings>> _settingsMock;
    private readonly Mock<ILogger<PayOSPayoutService>> _loggerMock;
    private readonly PayOSPayoutService _service;
    private readonly PayOSSettings _settings;

    public PayOSPayoutServiceTests()
    {
        _handlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_handlerMock.Object);
        _httpClient.BaseAddress = new Uri("https://api-merchant.payos.vn");
        
        _settings = new PayOSSettings
        {
            ClientId = "test-client-id",
            ApiKey = "test-api-key",
            ChecksumKey = "test-checksum-key"
        };
        _settingsMock = new Mock<IOptions<PayOSSettings>>();
        _settingsMock.Setup(s => s.Value).Returns(_settings);
        
        _loggerMock = new Mock<ILogger<PayOSPayoutService>>();
        
        _service = new PayOSPayoutService(_httpClient, _settingsMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreatePayoutAsync_WhenSuccessful_ShouldReturnPayoutResult()
    {
        var apiResponse = new { code = "00", desc = "Success", data = new { id = "P1", referenceId = "R1", status = "pending" } };
        SetupResponse(HttpStatusCode.OK, apiResponse);

        var result = await _service.CreatePayoutAsync("R1", 1000, "Desc", "970415", "123");

        result.Should().NotBeNull();
        result.Id.Should().Be("P1");
    }

    [Fact]
    public async Task CreatePayoutAsync_WhenApiReturnsError_ShouldThrow()
    {
        var apiResponse = new { code = "01", desc = "Invalid Signature" };
        SetupResponse(HttpStatusCode.OK, apiResponse);

        var act = async () => await _service.CreatePayoutAsync("R1", 1000, "Desc", "970415", "123");

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Invalid Signature*");
    }

    [Fact]
    public async Task CreatePayoutAsync_WhenHttpError_ShouldThrow()
    {
        SetupResponse(HttpStatusCode.BadRequest, "Bad Request");

        var act = async () => await _service.CreatePayoutAsync("R1", 1000, "Desc", "970415", "123");

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task GetPayoutAsync_WhenSuccessful_ShouldReturnPayout()
    {
        var apiResponse = new { code = "00", desc = "Success", data = new { id = "P123" } };
        SetupResponse(HttpStatusCode.OK, apiResponse);

        var result = await _service.GetPayoutAsync("P123");

        result.Id.Should().Be("P123");
    }

    [Fact]
    public async Task GetPayoutsAsync_WithFilter_ShouldBuildCorrectUrl()
    {
        var apiResponse = new { code = "00", desc = "Success", data = new { payouts = new[] { new { id = "P1" } }, pagination = new { total = 1 } } };
        SetupResponse(HttpStatusCode.OK, apiResponse);

        var filter = new PayOSPayoutFilter { Limit = 10, Offset = 0, ReferenceId = "REF" };
        var result = await _service.GetPayoutsAsync(filter);

        result.Payouts.Should().HaveCount(1);
        result.Pagination.Total.Should().Be(1);
    }

    [Fact]
    public async Task GetPayoutAccountBalanceAsync_ShouldReturnBalance()
    {
        var apiResponse = new { code = "00", data = new { balance = 5000, accountNumber = "123", accountName = "Test" } };
        SetupResponse(HttpStatusCode.OK, apiResponse);

        var result = await _service.GetPayoutAccountBalanceAsync();

        result.Balance.Should().Be(5000);
        result.AccountNumber.Should().Be("123");
    }

    [Fact]
    public async Task EstimateCreditAsync_ShouldReturnEstimatedAmount()
    {
        var apiResponse = new { code = "00", data = new { estimateCredit = 4500 } };
        SetupResponse(HttpStatusCode.OK, apiResponse);

        var result = await _service.EstimateCreditAsync("REF", new[] { "salary" }, new[] { new PayOSPayoutItem { Amount = 5000 } });

        result.Should().Be(4500);
    }

    private void SetupResponse(HttpStatusCode code, object body)
    {
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = code,
                Content = new StringContent(JsonSerializer.Serialize(body))
            });
    }
}
