using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace API.Middleware;

public class SwaggerBasicAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public SwaggerBasicAuthMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var username = _configuration["Swagger:Username"];
        var password = _configuration["Swagger:Password"];

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            await context.Response.WriteAsync("Swagger is not configured.");
            return;
        }

        if (!context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {
            Challenge(context.Response);
            return;
        }

        var headerValue = authorizationHeader.ToString();
        if (!headerValue.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            Challenge(context.Response);
            return;
        }

        var encodedCredentials = headerValue["Basic ".Length..].Trim();
        if (!TryParseCredentials(encodedCredentials, out var requestUsername, out var requestPassword))
        {
            Challenge(context.Response);
            return;
        }

        if (!SecureEquals(requestUsername, username) || !SecureEquals(requestPassword, password))
        {
            Challenge(context.Response);
            return;
        }

        await _next(context);
    }

    private static bool TryParseCredentials(string encodedCredentials, out string username, out string password)
    {
        username = string.Empty;
        password = string.Empty;

        try
        {
            var credentialBytes = Convert.FromBase64String(encodedCredentials);
            var credentials = Encoding.UTF8.GetString(credentialBytes);
            var separatorIndex = credentials.IndexOf(':');

            if (separatorIndex <= 0)
            {
                return false;
            }

            username = credentials[..separatorIndex];
            password = credentials[(separatorIndex + 1)..];
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static bool SecureEquals(string left, string right)
    {
        var leftBytes = Encoding.UTF8.GetBytes(left);
        var rightBytes = Encoding.UTF8.GetBytes(right);
        return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
    }

    private static void Challenge(HttpResponse response)
    {
        response.Headers.WWWAuthenticate = "Basic realm=\"Swagger\"";
        response.StatusCode = (int)HttpStatusCode.Unauthorized;
    }
}
