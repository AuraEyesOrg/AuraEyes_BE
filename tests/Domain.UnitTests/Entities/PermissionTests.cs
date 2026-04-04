using Domain.Entities.Authorization;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class PermissionTests
{
    #region Constructor

    [Fact]
    public void Constructor_ValidInput_ShouldCreatePermission()
    {
        var permission = new Permission("screening.create", "Create Screening", "Allow creating screenings", "Screening");

        permission.Name.Should().Be("screening.create");
        permission.DisplayName.Should().Be("Create Screening");
        permission.Description.Should().Be("Allow creating screenings");
        permission.Category.Should().Be("Screening");
        permission.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Constructor_MinimalInput_ShouldUseNullDefaults()
    {
        var permission = new Permission("user.read", "Read Users");

        permission.Description.Should().BeNull();
        permission.Category.Should().BeNull();
        permission.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyName_ShouldThrow(string? name)
    {
        var act = () => new Permission(name!, "Display");

        act.Should().Throw<ArgumentException>().WithParameterName("name");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyDisplayName_ShouldThrow(string? displayName)
    {
        var act = () => new Permission("perm.name", displayName!);

        act.Should().Throw<ArgumentException>().WithParameterName("displayName");
    }

    #endregion

    #region Update

    [Fact]
    public void Update_ValidInput_ShouldUpdateFields()
    {
        var permission = new Permission("perm.name", "Old Display", "Old Desc", "OldCat");

        permission.Update("New Display", "New Desc", "NewCat");

        permission.DisplayName.Should().Be("New Display");
        permission.Description.Should().Be("New Desc");
        permission.Category.Should().Be("NewCat");
        permission.UpdatedAt.Should().NotBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_EmptyDisplayName_ShouldThrow(string? displayName)
    {
        var permission = new Permission("perm.name", "Display");

        var act = () => permission.Update(displayName!, null, null);

        act.Should().Throw<ArgumentException>().WithParameterName("displayName");
    }

    #endregion

    #region Activate / Deactivate

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var permission = new Permission("perm.name", "Display");

        permission.Deactivate();

        permission.IsActive.Should().BeFalse();
        permission.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Activate_AfterDeactivate_ShouldSetIsActiveTrue()
    {
        var permission = new Permission("perm.name", "Display");
        permission.Deactivate();

        permission.Activate();

        permission.IsActive.Should().BeTrue();
        permission.UpdatedAt.Should().NotBeNull();
    }

    #endregion
}
