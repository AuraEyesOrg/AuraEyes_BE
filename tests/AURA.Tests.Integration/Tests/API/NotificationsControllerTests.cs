using AURA.Tests.Integration.Fixtures;
using AURA.Tests.Integration.Helpers;

namespace AURA.Tests.Integration.Tests.API;

[Collection("Integration")]
public sealed class NotificationsControllerTests
{
    private readonly IntegrationTestFixture _fixture;

    public NotificationsControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetMyNotifications_AfterWalletDeposit_ShouldContainNotification()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var login = await AuthHelper.LoginPatientWithTokensAsync(client);
        AuthHelper.SetBearerToken(client, login.AccessToken!);

        var createDepositResponse = await client.PostAsJsonAsync("/api/wallets/deposit", new
        {
            amountVnd = 120000m,
            paymentMethod = 0,
            description = "Integration notifications",
            returnUrl = "https://localhost/success",
            cancelUrl = "https://localhost/cancel"
        });
        createDepositResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var createJson = await HttpClientHelper.ReadJsonDocumentAsync(createDepositResponse);
        var orderCode = createJson.RootElement.GetProperty("data").GetProperty("orderCode").GetString();
        orderCode.Should().NotBeNullOrWhiteSpace();

        var statusResponse = await client.GetAsync($"/api/wallets/payment-status/{orderCode}");
        statusResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var notificationsResponse = await client.GetAsync("/api/notifications?pageNumber=1&pageSize=10");
        notificationsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var notificationsJson = await HttpClientHelper.ReadJsonDocumentAsync(notificationsResponse);
        notificationsJson.RootElement.GetProperty("success").GetBoolean().Should().BeTrue();

        var data = notificationsJson.RootElement.GetProperty("data");
        var items = data.GetProperty("items");
        items.ValueKind.Should().Be(JsonValueKind.Array);
        items.GetArrayLength().Should().BeGreaterThan(0);

        var unreadCount = data.GetProperty("unreadCount").GetInt32();
        unreadCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task MarkAsRead_AndMarkAllAsRead_ShouldUpdateUnreadCount()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var login = await AuthHelper.LoginPatientWithTokensAsync(client);
        AuthHelper.SetBearerToken(client, login.AccessToken!);

        var createDepositResponse = await client.PostAsJsonAsync("/api/wallets/deposit", new
        {
            amountVnd = 130000m,
            paymentMethod = 0,
            description = "Integration mark read",
            returnUrl = "https://localhost/success",
            cancelUrl = "https://localhost/cancel"
        });
        createDepositResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var createJson = await HttpClientHelper.ReadJsonDocumentAsync(createDepositResponse);
        var orderCode = createJson.RootElement.GetProperty("data").GetProperty("orderCode").GetString();
        orderCode.Should().NotBeNullOrWhiteSpace();

        var statusResponse = await client.GetAsync($"/api/wallets/payment-status/{orderCode}");
        statusResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var notificationsResponse = await client.GetAsync("/api/notifications?pageNumber=1&pageSize=10");
        notificationsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var notificationsJson = await HttpClientHelper.ReadJsonDocumentAsync(notificationsResponse);
        var firstNotificationId = notificationsJson.RootElement
            .GetProperty("data")
            .GetProperty("items")[0]
            .GetProperty("id")
            .GetGuid();

        var markOneResponse = await client.PostAsync($"/api/notifications/{firstNotificationId}/mark-read", null);
        markOneResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var markAllResponse = await client.PostAsync("/api/notifications/mark-all-read", null);
        markAllResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var unreadCountResponse = await client.GetAsync("/api/notifications/unread-count");
        unreadCountResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var unreadJson = await HttpClientHelper.ReadJsonDocumentAsync(unreadCountResponse);
        unreadJson.RootElement.GetProperty("unreadCount").GetInt32().Should().Be(0);
    }
}
