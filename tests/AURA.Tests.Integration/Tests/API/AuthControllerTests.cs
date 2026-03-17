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
}
