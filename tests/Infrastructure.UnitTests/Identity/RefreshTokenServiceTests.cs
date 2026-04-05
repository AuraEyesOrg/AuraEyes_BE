using FluentAssertions;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitTests.Identity;

public class RefreshTokenServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private static RefreshTokenService CreateService(ApplicationDbContext context)
        => new(context);

    private static async Task<Guid> SeedUserAsync(ApplicationDbContext context)
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "test@auraeyes.vn",
            NormalizedUserName = "TEST@AURAEYES.VN",
            Email = "test@auraeyes.vn",
            NormalizedEmail = "TEST@AURAEYES.VN",
            FullName = "Test User",
            EmailConfirmed = true
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user.Id;
    }

    #region CreateRefreshTokenAsync

    [Fact]
    public async Task CreateRefreshTokenAsync_ShouldReturnNonEmptyGuid()
    {
        await using var context = CreateContext();
        var userId = await SeedUserAsync(context);
        var service = CreateService(context);

        var id = await service.CreateRefreshTokenAsync(userId, "hash123", "jti-001");

        id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateRefreshTokenAsync_ShouldPersistToken()
    {
        await using var context = CreateContext();
        var userId = await SeedUserAsync(context);
        var service = CreateService(context);

        await service.CreateRefreshTokenAsync(
            userId, "hash-abc", "jti-abc", 14, "Chrome", "192.168.1.1");

        var token = await context.RefreshTokens.FirstOrDefaultAsync();
        token.Should().NotBeNull();
        token!.UserId.Should().Be(userId);
        token.TokenHash.Should().Be("hash-abc");
        token.JwtId.Should().Be("jti-abc");
        token.DeviceInfo.Should().Be("Chrome");
        token.IpAddress.Should().Be("192.168.1.1");
        token.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task CreateRefreshTokenAsync_WithDefaultExpiry_ShouldExpireIn7Days()
    {
        await using var context = CreateContext();
        var userId = await SeedUserAsync(context);
        var service = CreateService(context);
        var before = DateTime.UtcNow;

        await service.CreateRefreshTokenAsync(userId, "hash", "jti");

        var token = await context.RefreshTokens.FirstAsync();
        token.ExpiresAt.Should().BeCloseTo(before.AddDays(7), TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task CreateRefreshTokenAsync_WithCustomExpiry_ShouldUseCustomDays()
    {
        await using var context = CreateContext();
        var userId = await SeedUserAsync(context);
        var service = CreateService(context);
        var before = DateTime.UtcNow;

        await service.CreateRefreshTokenAsync(userId, "hash", "jti", expiryDays: 30);

        var token = await context.RefreshTokens.FirstAsync();
        token.ExpiresAt.Should().BeCloseTo(before.AddDays(30), TimeSpan.FromSeconds(5));
    }

    #endregion

    #region GetByTokenHashAsync

    [Fact]
    public async Task GetByTokenHashAsync_WhenTokenExists_ShouldReturnDto()
    {
        await using var context = CreateContext();
        var userId = await SeedUserAsync(context);
        var service = CreateService(context);
        await service.CreateRefreshTokenAsync(userId, "lookup-hash", "jti-lookup");

        var dto = await service.GetByTokenHashAsync("lookup-hash");

        dto.Should().NotBeNull();
        dto!.UserId.Should().Be(userId);
        dto.JwtId.Should().Be("jti-lookup");
        dto.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetByTokenHashAsync_WhenTokenDoesNotExist_ShouldReturnNull()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var dto = await service.GetByTokenHashAsync("nonexistent-hash");

        dto.Should().BeNull();
    }

    [Fact]
    public async Task GetByTokenHashAsync_ShouldReturnCorrectTimestamps()
    {
        await using var context = CreateContext();
        var userId = await SeedUserAsync(context);
        var service = CreateService(context);
        var before = DateTime.UtcNow;
        await service.CreateRefreshTokenAsync(userId, "ts-hash", "jti-ts");

        var dto = await service.GetByTokenHashAsync("ts-hash");

        dto.Should().NotBeNull();
        dto!.CreatedAt.Should().BeCloseTo(before, TimeSpan.FromSeconds(5));
        dto.ExpiresAt.Should().BeAfter(before);
        dto.UsedAt.Should().BeNull();
        dto.RevokedAt.Should().BeNull();
    }

    #endregion

    #region RotateRefreshTokenAsync

    [Fact]
    public async Task RotateRefreshTokenAsync_ShouldCreateNewToken()
    {
        await using var context = CreateContext();
        var userId = await SeedUserAsync(context);
        var service = CreateService(context);
        var oldId = await service.CreateRefreshTokenAsync(userId, "old-hash", "old-jti");

        var newId = await service.RotateRefreshTokenAsync(
            oldId, "new-hash", "new-jti", 14, "Firefox", "10.0.0.1");

        newId.Should().NotBeEmpty().And.NotBe(oldId);
        var newToken = await context.RefreshTokens.FirstAsync(t => t.Id == newId);
        newToken.TokenHash.Should().Be("new-hash");
        newToken.JwtId.Should().Be("new-jti");
        newToken.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task RotateRefreshTokenAsync_ShouldMarkOldTokenAsUsed()
    {
        await using var context = CreateContext();
        var userId = await SeedUserAsync(context);
        var service = CreateService(context);
        var oldId = await service.CreateRefreshTokenAsync(userId, "old", "old-jti");

        var newId = await service.RotateRefreshTokenAsync(oldId, "new", "new-jti");

        var oldToken = await context.RefreshTokens.FirstAsync(t => t.Id == oldId);
        oldToken.UsedAt.Should().NotBeNull();
        oldToken.ReplacedByTokenId.Should().Be(newId);
    }

    [Fact]
    public async Task RotateRefreshTokenAsync_WhenOldTokenNotFound_ShouldThrow()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var act = () => service.RotateRefreshTokenAsync(
            Guid.NewGuid(), "new-hash", "new-jti");

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    #endregion

    #region RevokeTokenAsync

    [Fact]
    public async Task RevokeTokenAsync_WhenTokenExists_ShouldRevokeIt()
    {
        await using var context = CreateContext();
        var userId = await SeedUserAsync(context);
        var service = CreateService(context);
        var tokenId = await service.CreateRefreshTokenAsync(userId, "h", "j");

        await service.RevokeTokenAsync(tokenId, "test_reason");

        var token = await context.RefreshTokens.FirstAsync(t => t.Id == tokenId);
        token.RevokedAt.Should().NotBeNull();
        token.RevokedReason.Should().Be("test_reason");
    }

    [Fact]
    public async Task RevokeTokenAsync_WhenTokenDoesNotExist_ShouldNotThrow()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var act = () => service.RevokeTokenAsync(Guid.NewGuid());

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task RevokeTokenAsync_WithDefaultReason_ShouldUseManualRevocation()
    {
        await using var context = CreateContext();
        var userId = await SeedUserAsync(context);
        var service = CreateService(context);
        var tokenId = await service.CreateRefreshTokenAsync(userId, "h", "j");

        await service.RevokeTokenAsync(tokenId);

        var token = await context.RefreshTokens.FirstAsync(t => t.Id == tokenId);
        token.RevokedReason.Should().Be("manual_revocation");
    }

    #endregion

    #region RevokeAllUserTokensAsync

    [Fact]
    public async Task RevokeAllUserTokensAsync_ShouldRevokeOnlyTargetUserTokens()
    {
        await using var context = CreateContext();
        var userId = await SeedUserAsync(context);
        var service = CreateService(context);

        var otherUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "other@auraeyes.vn",
            NormalizedUserName = "OTHER@AURAEYES.VN",
            Email = "other@auraeyes.vn",
            NormalizedEmail = "OTHER@AURAEYES.VN",
            FullName = "Other User"
        };
        context.Users.Add(otherUser);
        await context.SaveChangesAsync();

        await service.CreateRefreshTokenAsync(userId, "h1", "j1");
        await service.CreateRefreshTokenAsync(userId, "h2", "j2");
        await service.CreateRefreshTokenAsync(otherUser.Id, "h3", "j3");

        await service.RevokeAllUserTokensAsync(userId, "logout_all");

        var userTokens = await context.RefreshTokens
            .Where(t => t.UserId == userId).ToListAsync();
        userTokens.Should().AllSatisfy(t =>
        {
            t.RevokedAt.Should().NotBeNull();
            t.RevokedReason.Should().Be("logout_all");
        });

        var otherToken = await context.RefreshTokens
            .FirstAsync(t => t.UserId == otherUser.Id);
        otherToken.RevokedAt.Should().BeNull();
    }

    [Fact]
    public async Task RevokeAllUserTokensAsync_ShouldSkipAlreadyRevokedTokens()
    {
        await using var context = CreateContext();
        var userId = await SeedUserAsync(context);
        var service = CreateService(context);

        var tokenId = await service.CreateRefreshTokenAsync(userId, "h1", "j1");
        await service.RevokeTokenAsync(tokenId, "earlier_reason");
        await service.CreateRefreshTokenAsync(userId, "h2", "j2");

        await service.RevokeAllUserTokensAsync(userId);

        var tokens = await context.RefreshTokens
            .Where(t => t.UserId == userId).ToListAsync();
        tokens.Should().HaveCount(2);
        tokens.First(t => t.TokenHash == "h1").RevokedReason.Should().Be("earlier_reason");
        tokens.First(t => t.TokenHash == "h2").RevokedReason.Should().Be("logout_all");
    }

    #endregion

    #region RevokeTokenFamilyAsync

    [Fact]
    public async Task RevokeTokenFamilyAsync_ShouldRevokeAllUserTokens()
    {
        await using var context = CreateContext();
        var userId = await SeedUserAsync(context);
        var service = CreateService(context);

        var t1 = await service.CreateRefreshTokenAsync(userId, "fam1", "j1");
        await service.CreateRefreshTokenAsync(userId, "fam2", "j2");

        await service.RevokeTokenFamilyAsync(t1, "token_reuse_detected");

        var tokens = await context.RefreshTokens
            .Where(t => t.UserId == userId).ToListAsync();
        tokens.Should().AllSatisfy(t => t.RevokedAt.Should().NotBeNull());
    }

    [Fact]
    public async Task RevokeTokenFamilyAsync_WhenTokenNotFound_ShouldNotThrow()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var act = () => service.RevokeTokenFamilyAsync(Guid.NewGuid());

        await act.Should().NotThrowAsync();
    }

    #endregion

    #region CleanupExpiredTokensAsync

    [Fact]
    public async Task CleanupExpiredTokensAsync_ShouldRemoveOldExpiredTokens()
    {
        await using var context = CreateContext();
        var userId = await SeedUserAsync(context);
        var service = CreateService(context);

        var expiredToken = RefreshToken.Create(userId, "exp-hash", "jti-exp", 0);
        typeof(RefreshToken).GetProperty(nameof(RefreshToken.ExpiresAt))!
            .SetValue(expiredToken, DateTime.UtcNow.AddDays(-60));
        context.RefreshTokens.Add(expiredToken);

        var activeToken = RefreshToken.Create(userId, "active-hash", "jti-active", 7);
        context.RefreshTokens.Add(activeToken);
        await context.SaveChangesAsync();

        var removed = await service.CleanupExpiredTokensAsync(daysToKeep: 30);

        removed.Should().Be(1);
        (await context.RefreshTokens.CountAsync()).Should().Be(1);
        (await context.RefreshTokens.FirstAsync()).TokenHash.Should().Be("active-hash");
    }

    [Fact]
    public async Task CleanupExpiredTokensAsync_ShouldRemoveOldRevokedTokens()
    {
        await using var context = CreateContext();
        var userId = await SeedUserAsync(context);
        var service = CreateService(context);

        var revokedToken = RefreshToken.Create(userId, "rev-hash", "jti-rev", 365);
        revokedToken.Revoke("old_revocation");
        typeof(RefreshToken).GetProperty(nameof(RefreshToken.RevokedAt))!
            .SetValue(revokedToken, DateTime.UtcNow.AddDays(-60));
        context.RefreshTokens.Add(revokedToken);
        await context.SaveChangesAsync();

        var removed = await service.CleanupExpiredTokensAsync(daysToKeep: 30);

        removed.Should().Be(1);
    }

    [Fact]
    public async Task CleanupExpiredTokensAsync_WithNoOldTokens_ShouldReturnZero()
    {
        await using var context = CreateContext();
        var userId = await SeedUserAsync(context);
        var service = CreateService(context);

        await service.CreateRefreshTokenAsync(userId, "fresh", "jti-fresh");

        var removed = await service.CleanupExpiredTokensAsync();

        removed.Should().Be(0);
    }

    #endregion
}
