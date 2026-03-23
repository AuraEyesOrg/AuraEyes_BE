using Application.Common.Models.Auth;

namespace AURA.Tests.Integration.Helpers;

public static class AuthHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<string> LoginPatientAsync(HttpClient client)
    {
        var auth = await LoginWithTokensAsync(client, "patient@gmail.com", "Patient@123$");
        return auth.AccessToken!;
    }

    public static async Task<string> LoginSystemAdminAsync(HttpClient client)
    {
        var auth = await LoginWithTokensAsync(client, "systemadmin@gmail.com", "SystemAdmin@123$");
        return auth.AccessToken!;
    }

    public static async Task<string> LoginAsync(HttpClient client, string email, string password)
    {
        var auth = await LoginWithTokensAsync(client, email, password);
        return auth.AccessToken!;
    }

    public static async Task<AuthResponse> LoginPatientWithTokensAsync(HttpClient client)
    {
        return await LoginWithTokensAsync(client, "patient@gmail.com", "Patient@123$");
    }

    public static async Task<AuthResponse> LoginSystemAdminWithTokensAsync(HttpClient client)
    {
        return await LoginWithTokensAsync(client, "systemadmin@gmail.com", "SystemAdmin@123$");
    }

    public static async Task<AuthResponse> LoginWithTokensAsync(HttpClient client, string email, string password)
    {
        var loginPayload = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var response = await client.PostAsJsonAsync("/api/auth/login", loginPayload);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>(JsonOptions);
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.AccessToken.Should().NotBeNullOrWhiteSpace();
        apiResponse.Data.RefreshToken.Should().NotBeNullOrWhiteSpace();

        return apiResponse.Data;
    }

    public static void SetBearerToken(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
