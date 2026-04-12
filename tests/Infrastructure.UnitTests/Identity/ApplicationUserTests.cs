using FluentAssertions;
using Infrastructure.Identity;

namespace Infrastructure.UnitTests.Identity;

public class ApplicationUserTests
{
    [Fact]
    public void DefaultConstructor_ShouldSetDefaultStatusAndCreatedAt()
    {
        var before = DateTime.UtcNow;
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "u@test.com",
            Email = "u@test.com"
        };

        user.IsActive.Should().BeTrue();
        user.IsDeleted.Should().BeFalse();
        user.CreatedAt.Should().BeOnOrAfter(before);
    }

    [Fact]
    public void Deactivate_ShouldSetInactiveAndUpdatedAt()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "a", Email = "a@b.c" };

        user.Deactivate();

        user.IsActive.Should().BeFalse();
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Activate_ShouldSetActiveAndUpdatedAt()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "a", Email = "a@b.c" };
        user.Deactivate();

        user.Activate();

        user.IsActive.Should().BeTrue();
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void SoftDelete_ShouldSetDeletedFlagsAndTimestamps()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "a", Email = "a@b.c" };

        user.SoftDelete();

        user.IsDeleted.Should().BeTrue();
        user.IsActive.Should().BeFalse();
        user.DeletedAt.Should().NotBeNull();
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void UpdateLastLogin_ShouldSetLastLoginAt()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "a", Email = "a@b.c" };

        user.UpdateLastLogin();

        user.LastLoginAt.Should().NotBeNull();
    }
}
