using AURA.Tests.Integration.Fixtures;
using AURA.Tests.Integration.Helpers;

namespace AURA.Tests.Integration.Tests.API;

[Collection("Integration")]
public sealed class TwoFactorControllerTests
{
    private readonly IntegrationTestFixture _fixture;

    public TwoFactorControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetStatus_WithToken_ShouldReturnTwoFactorStatus()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var token = await AuthHelper.LoginPatientAsync(client);
        AuthHelper.SetBearerToken(client, token);

        var response = await client.GetAsync("/api/two-factor/status");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var body = await HttpClientHelper.ReadJsonDocumentAsync(response);
        body.RootElement.GetProperty("success").GetBoolean().Should().BeTrue();

        var data = body.RootElement.GetProperty("data");
        data.GetProperty("isEnabled").ValueKind.Should().BeOneOf(JsonValueKind.True, JsonValueKind.False);
        data.GetProperty("recoveryCodesRemaining").GetInt32().Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Setup_ShouldReturnAuthenticatorMaterial()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var token = await AuthHelper.LoginPatientAsync(client);
        AuthHelper.SetBearerToken(client, token);

        var response = await client.PostAsync("/api/two-factor/setup", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var body = await HttpClientHelper.ReadJsonDocumentAsync(response);
        body.RootElement.GetProperty("success").GetBoolean().Should().BeTrue();

        var data = body.RootElement.GetProperty("data");
        data.GetProperty("sharedKey").GetString().Should().NotBeNullOrWhiteSpace();
        data.GetProperty("authenticatorUri").GetString().Should().NotBeNullOrWhiteSpace();
        data.GetProperty("formattedKey").GetString().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Enable_WithInvalidCode_ShouldReturnBadRequest()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var token = await AuthHelper.LoginPatientAsync(client);
        AuthHelper.SetBearerToken(client, token);

        var setupResponse = await client.PostAsync("/api/two-factor/setup", null);
        setupResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await client.PostAsJsonAsync("/api/two-factor/enable", new
        {
            verificationCode = "000000"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Disable_AndGenerateRecoveryCodes_WithInvalidPassword_ShouldReturnBadRequest()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var token = await AuthHelper.LoginPatientAsync(client);
        AuthHelper.SetBearerToken(client, token);

        var disableResponse = await client.PostAsJsonAsync("/api/two-factor/disable", new
        {
            password = "WrongPassword@123"
        });
        disableResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var recoveryCodesResponse = await client.PostAsJsonAsync("/api/two-factor/recovery-codes", new
        {
            password = "WrongPassword@123"
        });
        recoveryCodesResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
