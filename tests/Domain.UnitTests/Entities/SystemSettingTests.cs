using Domain.Entities.Platform;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class SystemSettingTests
{
    #region Constructor

    [Fact]
    public void Constructor_ValidInput_ShouldCreateSetting()
    {
        var setting = new SystemSetting("FREE_AI_QUOTA", "10", "Daily free quota");

        setting.Key.Should().Be("FREE_AI_QUOTA");
        setting.Value.Should().Be("10");
        setting.Description.Should().Be("Daily free quota");
    }

    [Fact]
    public void Constructor_WithoutDescription_ShouldHaveNullDescription()
    {
        var setting = new SystemSetting("COMMISSION_RATE", "0.05");

        setting.Description.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyKey_ShouldThrow(string? key)
    {
        var act = () => new SystemSetting(key!, "value");

        act.Should().Throw<ArgumentException>().WithParameterName("key");
    }

    #endregion

    #region UpdateValue

    [Fact]
    public void UpdateValue_ShouldChangeValue()
    {
        var setting = new SystemSetting("FREE_AI_QUOTA", "10");

        setting.UpdateValue("20");

        setting.Value.Should().Be("20");
    }

    [Fact]
    public void UpdateValue_ToEmptyString_ShouldAllowIt()
    {
        var setting = new SystemSetting("SOME_KEY", "old");

        setting.UpdateValue("");

        setting.Value.Should().BeEmpty();
    }

    #endregion
}
