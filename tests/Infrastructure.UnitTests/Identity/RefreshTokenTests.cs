using FluentAssertions;
using Infrastructure.Identity;

namespace Infrastructure.UnitTests.Identity;

public class RefreshTokenTests
{
    [Fact]
    public void DefaultConstructor_ShouldSetIdAndCreatedAt()
    {
        var before = DateTime.UtcNow;
        var token = new RefreshToken();

        token.Id.Should().NotBeEmpty();
        token.CreatedAt.Should().BeOnOrAfter(before);
    }

    [Fact]
    public void Create_ShouldSetTokenDataAndExpiry()
    {
        var userId = Guid.NewGuid();
        var token = RefreshToken.Create(userId, "hash", "jti-1", 7, "device", "1.2.3.4");

        token.UserId.Should().Be(userId);
        token.TokenHash.Should().Be("hash");
        token.JwtId.Should().Be("jti-1");
        token.DeviceInfo.Should().Be("device");
        token.IpAddress.Should().Be("1.2.3.4");
        token.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void IsExpired_WhenPastExpiry_ShouldBeTrue()
    {
        var token = new RefreshToken
        {
            ExpiresAt = DateTime.UtcNow.AddDays(-1)
        };

        token.IsExpired.Should().BeTrue();
    }

    [Fact]
    public void IsActive_FreshToken_ShouldBeTrue()
    {
        var token = RefreshToken.Create(Guid.NewGuid(), "h", "j");

        token.IsActive.Should().BeTrue();
    }

    [Fact]
    public void MarkAsUsed_ShouldSetUsedAtAndReplacement()
    {
        var token = RefreshToken.Create(Guid.NewGuid(), "h", "j");
        var newId = Guid.NewGuid();

        token.MarkAsUsed(newId);

        token.UsedAt.Should().NotBeNull();
        token.ReplacedByTokenId.Should().Be(newId);
    }

    [Fact]
    public void Revoke_ShouldSetRevokedAtAndReason()
    {
        var token = RefreshToken.Create(Guid.NewGuid(), "h", "j");

        token.Revoke("logout");

        token.RevokedAt.Should().NotBeNull();
        token.RevokedReason.Should().Be("logout");
    }
}
