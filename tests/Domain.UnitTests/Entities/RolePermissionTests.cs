using Domain.Entities.Authorization;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class RolePermissionTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();

        var rp = new RolePermission(roleId, permissionId);

        rp.RoleId.Should().Be(roleId);
        rp.PermissionId.Should().Be(permissionId);
        rp.Id.Should().NotBeEmpty();
    }
}
