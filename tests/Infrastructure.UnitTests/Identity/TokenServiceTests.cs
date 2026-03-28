using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Infrastructure.Identity;
using Microsoft.Extensions.Options;

namespace Infrastructure.UnitTests.Identity;

public class TokenServiceTests
{
    private readonly TokenService _sut;
    private readonly JwtSettings _jwtSettings;

    public TokenServiceTests()
    {
        _jwtSettings = new JwtSettings
        {
            SecretKey = "ThisIsAVerySecureSecretKeyForTestingPurposes2024!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            AccessTokenExpiryMinutes = 15,
            ValidateIssuer = true,
            ValidateAudience = true,
            ClockSkewSeconds = 0
        };

        _sut = new TokenService(Options.Create(_jwtSettings));
    }

    #region GenerateAccessToken Tests

    [Fact]
    public async Task GenerateAccessTokenAsync_ShouldReturnValidToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var fullName = "Test User";
        var roles = new[] { "Patient" };

        // Act
        var result = await _sut.GenerateAccessTokenAsync(userId, email, fullName, roles);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.Jti.Should().NotBeNullOrEmpty();
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task GenerateAccessTokenAsync_ShouldContainCorrectClaims()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "doctor@aura.com";
        var fullName = "Dr. Smith";
        var roles = new[] { "Ophthalmologist" };

        // Act
        var result = await _sut.GenerateAccessTokenAsync(userId, email, fullName, roles);

