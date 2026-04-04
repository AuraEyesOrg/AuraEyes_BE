using Domain.Entities.Scheduling;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class ScheduleTemplateTests
{
    [Fact]
    public void Constructor_WithOrgId_ShouldCreateTemplate()
    {
        var template = new ScheduleTemplate(
            DayOfWeek.Monday,
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            30,
            2,
            orgId: Guid.NewGuid());

        template.DayOfWeek.Should().Be(DayOfWeek.Monday);
        template.SlotDuration.Should().Be(30);
        template.MaxCapacity.Should().Be(2);
        template.OrgId.Should().NotBeNull();
        template.OphthalId.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithoutOwner_ShouldThrow()
    {
        var act = () => new ScheduleTemplate(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(17, 0), 30, 2);

        act.Should().Throw<ArgumentException>()
            .WithMessage("At least one of OrgId or OphthalId must be provided");
    }

    [Fact]
    public void Constructor_WithBothOwners_ShouldThrow()
    {
        var act = () => new ScheduleTemplate(
            DayOfWeek.Monday,
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            30,
            2,
            Guid.NewGuid(),
            Guid.NewGuid());

        act.Should().Throw<ArgumentException>()
            .WithMessage("OrgId must be null when OphthalId is provided.");
    }

    [Fact]
    public void Update_ValidInput_ShouldUpdateFields()
    {
        var template = new ScheduleTemplate(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(17, 0), 30, 2, orgId: Guid.NewGuid());

        template.Update(DayOfWeek.Friday, new TimeOnly(8, 0), new TimeOnly(16, 0), 20, 3, 120_000m);

        template.DayOfWeek.Should().Be(DayOfWeek.Friday);
        template.SlotDuration.Should().Be(20);
        template.MaxCapacity.Should().Be(3);
        template.Cost.Should().Be(120_000m);
    }
}