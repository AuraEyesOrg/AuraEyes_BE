using AURA.Tests.Integration.Fixtures;
using AURA.Tests.Integration.Helpers;
using Application.Common.Models.Auth;

namespace AURA.Tests.Integration.Tests.API;

[Collection("Integration")]
public sealed class AuthControllerTests
{
    private readonly IntegrationTestFixture _fixture;

    public AuthControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task RegisterPatient_ShouldSucceed()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var payload = new RegisterPatientRequest
        {
            Email = $"integration.patient.{Guid.NewGuid():N}@aura.test",
            Password = "Patient@123$",
            ConfirmPassword = "Patient@123$",
            FullName = "Integration Test Patient"
        };

        var response = await client.PostAsJsonAsync("/api/auth/register/patient", payload);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await HttpClientHelper.ReadJsonDocumentAsync(response);
        body.RootElement.GetProperty("success").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public async Task Login_ShouldReturnJwt()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var token = await AuthHelper.LoginPatientAsync(client);

        token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ShouldFail()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var payload = new LoginRequest
        {
            Email = "patient@gmail.com",
            Password = "WrongPassword@123"
        };

        var response = await client.PostAsJsonAsync("/api/auth/login", payload);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetMe_WithoutToken_ShouldReturnUnauthorized()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var response = await client.GetAsync("/api/auth/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMe_WithToken_ShouldReturnCurrentUser()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var token = await AuthHelper.LoginPatientAsync(client);
        AuthHelper.SetBearerToken(client, token);

        var response = await client.GetAsync("/api/auth/me");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var body = await HttpClientHelper.ReadJsonDocumentAsync(response);
        body.RootElement.GetProperty("success").GetBoolean().Should().BeTrue();

        var data = body.RootElement.GetProperty("data");
        data.GetProperty("email").GetString().Should().Be("patient@gmail.com");
    }

    [Fact]
    public async Task Refresh_ShouldReturnNewTokenPair()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var login = await AuthHelper.LoginPatientWithTokensAsync(client);

        var payload = new RefreshTokenRequest
        {
            AccessToken = login.AccessToken!,
            RefreshToken = login.RefreshToken!
        };

        var response = await client.PostAsJsonAsync("/api/auth/refresh", payload);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.AccessToken.Should().NotBeNullOrWhiteSpace();
        apiResponse.Data.RefreshToken.Should().NotBeNullOrWhiteSpace();
        apiResponse.Data.RefreshToken.Should().NotBe(login.RefreshToken);
    }

    [Fact]
    public async Task Logout_ShouldRevokeRefreshToken()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var login = await AuthHelper.LoginPatientWithTokensAsync(client);
        AuthHelper.SetBearerToken(client, login.AccessToken!);

        var logoutPayload = new RefreshTokenRequest
        {
            AccessToken = login.AccessToken!,
            RefreshToken = login.RefreshToken!
        };

        var logoutResponse = await client.PostAsJsonAsync("/api/auth/logout", logoutPayload);
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var refreshResponse = await client.PostAsJsonAsync("/api/auth/refresh", logoutPayload);
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task LogoutAll_ShouldRevokeAllRefreshTokens()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var login = await AuthHelper.LoginPatientWithTokensAsync(client);
        AuthHelper.SetBearerToken(client, login.AccessToken!);

        var logoutAllResponse = await client.PostAsync("/api/auth/logout-all", null);
        logoutAllResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var refreshPayload = new RefreshTokenRequest
        {
            AccessToken = login.AccessToken!,
            RefreshToken = login.RefreshToken!
        };

        var refreshResponse = await client.PostAsJsonAsync("/api/auth/refresh", refreshPayload);
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
