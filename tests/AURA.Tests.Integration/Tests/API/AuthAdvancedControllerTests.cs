using AURA.Tests.Integration.Fixtures;
using AURA.Tests.Integration.Helpers;

namespace AURA.Tests.Integration.Tests.API;

[Collection("Integration")]
public sealed class AuthAdvancedControllerTests
{
    private readonly IntegrationTestFixture _fixture;

    public AuthAdvancedControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task RegisterOphthalmologist_WithMultipartForm_ShouldSucceed()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        using var form = new MultipartFormDataContent
        {
            { new StringContent($"integration.ophth.{Guid.NewGuid():N}@aura.test"), "Email" },
            { new StringContent("Ophthalmologist@123$"), "Password" },
            { new StringContent("Ophthalmologist@123$"), "ConfirmPassword" },
            { new StringContent("Integration Ophthalmologist"), "FullName" },
            { new StringContent("+84987654321"), "Phone" },
            { new StringContent("Integration bio"), "Bio" },
            { new StringContent("8"), "YearsOfExperience" }
        };

        var response = await client.PostAsync("/api/auth/register/ophthalmologist", form);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var body = await HttpClientHelper.ReadJsonDocumentAsync(response);
        body.RootElement.GetProperty("success").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public async Task ForgotPassword_AndResendConfirmation_ShouldAlwaysReturnOk()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var forgotPassword = await client.PostAsJsonAsync("/api/auth/forgot-password", new
        {
            email = "notfound@example.com"
        });
        forgotPassword.StatusCode.Should().Be(HttpStatusCode.OK);

        var resendConfirmation = await client.PostAsJsonAsync("/api/auth/resend-confirmation", new
        {
            email = "notfound@example.com"
        });
        resendConfirmation.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ConfirmEmail_AndResetPassword_WithInvalidToken_ShouldFail()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var confirmResponse = await client.GetAsync($"/api/auth/confirm-email?userId={Guid.NewGuid():N}&token=invalid-token");
        confirmResponse.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);

        var resetResponse = await client.PostAsJsonAsync("/api/auth/reset-password", new
        {
            email = "patient@gmail.com",
            token = "invalid-token",
            newPassword = "Patient@123$",
            confirmPassword = "Patient@123$"
        });

        resetResponse.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GoogleLogin_WithInvalidCredential_ShouldFail()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/google-login", new
        {
            credential = "invalid-google-id-token"
        });

        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized);
    }
}
