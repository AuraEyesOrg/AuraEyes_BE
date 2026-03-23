using AURA.Tests.Integration.Fixtures;
using AURA.Tests.Integration.Helpers;
using Application.Common.Models.Auth;

namespace AURA.Tests.Integration.Tests.API;

[Collection("Integration")]
public sealed class WalletControllerTests
{
    private readonly IntegrationTestFixture _fixture;

    public WalletControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetPaymentStatus_ShouldReturnStatusForExistingOrderCode()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var login = await AuthHelper.LoginPatientWithTokensAsync(client);
        AuthHelper.SetBearerToken(client, login.AccessToken!);

        var createDepositResponse = await client.PostAsJsonAsync("/api/wallets/deposit", new
        {
            amountVnd = 100000m,
            paymentMethod = 0,
            description = "Integration payment status",
            returnUrl = "https://localhost/success",
            cancelUrl = "https://localhost/cancel"
        });

        createDepositResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var createJson = await HttpClientHelper.ReadJsonDocumentAsync(createDepositResponse);
        var orderCode = createJson.RootElement
            .GetProperty("data")
            .GetProperty("orderCode")
            .GetString();

        orderCode.Should().NotBeNullOrWhiteSpace();

        client.DefaultRequestHeaders.Authorization = null;
        var statusResponse = await client.GetAsync($"/api/wallets/payment-status/{orderCode}");

        statusResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var statusJson = await HttpClientHelper.ReadJsonDocumentAsync(statusResponse);
        statusJson.RootElement.GetProperty("success").GetBoolean().Should().BeTrue();

        var data = statusJson.RootElement.GetProperty("data");
        data.GetProperty("orderCode").GetString().Should().Be(orderCode);
        data.GetProperty("status").GetString().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task PayOSWebhook_WithFailurePayload_ShouldReturnAcknowledged()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var response = await client.PostAsJsonAsync("/api/wallets/webhook/payos", new
        {
            code = "00",
            desc = "mock-failure",
            success = false,
            data = (object?)null,
            signature = "integration-signature"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var body = await HttpClientHelper.ReadJsonDocumentAsync(response);
        body.RootElement.GetProperty("success").GetBoolean().Should().BeTrue();
    }
}
