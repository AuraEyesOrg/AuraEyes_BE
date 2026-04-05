using Domain.Entities.Authorization;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class UserPermissionTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _permissionId = Guid.NewGuid();

    #region Constructor

    [Fact]
    public void Constructor_ValidInput_ShouldCreateUserPermission()
    {
        var grantedBy = Guid.NewGuid();
        var expires = DateTime.UtcNow.AddDays(30);

        var up = new UserPermission(_userId, _permissionId, true, grantedBy, expires);

        up.UserId.Should().Be(_userId);
        up.PermissionId.Should().Be(_permissionId);
        up.IsGranted.Should().BeTrue();
        up.GrantedBy.Should().Be(grantedBy);
        up.ExpiresAt.Should().Be(expires);
        up.IsActive.Should().BeTrue();
        up.GrantedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Constructor_Defaults_ShouldGrantWithoutExpiry()
    {
        var up = new UserPermission(_userId, _permissionId);

        up.IsGranted.Should().BeTrue();
        up.GrantedBy.Should().BeNull();
        up.ExpiresAt.Should().BeNull();
        up.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Constructor_RevokeGrant_ShouldSetIsGrantedFalse()
    {
        var up = new UserPermission(_userId, _permissionId, isGranted: false);

        up.IsGranted.Should().BeFalse();
    }

    #endregion

    #region Revoke

    [Fact]
    public void Revoke_ShouldSetIsActiveFalse()
    {
        var up = new UserPermission(_userId, _permissionId);

        up.Revoke();

        up.IsActive.Should().BeFalse();
        up.UpdatedAt.Should().NotBeNull();
    }

    #endregion

    #region Extend

    [Fact]
    public void Extend_FutureDate_ShouldUpdateExpiresAt()
    {
        var up = new UserPermission(_userId, _permissionId);
        var newExpiry = DateTime.UtcNow.AddDays(60);

        up.Extend(newExpiry);

        up.ExpiresAt.Should().Be(newExpiry);
        up.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Extend_PastDate_ShouldThrow()
    {
        var up = new UserPermission(_userId, _permissionId);
        var pastDate = DateTime.UtcNow.AddDays(-1);

        var act = () => up.Extend(pastDate);

        act.Should().Throw<ArgumentException>().WithParameterName("newExpiryDate");
    }

    #endregion

    #region IsExpired

    [Fact]
    public void IsExpired_NoExpiryDate_ShouldReturnFalse()
    {
        var up = new UserPermission(_userId, _permissionId);

        up.IsExpired.Should().BeFalse();
    }

    [Fact]
    public void IsExpired_FutureExpiry_ShouldReturnFalse()
    {
        var up = new UserPermission(_userId, _permissionId, expiresAt: DateTime.UtcNow.AddDays(30));

        up.IsExpired.Should().BeFalse();
    }

    [Fact]
    public void IsExpired_PastExpiry_ShouldReturnTrue()
    {
        var up = new UserPermission(_userId, _permissionId, expiresAt: DateTime.UtcNow.AddDays(-1));

        up.IsExpired.Should().BeTrue();
    }

    #endregion
}
