using Application.Scheduling.ScheduleTemplates.Commands.CreateScheduleTemplate;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class CreateScheduleTemplateCommandValidatorTests
{
    private readonly CreateScheduleTemplateCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommandWithOrgId_ShouldPass()
    {
        var command = new CreateScheduleTemplateCommand
        {
            OrgId = Guid.NewGuid(),
            OphthalId = null,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            SlotDuration = 30,
            MaxCapacity = 5,
            Cost = 200m
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ValidCommandWithOphthalId_ShouldPass()
    {
        var command = new CreateScheduleTemplateCommand
        {
            OrgId = null,
            OphthalId = Guid.NewGuid(),
            DayOfWeek = DayOfWeek.Friday,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(12, 0),
            SlotDuration = 15,
            MaxCapacity = 1
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_NeitherOrgIdNorOphthalId_ShouldFail()
    {
        var command = new CreateScheduleTemplateCommand
        {
            OrgId = null,
            OphthalId = null,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            SlotDuration = 30,
            MaxCapacity = 1
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveAnyValidationError()
            .WithErrorMessage("At least one of OrgId or OphthalId must be provided.");
    }

    [Fact]
    public void Validate_BothOrgIdAndOphthalId_ShouldFail()
    {
        var command = new CreateScheduleTemplateCommand
        {
            OrgId = Guid.NewGuid(),
            OphthalId = Guid.NewGuid(),
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            SlotDuration = 30,
            MaxCapacity = 1
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveAnyValidationError()
            .WithErrorMessage("OrgId must be null when OphthalId is provided.");
    }

    [Fact]
    public void Validate_InvalidDayOfWeek_ShouldFail()
    {
        var command = new CreateScheduleTemplateCommand
        {
            OrgId = Guid.NewGuid(),
            DayOfWeek = (DayOfWeek)99,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            SlotDuration = 30,
            MaxCapacity = 1
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DayOfWeek)
            .WithErrorMessage("Invalid day of week.");
    }

    [Fact]
    public void Validate_EndTimeBeforeStartTime_ShouldFail()
    {
        var command = new CreateScheduleTemplateCommand
        {
            OrgId = Guid.NewGuid(),
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(17, 0),
            EndTime = new TimeOnly(9, 0),
            SlotDuration = 30,
            MaxCapacity = 1
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EndTime)
            .WithErrorMessage("End time must be after start time.");
    }

    [Fact]
    public void Validate_EndTimeEqualToStartTime_ShouldFail()
    {
        var command = new CreateScheduleTemplateCommand
        {
            OrgId = Guid.NewGuid(),
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(9, 0),
            SlotDuration = 30,
            MaxCapacity = 1
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EndTime)
            .WithErrorMessage("End time must be after start time.");
    }

    [Fact]
    public void Validate_SlotDurationZero_ShouldFail()
    {
        var command = new CreateScheduleTemplateCommand
        {
            OrgId = Guid.NewGuid(),
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            SlotDuration = 0,
            MaxCapacity = 1
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SlotDuration)
            .WithErrorMessage("Slot duration must be at least 1 minute.");
    }

    [Fact]
    public void Validate_MaxCapacityZero_ShouldFail()
    {
        var command = new CreateScheduleTemplateCommand
        {
            OrgId = Guid.NewGuid(),
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            SlotDuration = 30,
            MaxCapacity = 0
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MaxCapacity)
            .WithErrorMessage("Max capacity must be at least 1.");
    }

    [Fact]
    public void Validate_NegativeCost_ShouldFail()
    {
        var command = new CreateScheduleTemplateCommand
        {
            OrgId = Guid.NewGuid(),
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            SlotDuration = 30,
            MaxCapacity = 1,
            Cost = -1m
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Cost)
            .WithErrorMessage("Cost cannot be negative.");
    }

    [Fact]
    public void Validate_ZeroCost_ShouldPass()
    {
        var command = new CreateScheduleTemplateCommand
        {
            OrgId = Guid.NewGuid(),
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            SlotDuration = 30,
            MaxCapacity = 1,
            Cost = 0m
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Cost);
    }

    [Fact]
    public void Validate_NullCost_ShouldPass()
    {
        var command = new CreateScheduleTemplateCommand
        {
            OrgId = Guid.NewGuid(),
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            SlotDuration = 30,
            MaxCapacity = 1,
            Cost = null
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Cost);
    }
}
