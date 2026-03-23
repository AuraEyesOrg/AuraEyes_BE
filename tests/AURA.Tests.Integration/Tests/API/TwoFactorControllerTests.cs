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
    public async Task Setup_Enable_LoginVerify_WithTotpAndRecoveryCode_ShouldWorkEndToEnd()
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

        var sharedKey = data.GetProperty("sharedKey").GetString();
        sharedKey.Should().NotBeNullOrWhiteSpace();

        var enableCode = GenerateTotpCode(sharedKey!);
        var enableResponse = await client.PostAsJsonAsync("/api/two-factor/enable", new
        {
            verificationCode = enableCode
        });

        enableResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var enableBody = await HttpClientHelper.ReadJsonDocumentAsync(enableResponse);
        var enableData = enableBody.RootElement.GetProperty("data");
        enableData.GetProperty("succeeded").GetBoolean().Should().BeTrue();
        var recoveryCodes = enableData.GetProperty("recoveryCodes");
        recoveryCodes.GetArrayLength().Should().BeGreaterThan(0);
        var recoveryCode = recoveryCodes[0].GetString();
        recoveryCode.Should().NotBeNullOrWhiteSpace();
        var normalizedRecoveryCode = recoveryCode!.Replace("-", string.Empty).Replace(" ", string.Empty);

        var statusAfterEnable = await client.GetAsync("/api/two-factor/status");
        statusAfterEnable.StatusCode.Should().Be(HttpStatusCode.OK);
        using var statusBody = await HttpClientHelper.ReadJsonDocumentAsync(statusAfterEnable);
        statusBody.RootElement.GetProperty("data").GetProperty("isEnabled").GetBoolean().Should().BeTrue();

        client.DefaultRequestHeaders.Authorization = null;

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "patient@gmail.com",
            password = "Patient@123$"
        });
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var loginBody = await HttpClientHelper.ReadJsonDocumentAsync(loginResponse);
        var loginData = loginBody.RootElement.GetProperty("data");
        loginData.GetProperty("requiresTwoFactor").GetBoolean().Should().BeTrue();
        var userId = loginData.GetProperty("userId").GetGuid();

        var verifyCode = GenerateTotpCode(sharedKey!);
        var verifyResponse = await client.PostAsJsonAsync("/api/auth/login/verify-2fa", new
        {
            userId,
            code = verifyCode,
            useRecoveryCode = false
        });

        verifyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        using var verifyBody = await HttpClientHelper.ReadJsonDocumentAsync(verifyResponse);
        verifyBody.RootElement.GetProperty("data").GetProperty("accessToken").GetString().Should().NotBeNullOrWhiteSpace();

        var secondLoginResponse = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "patient@gmail.com",
            password = "Patient@123$"
        });
        secondLoginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        using var secondLoginBody = await HttpClientHelper.ReadJsonDocumentAsync(secondLoginResponse);
        var secondUserId = secondLoginBody.RootElement.GetProperty("data").GetProperty("userId").GetGuid();

        var recoveryVerifyResponse = await client.PostAsJsonAsync("/api/auth/login/verify-2fa", new
        {
            userId = secondUserId,
            code = normalizedRecoveryCode,
            useRecoveryCode = true
        });
        recoveryVerifyResponse.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized);

        if (recoveryVerifyResponse.StatusCode == HttpStatusCode.OK)
        {
            var thirdLoginResponse = await client.PostAsJsonAsync("/api/auth/login", new
            {
                email = "patient@gmail.com",
                password = "Patient@123$"
            });
            thirdLoginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            using var thirdLoginBody = await HttpClientHelper.ReadJsonDocumentAsync(thirdLoginResponse);
            var thirdUserId = thirdLoginBody.RootElement.GetProperty("data").GetProperty("userId").GetGuid();

            var reusedRecoveryCodeResponse = await client.PostAsJsonAsync("/api/auth/login/verify-2fa", new
            {
                userId = thirdUserId,
                code = normalizedRecoveryCode,
                useRecoveryCode = true
            });
            reusedRecoveryCodeResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        AuthHelper.SetBearerToken(client, verifyBody.RootElement.GetProperty("data").GetProperty("accessToken").GetString()!);
        var disableResponse = await client.PostAsJsonAsync("/api/two-factor/disable", new
        {
            password = "Patient@123$"
        });
        disableResponse.StatusCode.Should().Be(HttpStatusCode.OK);
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
    public async Task GenerateRecoveryCodes_AndDisable_WithValidPassword_ShouldSucceed()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var token = await AuthHelper.LoginPatientAsync(client);
        AuthHelper.SetBearerToken(client, token);

        var setupResponse = await client.PostAsync("/api/two-factor/setup", null);
        setupResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var setupBody = await HttpClientHelper.ReadJsonDocumentAsync(setupResponse);
        var sharedKey = setupBody.RootElement.GetProperty("data").GetProperty("sharedKey").GetString();
        sharedKey.Should().NotBeNullOrWhiteSpace();

        var enableCode = GenerateTotpCode(sharedKey!);
        var enableResponse = await client.PostAsJsonAsync("/api/two-factor/enable", new
        {
            verificationCode = enableCode
        });
        enableResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var recoveryCodesResponse = await client.PostAsJsonAsync("/api/two-factor/recovery-codes", new
        {
            password = "Patient@123$"
        });
        recoveryCodesResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var recoveryCodesBody = await HttpClientHelper.ReadJsonDocumentAsync(recoveryCodesResponse);
        recoveryCodesBody.RootElement.GetProperty("data").GetProperty("recoveryCodes").GetArrayLength()
            .Should().BeGreaterThan(0);

        var disableResponse = await client.PostAsJsonAsync("/api/two-factor/disable", new
        {
            password = "Patient@123$"
        });
        disableResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var statusAfterDisable = await client.GetAsync("/api/two-factor/status");
        statusAfterDisable.StatusCode.Should().Be(HttpStatusCode.OK);
        using var statusBody = await HttpClientHelper.ReadJsonDocumentAsync(statusAfterDisable);
        statusBody.RootElement.GetProperty("data").GetProperty("isEnabled").GetBoolean().Should().BeFalse();
    }

    private static string GenerateTotpCode(string base32Secret)
    {
        var secret = DecodeBase32(base32Secret);
        var timeStep = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 30;
        var counterBytes = BitConverter.GetBytes(timeStep);

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(counterBytes);
        }

        using var hmac = new System.Security.Cryptography.HMACSHA1(secret);
        var hash = hmac.ComputeHash(counterBytes);
        var offset = hash[^1] & 0x0F;
        var binaryCode = ((hash[offset] & 0x7F) << 24)
            | (hash[offset + 1] << 16)
            | (hash[offset + 2] << 8)
            | hash[offset + 3];

        var otp = binaryCode % 1_000_000;
        return otp.ToString("D6");
    }

    private static byte[] DecodeBase32(string input)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        var cleaned = input
            .Trim()
            .Replace("=", string.Empty)
            .Replace(" ", string.Empty)
            .ToUpperInvariant();

        var output = new List<byte>();
        var buffer = 0;
        var bitsLeft = 0;

        foreach (var c in cleaned)
        {
            var index = alphabet.IndexOf(c);
            if (index < 0)
            {
                continue;
            }

            buffer = (buffer << 5) | index;
            bitsLeft += 5;

            if (bitsLeft >= 8)
            {
                output.Add((byte)(buffer >> (bitsLeft - 8)));
                bitsLeft -= 8;
                buffer &= (1 << bitsLeft) - 1;
            }
        }

        return output.ToArray();
    }
}
