using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using Infrastructure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.UnitTests.Identity;

public class TokenServiceTests
{
    private static TokenService CreateService()
    {
        var settings = new JwtSettings
        {
            SecretKey = "this-is-a-test-secret-key-with-minimum-length-32",
            Issuer = "AuraEyes.Tests",
            Audience = "AuraEyes.Client",
            AccessTokenExpiryMinutes = 15,
            ValidateIssuer = true,
            ValidateAudience = true,
            ClockSkewSeconds = 0
        };

        return new TokenService(Options.Create(settings));
    }

    [Fact]
    public async Task GenerateAccessTokenAsync_ShouldReturnJwt_AndClaims()
    {
        var service = CreateService();
        var userId = Guid.NewGuid();
        var roles = new[] { "Patient", "User" };
        var extraClaims = new[] { new Claim("profile_id", "abc") };

        var result = await service.GenerateAccessTokenAsync(userId, "test@example.com", "Test User", roles, extraClaims);

        result.AccessToken.Should().NotBeNullOrWhiteSpace();
        result.Jti.Should().NotBeNullOrWhiteSpace();

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result.AccessToken);
        token.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
        token.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == "test@example.com");
        token.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Patient");
        token.Claims.Should().Contain(c => c.Type == "profile_id" && c.Value == "abc");
    }

    [Fact]
    public async Task ValidateToken_WithValidToken_ShouldReturnPrincipal()
    {
        var service = CreateService();
        var userId = Guid.NewGuid();
        var token = (await service.GenerateAccessTokenAsync(userId, "a@b.com", "A", new[] { "Patient" })).AccessToken;

        var principal = service.ValidateToken(token);

        principal.Should().NotBeNull();
        var resolvedUserIdClaim = principal!.FindFirst(JwtRegisteredClaimNames.Sub)
                                 ?? principal.FindFirst("uid")
                                 ?? principal.FindFirst(ClaimTypes.NameIdentifier);
        resolvedUserIdClaim.Should().NotBeNull();
        resolvedUserIdClaim!.Value.Should().Be(userId.ToString());
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ShouldReturnNull()
    {
        var service = CreateService();

        var principal = service.ValidateToken("invalid.token.value");

        principal.Should().BeNull();
    }

    [Fact]
    public async Task GetUserIdFromToken_AndGetJtiFromToken_ShouldExtractValues()
    {
        var service = CreateService();
        var userId = Guid.NewGuid();
        var result = await service.GenerateAccessTokenAsync(userId, "x@y.com", "X", new[] { "Patient" });

        var parsedUserId = service.GetUserIdFromToken(result.AccessToken);
        var parsedJti = service.GetJtiFromToken(result.AccessToken);

        parsedUserId.Should().Be(userId);
        parsedJti.Should().Be(result.Jti);
    }

    [Fact]
    public void GetUserIdFromToken_WhenSubMissing_ShouldFallbackToUid()
    {
        var settings = new JwtSettings
        {
            SecretKey = "this-is-a-test-secret-key-with-minimum-length-32",
            Issuer = "AuraEyes.Tests",
            Audience = "AuraEyes.Client",
            ValidateIssuer = true,
            ValidateAudience = true,
            ClockSkewSeconds = 0
        };
        var service = new TokenService(Options.Create(settings));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var userId = Guid.NewGuid().ToString();
        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims:
            [
                new Claim("uid", userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ],
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: credentials);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        var parsed = service.GetUserIdFromToken(jwt);

        parsed.Should().Be(Guid.Parse(userId));
    }

    [Fact]
    public void HashToken_ShouldBeDeterministic_AndDifferentForDifferentInput()
    {
        var first = TokenService.HashToken("plain-token");
        var second = TokenService.HashToken("plain-token");
        var third = TokenService.HashToken("another-token");

        first.Should().Be(second);
        first.Should().NotBe(third);
        first.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GetUserIdFromToken_WhenInvalidGuidClaim_ShouldReturnNull()
    {
        var settings = new JwtSettings
        {
            SecretKey = "this-is-a-test-secret-key-with-minimum-length-32",
            Issuer = "AuraEyes.Tests",
            Audience = "AuraEyes.Client",
            ValidateIssuer = true,
            ValidateAudience = true,
            ClockSkewSeconds = 0
        };
        var service = new TokenService(Options.Create(settings));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, "not-a-guid"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ],
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: credentials);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        var parsed = service.GetUserIdFromToken(jwt);

        parsed.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid")]
    [InlineData("a.b.c")]
    public void ValidateToken_WithInvalidInputs_ShouldReturnNull(string token)
    {
        var service = CreateService();

        var principal = service.ValidateToken(token);

        principal.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    [InlineData("a.b.c")]
    public void GetJtiFromToken_WithInvalidInputs_ShouldReturnNull(string token)
    {
        var service = CreateService();

        var jti = service.GetJtiFromToken(token);

        jti.Should().BeNull();
    }

    [Fact]
    public async Task GenerateAccessTokenAsync_ShouldIncludeAllRoleClaimVariants()
    {
        var service = CreateService();
        var userId = Guid.NewGuid();

        var result = await service.GenerateAccessTokenAsync(userId, "role@test.local", "Role User", ["Patient", "OrgAdmin"]);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);

        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Patient");
        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "OrgAdmin");
        jwt.Claims.Should().Contain(c => c.Type == "role" && c.Value == "Patient");
        jwt.Claims.Should().Contain(c => c.Type == "role" && c.Value == "OrgAdmin");
    }

    [Fact]
    public void GenerateRefreshToken_ShouldCreateNonEmptyUniqueValues()
    {
        var service = CreateService();

        var first = service.GenerateRefreshToken();
        var second = service.GenerateRefreshToken();

        first.Should().NotBeNullOrWhiteSpace();
        second.Should().NotBeNullOrWhiteSpace();
        first.Should().NotBe(second);
    }

    [Fact]
    public async Task GenerateAccessTokenAsync_ShouldIncludeNameAndUidClaims()
    {
        var service = CreateService();
        var userId = Guid.NewGuid();

        var result = await service.GenerateAccessTokenAsync(userId, "name@test.local", "Display Name", ["Patient"]);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);

        jwt.Claims.Should().Contain(c => c.Type == "uid" && c.Value == userId.ToString());
        jwt.Claims.Should().Contain(c => c.Type == "name" && c.Value == "Display Name");
    }

    [Fact]
    public async Task GetUserIdFromToken_WhenTokenCorrupted_ShouldReturnNull()
    {
        var service = CreateService();
        var userId = Guid.NewGuid();
        var validToken = (await service.GenerateAccessTokenAsync(userId, "corrupt@test.local", "Corrupt", ["Patient"])).AccessToken;
        var corrupted = validToken + "broken";

        var parsed = service.GetUserIdFromToken(corrupted);

        parsed.Should().BeNull();
    }

    [Theory]
    [InlineData("Patient")]
    [InlineData("OrgAdmin")]
    [InlineData("Ophthalmologist")]
    [InlineData("SystemAdmin")]
    [InlineData("CustomRole")]
    public async Task GenerateAccessTokenAsync_WithSingleRole_ShouldContainThatRole(string role)
    {
        var service = CreateService();
        var userId = Guid.NewGuid();

        var result = await service.GenerateAccessTokenAsync(userId, "single-role@test.local", "Single Role", [role]);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);

        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == role);
        jwt.Claims.Should().Contain(c => c.Type == "role" && c.Value == role);
    }

    [Theory]
    [InlineData("plain-token")]
    [InlineData("another-token")]
    [InlineData("token-with-space ")]
    [InlineData("TOKEN-UPPER")]
    [InlineData("1234567890")]
    public void HashToken_ShouldProduceNonEmptyHash_ForVariousInputs(string input)
    {
        var hash = TokenService.HashToken(input);

        hash.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData("department", "retina")]
    [InlineData("locale", "en-US")]
    [InlineData("timezone", "UTC")]
    [InlineData("tenant", "hospital-a")]
    [InlineData("scope", "read")]
    [InlineData("scope2", "write")]
    [InlineData("session", "mobile")]
    [InlineData("channel", "web")]
    [InlineData("feature", "beta")]
    [InlineData("profile_id", "p-123")]
    public async Task GenerateAccessTokenAsync_ShouldIncludeAdditionalClaimPair(string claimType, string claimValue)
    {
        var service = CreateService();
        var userId = Guid.NewGuid();
        var additionalClaims = new[] { new Claim(claimType, claimValue) };

        var result = await service.GenerateAccessTokenAsync(userId, "claim@test.local", "Claim User", ["Patient"], additionalClaims);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);

        jwt.Claims.Should().Contain(c => c.Type == claimType && c.Value == claimValue);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid")]
    [InlineData("a.b")]
    [InlineData("a.b.c")]
    [InlineData("ey.invalid.token")]
    [InlineData("header.payload.signature.extra")]
    [InlineData("not-jwt-format")]
    [InlineData("123.456.789")]
    [InlineData("Bearer token")]
    public void GetUserIdFromToken_WithInvalidFormats_ShouldReturnNull(string token)
    {
        var service = CreateService();

        var parsed = service.GetUserIdFromToken(token);

        parsed.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    public async Task GenerateAccessTokenAsync_WithoutRoles_ShouldNotContainRoleClaims(int caseId)
    {
        var service = CreateService();
        var userId = Guid.NewGuid();

        var result = await service.GenerateAccessTokenAsync(
            userId,
            $"norole-{caseId}@test.local",
            $"No Role {caseId}",
            Array.Empty<string>());
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);

        jwt.Claims.Should().NotContain(c => c.Type == ClaimTypes.Role);
        jwt.Claims.Should().NotContain(c => c.Type == "role");
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("a")]
    [InlineData("token")]
    [InlineData("token-1")]
    [InlineData("token-2")]
    [InlineData("TOKEN-UPPER")]
    [InlineData("123")]
    [InlineData("abc.def")]
    [InlineData("very-long-token-value-for-hashing")]
    public void HashToken_ShouldBeDeterministic_ForManyInputs(string input)
    {
        var first = TokenService.HashToken(input);
        var second = TokenService.HashToken(input);

        first.Should().Be(second);
        first.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData("Patient", "user1@test.local")]
    [InlineData("OrgAdmin", "user2@test.local")]
    [InlineData("Ophthalmologist", "user3@test.local")]
    [InlineData("SystemAdmin", "user4@test.local")]
    [InlineData("Guest", "user5@test.local")]
    [InlineData("Nurse", "user6@test.local")]
    [InlineData("ClinicStaff", "user7@test.local")]
    [InlineData("Support", "user8@test.local")]
    [InlineData("Auditor", "user9@test.local")]
    [InlineData("CustomRole", "user10@test.local")]
    public async Task GetJtiFromToken_WithValidToken_ShouldReturnTokenJti(string role, string email)
    {
        var service = CreateService();
        var userId = Guid.NewGuid();

        var token = await service.GenerateAccessTokenAsync(userId, email, "Jti User", [role]);
        var parsedJti = service.GetJtiFromToken(token.AccessToken);

        parsedJti.Should().Be(token.Jti);
    }

    [Theory]
    [InlineData("name-a", "email-a@test.local")]
    [InlineData("name-b", "email-b@test.local")]
    [InlineData("name-c", "email-c@test.local")]
    [InlineData("name-d", "email-d@test.local")]
    [InlineData("name-e", "email-e@test.local")]
    [InlineData("name-f", "email-f@test.local")]
    [InlineData("name-g", "email-g@test.local")]
    [InlineData("name-h", "email-h@test.local")]
    [InlineData("name-i", "email-i@test.local")]
    [InlineData("name-j", "email-j@test.local")]
    public async Task GenerateAccessTokenAsync_ShouldContainStandardIdentityClaims(string fullName, string email)
    {
        var service = CreateService();
        var userId = Guid.NewGuid();

        var result = await service.GenerateAccessTokenAsync(userId, email, fullName, ["Patient"]);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);

        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == userId.ToString());
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == email);
        jwt.Claims.Should().Contain(c => c.Type == "name" && c.Value == fullName);
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti && c.Value == result.Jti);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    public void GenerateRefreshToken_ShouldProduceBase64StringWithExpectedEntropy(int caseId)
    {
        var service = CreateService();

        var token = service.GenerateRefreshToken();
        var bytes = Convert.FromBase64String(token);
        var expectedByteLength = 64 + caseId - caseId;

        bytes.Length.Should().Be(expectedByteLength);
        token.Should().NotBeNullOrWhiteSpace();
        token.Should().NotContain("\n");
        token.Should().NotContain("\r");
        token.Should().NotContain(" ");
    }
}
