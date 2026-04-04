using Domain.Entities.Platform;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class AuditLogTests
{
    #region Constructor

    [Fact]
    public void Constructor_ValidInput_ShouldCreateAuditLog()
    {
        var userId = Guid.NewGuid();

        var log = new AuditLog(
            "Update",
            "Patient",
            "patient-123",
            userId,
            "{\"name\":\"old\"}",
            "{\"name\":\"new\"}",
            "192.168.1.1");

        log.Action.Should().Be("Update");
        log.EntityName.Should().Be("Patient");
        log.EntityId.Should().Be("patient-123");
        log.UserId.Should().Be(userId);
        log.OldValue.Should().Be("{\"name\":\"old\"}");
        log.NewValue.Should().Be("{\"name\":\"new\"}");
        log.IpAddress.Should().Be("192.168.1.1");
    }

    [Fact]
    public void Constructor_MinimalInput_ShouldUseNullDefaults()
    {
        var log = new AuditLog("Create", "Organisation");

        log.EntityId.Should().BeNull();
        log.UserId.Should().BeNull();
        log.OldValue.Should().BeNull();
        log.NewValue.Should().BeNull();
        log.IpAddress.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyAction_ShouldThrow(string? action)
    {
        var act = () => new AuditLog(action!, "Entity");

        act.Should().Throw<ArgumentException>().WithParameterName("action");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyEntityName_ShouldThrow(string? entityName)
    {
        var act = () => new AuditLog("Create", entityName!);

        act.Should().Throw<ArgumentException>().WithParameterName("entityName");
    }

    #endregion
}
