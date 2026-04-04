using Application.Scheduling.ScheduleTemplates.Commands.UpdateScheduleTemplate;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class UpdateScheduleTemplateCommandValidatorTests
{
    private readonly UpdateScheduleTemplateCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new UpdateScheduleTemplateCommand
        {
            ScheduleTemplateId = Guid.NewGuid(),
            DayOfWeek = DayOfWeek.Wednesday,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(16, 0),
            SlotDuration = 20,
            MaxCapacity = 3,
            Cost = 150m
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyScheduleTemplateId_ShouldFail()
    {
        var command = new UpdateScheduleTemplateCommand
        {
            ScheduleTemplateId = Guid.Empty,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            SlotDuration = 30,
            MaxCapacity = 1
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ScheduleTemplateId)
            .WithErrorMessage("Schedule template ID is required.");
    }

    [Fact]
    public void Validate_InvalidDayOfWeek_ShouldFail()
    {
        var command = new UpdateScheduleTemplateCommand
        {
            ScheduleTemplateId = Guid.NewGuid(),
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
    public void Validate_StartTimeAfterEndTime_ShouldFail()
    {
        var command = new UpdateScheduleTemplateCommand
        {
            ScheduleTemplateId = Guid.NewGuid(),
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(17, 0),
            EndTime = new TimeOnly(9, 0),
            SlotDuration = 30,
            MaxCapacity = 1
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StartTime)
            .WithErrorMessage("Start time must be before end time.");
    }

    [Fact]
    public void Validate_EndTimeBeforeStartTime_ShouldFail()
    {
        var command = new UpdateScheduleTemplateCommand
        {
            ScheduleTemplateId = Guid.NewGuid(),
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
    public void Validate_SlotDurationZero_ShouldFail()
    {
        var command = new UpdateScheduleTemplateCommand
        {
            ScheduleTemplateId = Guid.NewGuid(),
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
    public void Validate_SlotDurationOne_ShouldPass()
    {
        var command = new UpdateScheduleTemplateCommand
        {
            ScheduleTemplateId = Guid.NewGuid(),
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            SlotDuration = 1,
            MaxCapacity = 1
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.SlotDuration);
    }

    [Fact]
    public void Validate_MaxCapacityZero_ShouldFail()
    {
        var command = new UpdateScheduleTemplateCommand
        {
            ScheduleTemplateId = Guid.NewGuid(),
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
        var command = new UpdateScheduleTemplateCommand
        {
            ScheduleTemplateId = Guid.NewGuid(),
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(17, 0),
            SlotDuration = 30,
            MaxCapacity = 1,
            Cost = -5m
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Cost)
            .WithErrorMessage("Cost cannot be negative.");
    }

    [Fact]
    public void Validate_NullCost_ShouldPass()
    {
        var command = new UpdateScheduleTemplateCommand
        {
            ScheduleTemplateId = Guid.NewGuid(),
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
