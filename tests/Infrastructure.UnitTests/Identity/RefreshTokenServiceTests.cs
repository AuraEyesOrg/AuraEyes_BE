using FluentAssertions;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitTests.Identity;

public class RefreshTokenServiceTests
{
    [Fact]
    public async Task CreateRefreshTokenAsync_ShouldPersistAndReturnId()
    {
        await using var context = CreateContext();
        var service = new RefreshTokenService(context);
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);

        var tokenId = await service.CreateRefreshTokenAsync(userId, "hash-1", "jwt-1");

        tokenId.Should().NotBe(Guid.Empty);
        var dbToken = await context.RefreshTokens.FirstAsync(x => x.Id == tokenId);
        dbToken.UserId.Should().Be(userId);
        dbToken.TokenHash.Should().Be("hash-1");
    }

    [Fact]
    public async Task GetByTokenHashAsync_WhenFound_ShouldReturnDto()
    {
        await using var context = CreateContext();
        var service = new RefreshTokenService(context);
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);
        var tokenId = await service.CreateRefreshTokenAsync(userId, "hash-lookup", "jwt-lookup");

        var result = await service.GetByTokenHashAsync("hash-lookup");

        result.Should().NotBeNull();
        result!.Id.Should().Be(tokenId);
        result.UserId.Should().Be(userId);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetByTokenHashAsync_WhenNotFound_ShouldReturnNull()
    {
        await using var context = CreateContext();
        var service = new RefreshTokenService(context);

        var result = await service.GetByTokenHashAsync("missing");

        result.Should().BeNull();
    }

    [Fact]
    public async Task RotateRefreshTokenAsync_ShouldCreateNewAndMarkOldUsed()
    {
        await using var context = CreateContext();
        var service = new RefreshTokenService(context);
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);
        var oldId = await service.CreateRefreshTokenAsync(userId, "old-hash", "old-jti");

        var newId = await service.RotateRefreshTokenAsync(oldId, "new-hash", "new-jti");

        newId.Should().NotBe(Guid.Empty);
        var oldToken = await context.RefreshTokens.FirstAsync(x => x.Id == oldId);
        var newToken = await context.RefreshTokens.FirstAsync(x => x.Id == newId);
        oldToken.UsedAt.Should().NotBeNull();
        oldToken.ReplacedByTokenId.Should().Be(newId);
        newToken.TokenHash.Should().Be("new-hash");
    }

    [Fact]
    public async Task RotateRefreshTokenAsync_WhenOriginalTokenMissing_ShouldThrow()
    {
        await using var context = CreateContext();
        var service = new RefreshTokenService(context);

        var act = async () => await service.RotateRefreshTokenAsync(Guid.NewGuid(), "h", "j");

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Original token not found");
    }

    [Fact]
    public async Task RevokeTokenAsync_WhenTokenExists_ShouldSetRevokedFields()
    {
        await using var context = CreateContext();
        var service = new RefreshTokenService(context);
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);
        var tokenId = await service.CreateRefreshTokenAsync(userId, "hash-revoke", "jwt");

        await service.RevokeTokenAsync(tokenId, "logout");

        var token = await context.RefreshTokens.FirstAsync(x => x.Id == tokenId);
        token.RevokedAt.Should().NotBeNull();
        token.RevokedReason.Should().Be("logout");
    }

    [Fact]
    public async Task RevokeAllUserTokensAsync_ShouldRevokeAllMatchingTokens()
    {
        await using var context = CreateContext();
        var service = new RefreshTokenService(context);
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);
        await SeedUserAsync(context, Guid.NewGuid());
        await service.CreateRefreshTokenAsync(userId, "hash-a", "jwt-a");
        await service.CreateRefreshTokenAsync(userId, "hash-b", "jwt-b");
        await service.CreateRefreshTokenAsync(Guid.NewGuid(), "hash-other", "jwt-c");

        await service.RevokeAllUserTokensAsync(userId, "logout_all");

        var userTokens = await context.RefreshTokens.Where(x => x.UserId == userId).ToListAsync();
        userTokens.Should().OnlyContain(t => t.RevokedAt.HasValue);
    }

    [Fact]
    public async Task RevokeTokenFamilyAsync_ShouldRevokeAllUserTokensWhenTokenFound()
    {
        await using var context = CreateContext();
        var service = new RefreshTokenService(context);
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);
        var tokenId = await service.CreateRefreshTokenAsync(userId, "family-a", "jti-a");
        await service.CreateRefreshTokenAsync(userId, "family-b", "jti-b");

        await service.RevokeTokenFamilyAsync(tokenId, "token_reuse_detected");

        var userTokens = await context.RefreshTokens.Where(x => x.UserId == userId).ToListAsync();
        userTokens.Should().OnlyContain(t => t.RevokedReason == "token_reuse_detected");
    }

    [Fact]
    public async Task RevokeTokenFamilyAsync_WhenTokenMissing_ShouldDoNothing()
    {
        await using var context = CreateContext();
        var service = new RefreshTokenService(context);

        var act = async () => await service.RevokeTokenFamilyAsync(Guid.NewGuid(), "reuse");

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task CleanupExpiredTokensAsync_ShouldDeleteExpiredAndOldRevoked()
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);

        var expired = RefreshToken.Create(userId, "expired", "e", expiryDays: -40);
        var oldRevoked = RefreshToken.Create(userId, "revoked-old", "r", expiryDays: 40);
        oldRevoked.RevokedAt = DateTime.UtcNow.AddDays(-40);
        oldRevoked.RevokedReason = "old";
        var active = RefreshToken.Create(userId, "active", "a", expiryDays: 40);

        await context.RefreshTokens.AddRangeAsync(expired, oldRevoked, active);
        await context.SaveChangesAsync();

        var service = new RefreshTokenService(context);
        var deleted = await service.CleanupExpiredTokensAsync(daysToKeep: 30);

        deleted.Should().Be(2);
        var hashes = await context.RefreshTokens.Select(x => x.TokenHash).ToListAsync();
        hashes.Should().ContainSingle().Which.Should().Be("active");
    }

    [Theory]
    [InlineData("logout")]
    [InlineData("manual_revocation")]
    [InlineData("security_event")]
    public async Task RevokeTokenAsync_WithDifferentReasons_ShouldPersistReason(string reason)
    {
        await using var context = CreateContext();
        var service = new RefreshTokenService(context);
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);
        var tokenId = await service.CreateRefreshTokenAsync(userId, $"hash-{reason}", "jwt-r");

        await service.RevokeTokenAsync(tokenId, reason);

        var token = await context.RefreshTokens.FirstAsync(x => x.Id == tokenId);
        token.RevokedReason.Should().Be(reason);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(7)]
    [InlineData(30)]
    public async Task CleanupExpiredTokensAsync_WithDifferentRetentionDays_ShouldNotDeleteFreshTokens(int keepDays)
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);
        await context.RefreshTokens.AddAsync(RefreshToken.Create(userId, $"active-{keepDays}", "jwt", expiryDays: 20));
        await context.SaveChangesAsync();
        var service = new RefreshTokenService(context);

        var deleted = await service.CleanupExpiredTokensAsync(daysToKeep: keepDays);

        deleted.Should().Be(0);
        (await context.RefreshTokens.CountAsync()).Should().Be(1);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(30)]
    public async Task CreateRefreshTokenAsync_WithCustomExpiry_ShouldSetFutureExpiry(int expiryDays)
    {
        await using var context = CreateContext();
        var service = new RefreshTokenService(context);
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);

        var id = await service.CreateRefreshTokenAsync(userId, $"hash-{expiryDays}", "jwt", expiryDays);

        var token = await context.RefreshTokens.FirstAsync(x => x.Id == id);
        token.ExpiresAt.Should().BeAfter(DateTime.UtcNow.AddDays(expiryDays - 1).AddMinutes(-1));
    }

    [Theory]
    [InlineData("hash-none")]
    [InlineData("hash-space ")]
    [InlineData("HASH-UPPER")]
    public async Task GetByTokenHashAsync_ShouldMatchStoredHashExactly(string hash)
    {
        await using var context = CreateContext();
        var service = new RefreshTokenService(context);
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);
        await service.CreateRefreshTokenAsync(userId, hash, "jwt");

        var result = await service.GetByTokenHashAsync(hash);

        result.Should().NotBeNull();
        result!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task RevokeTokenAsync_WhenTokenMissing_ShouldNotThrow()
    {
        await using var context = CreateContext();
        var service = new RefreshTokenService(context);

        var act = async () => await service.RevokeTokenAsync(Guid.NewGuid(), "none");

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task RotateRefreshTokenAsync_ShouldKeepSameUserAndDeactivateOldToken()
    {
        await using var context = CreateContext();
        var service = new RefreshTokenService(context);
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);
        var oldId = await service.CreateRefreshTokenAsync(userId, "rotate-old", "old-jti");

        var newId = await service.RotateRefreshTokenAsync(oldId, "rotate-new", "new-jti");

        var oldToken = await context.RefreshTokens.FirstAsync(x => x.Id == oldId);
        var newToken = await context.RefreshTokens.FirstAsync(x => x.Id == newId);
        oldToken.IsActive.Should().BeFalse();
        newToken.UserId.Should().Be(userId);
        newToken.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task RevokeAllUserTokensAsync_ShouldNotAffectOtherUsersTokens()
    {
        await using var context = CreateContext();
        var service = new RefreshTokenService(context);
        var userA = Guid.NewGuid();
        var userB = Guid.NewGuid();
        await SeedUserAsync(context, userA);
        await SeedUserAsync(context, userB);
        await service.CreateRefreshTokenAsync(userA, "a-1", "a-jti");
        await service.CreateRefreshTokenAsync(userB, "b-1", "b-jti");

        await service.RevokeAllUserTokensAsync(userA, "logout_all");

        var tokenA = await context.RefreshTokens.FirstAsync(x => x.UserId == userA);
        var tokenB = await context.RefreshTokens.FirstAsync(x => x.UserId == userB);
        tokenA.RevokedAt.Should().NotBeNull();
        tokenB.RevokedAt.Should().BeNull();
    }

    [Theory]
    [InlineData("logout_all")]
    [InlineData("security_event")]
    [InlineData("admin_forced_logout")]
    [InlineData("password_changed")]
    [InlineData("device_compromised")]
    [InlineData("suspicious_activity")]
    [InlineData("session_reset")]
    [InlineData("manual_cleanup")]
    public async Task RevokeAllUserTokensAsync_WithVariousReasons_ShouldApplyReasonToAllTokens(string reason)
    {
        await using var context = CreateContext();
        var service = new RefreshTokenService(context);
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);
        await service.CreateRefreshTokenAsync(userId, $"h1-{reason}", "j1");
        await service.CreateRefreshTokenAsync(userId, $"h2-{reason}", "j2");

        await service.RevokeAllUserTokensAsync(userId, reason);

        var tokens = await context.RefreshTokens.Where(x => x.UserId == userId).ToListAsync();
        tokens.Should().HaveCount(2);
        tokens.Should().OnlyContain(t => t.RevokedReason == reason && t.RevokedAt.HasValue);
    }

    [Theory]
    [InlineData("web", "127.0.0.1")]
    [InlineData("ios", "10.0.0.2")]
    [InlineData("android", "192.168.1.9")]
    [InlineData("chrome", "172.16.0.3")]
    [InlineData("firefox", "8.8.8.8")]
    [InlineData("edge", "1.1.1.1")]
    [InlineData("tablet", "203.0.113.7")]
    [InlineData("desktop", "198.51.100.2")]
    public async Task CreateRefreshTokenAsync_WithDeviceAndIp_ShouldPersistMetadata(string deviceInfo, string ipAddress)
    {
        await using var context = CreateContext();
        var service = new RefreshTokenService(context);
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);

        var id = await service.CreateRefreshTokenAsync(userId, $"hash-{deviceInfo}", "jwt", 7, deviceInfo, ipAddress);

        var token = await context.RefreshTokens.FirstAsync(x => x.Id == id);
        token.DeviceInfo.Should().Be(deviceInfo);
        token.IpAddress.Should().Be(ipAddress);
    }

    [Theory]
    [InlineData("reuse_detected")]
    [InlineData("credential_compromise")]
    [InlineData("suspicious_rotation")]
    [InlineData("multiple_invalid_attempts")]
    [InlineData("manual_security_revocation")]
    [InlineData("cross_device_revoke")]
    [InlineData("risk_policy_triggered")]
    [InlineData("admin_token_family_reset")]
    public async Task RevokeTokenFamilyAsync_WithVariousReasons_ShouldApplyReasonToWholeFamily(string reason)
    {
        await using var context = CreateContext();
        var service = new RefreshTokenService(context);
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);
        var tokenA = await service.CreateRefreshTokenAsync(userId, $"fam-a-{reason}", "jti-a");
        await service.CreateRefreshTokenAsync(userId, $"fam-b-{reason}", "jti-b");

        await service.RevokeTokenFamilyAsync(tokenA, reason);

        var tokens = await context.RefreshTokens.Where(x => x.UserId == userId).ToListAsync();
        tokens.Should().HaveCount(2);
        tokens.Should().OnlyContain(t => t.RevokedReason == reason && t.RevokedAt.HasValue);
    }

    [Theory]
    [InlineData(1, "ios", "10.0.0.10")]
    [InlineData(3, "android", "10.0.0.11")]
    [InlineData(5, "web", "10.0.0.12")]
    [InlineData(7, "desktop", "10.0.0.13")]
    [InlineData(14, "tablet", "10.0.0.14")]
    [InlineData(21, "edge", "10.0.0.15")]
    [InlineData(30, "chrome", "10.0.0.16")]
    [InlineData(45, "firefox", "10.0.0.17")]
    public async Task RotateRefreshTokenAsync_WithMetadataAndExpiry_ShouldCreateConfiguredNewToken(
        int expiryDays,
        string deviceInfo,
        string ipAddress)
    {
        await using var context = CreateContext();
        var service = new RefreshTokenService(context);
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);
        var oldId = await service.CreateRefreshTokenAsync(userId, $"old-{expiryDays}", "old-jti");

        var newId = await service.RotateRefreshTokenAsync(oldId, $"new-{expiryDays}", "new-jti", expiryDays, deviceInfo, ipAddress);

        var token = await context.RefreshTokens.FirstAsync(x => x.Id == newId);
        token.DeviceInfo.Should().Be(deviceInfo);
        token.IpAddress.Should().Be(ipAddress);
        token.ExpiresAt.Should().BeAfter(DateTime.UtcNow.AddDays(expiryDays - 1).AddMinutes(-1));
    }

    [Theory]
    [InlineData("CaseSensitive")]
    [InlineData("casesensitive")]
    [InlineData("CASESENSITIVE")]
    [InlineData("mix-ed_01")]
    [InlineData("hash.with.dot")]
    [InlineData("hash/with/slash")]
    [InlineData("hash:with:colon")]
    [InlineData("hash+plus")]
    [InlineData("hash=equals")]
    [InlineData("hash-space")]
    public async Task GetByTokenHashAsync_WithDifferentStoredHashes_ShouldReturnOnlyExactMatch(string storedHash)
    {
        await using var context = CreateContext();
        var service = new RefreshTokenService(context);
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);
        await service.CreateRefreshTokenAsync(userId, storedHash, "jti-1");

        var exact = await service.GetByTokenHashAsync(storedHash);
        var mismatch = await service.GetByTokenHashAsync($"{storedHash}_other");

        exact.Should().NotBeNull();
        mismatch.Should().BeNull();
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    [InlineData(3, 0)]
    [InlineData(7, 0)]
    [InlineData(14, 0)]
    [InlineData(30, 0)]
    [InlineData(60, 0)]
    [InlineData(90, 0)]
    [InlineData(120, 0)]
    [InlineData(180, 0)]
    public async Task CleanupExpiredTokensAsync_WithRecentRevokedToken_ShouldRespectRetentionCutoff(int keepDays, int expectedDeleted)
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);

        var recentRevoked = RefreshToken.Create(userId, $"recent-{keepDays}", "jti-recent", expiryDays: 30);
        recentRevoked.RevokedAt = DateTime.UtcNow.AddDays(-Math.Max(0, keepDays - 1));
        recentRevoked.RevokedReason = "recent";
        await context.RefreshTokens.AddAsync(recentRevoked);
        await context.SaveChangesAsync();

        var service = new RefreshTokenService(context);
        var deleted = await service.CleanupExpiredTokensAsync(keepDays);

        deleted.Should().Be(expectedDeleted);
        (await context.RefreshTokens.CountAsync()).Should().Be(1 - expectedDeleted);
    }

    [Theory]
    [InlineData("logout_all")]
    [InlineData("security_event")]
    [InlineData("admin_forced_logout")]
    [InlineData("password_changed")]
    [InlineData("device_compromised")]
    [InlineData("suspicious_activity")]
    [InlineData("session_reset")]
    [InlineData("manual_cleanup")]
    [InlineData("token_reuse_detected")]
    [InlineData("policy_enforcement")]
    public async Task RevokeAllUserTokensAsync_ShouldNotModifyAlreadyRevokedTokens(string reason)
    {
        await using var context = CreateContext();
        var service = new RefreshTokenService(context);
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);

        var activeId = await service.CreateRefreshTokenAsync(userId, $"active-{reason}", "active-jti");
        var alreadyRevokedId = await service.CreateRefreshTokenAsync(userId, $"revoked-{reason}", "revoked-jti");
        var alreadyRevoked = await context.RefreshTokens.FirstAsync(x => x.Id == alreadyRevokedId);
        alreadyRevoked.RevokedAt = DateTime.UtcNow.AddDays(-2);
        alreadyRevoked.RevokedReason = "pre_existing_reason";
        await context.SaveChangesAsync();

        await service.RevokeAllUserTokensAsync(userId, reason);

        var active = await context.RefreshTokens.FirstAsync(x => x.Id == activeId);
        var revoked = await context.RefreshTokens.FirstAsync(x => x.Id == alreadyRevokedId);
        active.RevokedReason.Should().Be(reason);
        active.RevokedAt.Should().NotBeNull();
        revoked.RevokedReason.Should().Be("pre_existing_reason");
    }

    [Theory]
    [InlineData(30, -40, 1)]
    [InlineData(30, -31, 1)]
    [InlineData(30, -30, 1)]
    [InlineData(30, -29, 0)]
    [InlineData(15, -20, 1)]
    [InlineData(15, -15, 1)]
    [InlineData(15, -14, 0)]
    [InlineData(7, -8, 1)]
    [InlineData(7, -7, 1)]
    [InlineData(7, -6, 0)]
    public async Task CleanupExpiredTokensAsync_ShouldDeleteByCutoffBoundary(int keepDays, int expiryOffsetDays, int expectedDeleted)
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        await SeedUserAsync(context, userId);

        var token = RefreshToken.Create(userId, $"cutoff-{keepDays}-{expiryOffsetDays}", "cutoff-jti", expiryDays: 90);
        token.ExpiresAt = DateTime.UtcNow.AddDays(expiryOffsetDays);
        await context.RefreshTokens.AddAsync(token);
        await context.SaveChangesAsync();

        var service = new RefreshTokenService(context);
        var deleted = await service.CleanupExpiredTokensAsync(daysToKeep: keepDays);

        deleted.Should().Be(expectedDeleted);
        (await context.RefreshTokens.CountAsync()).Should().Be(1 - expectedDeleted);
    }

    [Theory]
    [InlineData("reuse_detected")]
    [InlineData("credential_compromise")]
    [InlineData("suspicious_rotation")]
    [InlineData("multiple_invalid_attempts")]
    [InlineData("manual_security_revocation")]
    [InlineData("cross_device_revoke")]
    [InlineData("risk_policy_triggered")]
    [InlineData("admin_token_family_reset")]
    [InlineData("session_hijack")]
    [InlineData("anomaly_detected")]
    public async Task RevokeTokenFamilyAsync_ShouldNotAffectOtherUserFamilies(string reason)
    {
        await using var context = CreateContext();
        var service = new RefreshTokenService(context);
        var userA = Guid.NewGuid();
        var userB = Guid.NewGuid();
        await SeedUserAsync(context, userA);
        await SeedUserAsync(context, userB);

        var familyTokenA = await service.CreateRefreshTokenAsync(userA, $"a1-{reason}", "a1-jti");
        await service.CreateRefreshTokenAsync(userA, $"a2-{reason}", "a2-jti");
        var tokenB = await service.CreateRefreshTokenAsync(userB, $"b1-{reason}", "b1-jti");

        await service.RevokeTokenFamilyAsync(familyTokenA, reason);

        var tokensA = await context.RefreshTokens.Where(x => x.UserId == userA).ToListAsync();
        var tokenBReload = await context.RefreshTokens.FirstAsync(x => x.Id == tokenB);
        tokensA.Should().OnlyContain(t => t.RevokedAt.HasValue && t.RevokedReason == reason);
        tokenBReload.RevokedAt.Should().BeNull();
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static async Task SeedUserAsync(ApplicationDbContext context, Guid userId)
    {
        if (await context.Users.AnyAsync(x => x.Id == userId))
        {
            return;
        }

        await context.Users.AddAsync(new ApplicationUser
        {
            Id = userId,
            UserName = $"{userId}@test.local",
            Email = $"{userId}@test.local",
            FullName = "Test User",
            IsDeleted = false,
            IsActive = true
        });
        await context.SaveChangesAsync();
    }
}