        // Assert - Parse the token and check claims
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(result.AccessToken);

        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == email);
        jwt.Claims.Should().Contain(c => c.Type == "name" && c.Value == fullName);
        jwt.Claims.Should().Contain(c => c.Type == "role" && c.Value == "Ophthalmologist");
    }

    [Fact]
    public async Task GenerateAccessTokenAsync_WithMultipleRoles_ShouldContainAllRoles()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roles = new[] { "Admin", "Ophthalmologist" };

        // Act
        var result = await _sut.GenerateAccessTokenAsync(userId, "test@test.com", "Test", roles);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(result.AccessToken);

        var roleClaims = jwt.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();
        roleClaims.Should().Contain("Admin");
        roleClaims.Should().Contain("Ophthalmologist");
    }

    [Fact]
    public async Task GenerateAccessTokenAsync_WithAdditionalClaims_ShouldIncludeThem()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var additionalClaims = new[]
        {
            new Claim("profile_id", Guid.NewGuid().ToString()),
            new Claim("custom_claim", "custom_value")
        };

        // Act
        var result = await _sut.GenerateAccessTokenAsync(
            userId, "test@test.com", "Test", new[] { "Patient" }, additionalClaims);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(result.AccessToken);

        jwt.Claims.Should().Contain(c => c.Type == "profile_id");
        jwt.Claims.Should().Contain(c => c.Type == "custom_claim" && c.Value == "custom_value");
    }

    [Fact]
    public async Task GenerateAccessTokenAsync_ExpiresAt_ShouldMatchSettings()
    {
        // Act
        var before = DateTime.UtcNow;
        var result = await _sut.GenerateAccessTokenAsync(
            Guid.NewGuid(), "test@test.com", "Test", new[] { "Patient" });
        var after = DateTime.UtcNow;

        // Assert - should expire ~15 minutes from now
        var expectedMinExpiry = before.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes);
        var expectedMaxExpiry = after.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes);

        result.ExpiresAt.Should().BeOnOrAfter(expectedMinExpiry.AddSeconds(-1));
        result.ExpiresAt.Should().BeOnOrBefore(expectedMaxExpiry.AddSeconds(1));
    }

    [Fact]
    public async Task GenerateAccessTokenAsync_Jti_ShouldBeUniquePerCall()
    {
        // Act
        var result1 = await _sut.GenerateAccessTokenAsync(
            Guid.NewGuid(), "test@test.com", "Test", new[] { "Patient" });
        var result2 = await _sut.GenerateAccessTokenAsync(
            Guid.NewGuid(), "test@test.com", "Test", new[] { "Patient" });

        // Assert
        result1.Jti.Should().NotBe(result2.Jti);
    }

    [Fact]
    public async Task GenerateAccessTokenAsync_TokenFormat_ShouldBeValidJwt()
    {
        // Act
        var result = await _sut.GenerateAccessTokenAsync(
            Guid.NewGuid(), "test@test.com", "Test", new[] { "Patient" });

        // Assert - JWT has 3 parts separated by dots
        result.AccessToken.Split('.').Should().HaveCount(3);
    }

    #endregion

    #region GenerateRefreshToken Tests

    [Fact]
    public void GenerateRefreshToken_ShouldReturnNonEmptyString()
    {
        var result = _sut.GenerateRefreshToken();
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnBase64String()
    {
        var result = _sut.GenerateRefreshToken();

        // Should be valid base64
        var act = () => Convert.FromBase64String(result);
        act.Should().NotThrow();
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturn64BytesWhenDecoded()
    {
        var result = _sut.GenerateRefreshToken();
        var bytes = Convert.FromBase64String(result);
        bytes.Should().HaveCount(64);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldGenerateUniqueTokens()
    {
        var tokens = Enumerable.Range(0, 10).Select(_ => _sut.GenerateRefreshToken()).ToList();
        tokens.Distinct().Should().HaveCount(10);
    }

    #endregion

    #region ValidateToken Tests

    [Fact]
    public async Task ValidateToken_WithValidToken_ShouldReturnPrincipal()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tokenResult = await _sut.GenerateAccessTokenAsync(
            userId, "test@test.com", "Test", new[] { "Patient" });

        // Act
        var principal = _sut.ValidateToken(tokenResult.AccessToken);

        // Assert
        principal.Should().NotBeNull();
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ShouldReturnNull()
    {
        var principal = _sut.ValidateToken("invalid.token.string");
        principal.Should().BeNull();
    }

    [Fact]
    public void ValidateToken_WithEmptyString_ShouldReturnNull()
    {
        var principal = _sut.ValidateToken(string.Empty);
        principal.Should().BeNull();
    }

    [Fact]
    public async Task ValidateToken_WithTamperedToken_ShouldReturnNull()
    {
        // Arrange
        var tokenResult = await _sut.GenerateAccessTokenAsync(
            Guid.NewGuid(), "test@test.com", "Test", new[] { "Patient" });

        // Tamper with signature segment to guarantee signature mismatch.
        var segments = tokenResult.AccessToken.Split('.');
        segments.Should().HaveCount(3);

        var signature = segments[2];
        signature.Should().NotBeNullOrEmpty();
        var tamperedFirstChar = signature[0] == 'A' ? 'B' : 'A';
        var tamperedSignature = tamperedFirstChar + signature[1..];
        var tampered = string.Join('.', segments[0], segments[1], tamperedSignature);

        // Act
        var principal = _sut.ValidateToken(tampered);

        // Assert
        principal.Should().BeNull();
    }

    #endregion

    #region GetUserIdFromToken Tests

    [Fact]
    public async Task GetUserIdFromToken_WithValidToken_ShouldReturnUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tokenResult = await _sut.GenerateAccessTokenAsync(
            userId, "test@test.com", "Test", new[] { "Patient" });

        // Act
        var result = _sut.GetUserIdFromToken(tokenResult.AccessToken);

        // Assert
        result.Should().Be(userId);
    }

    [Fact]
    public void GetUserIdFromToken_WithInvalidToken_ShouldReturnNull()
    {
        var result = _sut.GetUserIdFromToken("invalid.token");
        result.Should().BeNull();
    }

    #endregion

    #region GetJtiFromToken Tests

    [Fact]
    public async Task GetJtiFromToken_WithValidToken_ShouldReturnJti()
    {
        // Arrange
        var tokenResult = await _sut.GenerateAccessTokenAsync(
            Guid.NewGuid(), "test@test.com", "Test", new[] { "Patient" });

        // Act
        var result = _sut.GetJtiFromToken(tokenResult.AccessToken);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Be(tokenResult.Jti);
    }

    [Fact]
    public void GetJtiFromToken_WithInvalidToken_ShouldReturnNull()
    {
        var result = _sut.GetJtiFromToken("invalid.token");
        result.Should().BeNull();
    }

    #endregion

    #region HashToken Tests

    [Fact]
    public void HashToken_ShouldReturnNonEmptyString()
    {
        var result = TokenService.HashToken("test-token");
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void HashToken_SameInput_ShouldReturnSameHash()
    {
        var hash1 = TokenService.HashToken("same-token");
        var hash2 = TokenService.HashToken("same-token");

        hash1.Should().Be(hash2);
    }

    [Fact]
    public void HashToken_DifferentInputs_ShouldReturnDifferentHashes()
    {
        var hash1 = TokenService.HashToken("token-1");
        var hash2 = TokenService.HashToken("token-2");

        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void HashToken_ShouldReturnBase64String()
    {
        var result = TokenService.HashToken("test-token");
        var act = () => Convert.FromBase64String(result);
        act.Should().NotThrow();
    }

    [Fact]
    public void HashToken_ShouldReturn32BytesWhenDecoded()
    {
        // SHA256 produces 32 bytes
        var result = TokenService.HashToken("test-token");
        var bytes = Convert.FromBase64String(result);
        bytes.Should().HaveCount(32);
    }

    #endregion
}
