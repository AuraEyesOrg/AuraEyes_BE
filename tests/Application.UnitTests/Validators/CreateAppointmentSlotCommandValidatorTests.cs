using Application.Scheduling.AppointmentSlots.Commands.CreateAppointmentSlot;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class CreateAppointmentSlotCommandValidatorTests
{
    private readonly CreateAppointmentSlotCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new CreateAppointmentSlotCommand
        {
            ScheduleTemplateId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(9, 30),
            Cost = 100m
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyScheduleTemplateId_ShouldFail()
    {
        var command = new CreateAppointmentSlotCommand
        {
            ScheduleTemplateId = Guid.Empty,
            Date = DateOnly.FromDateTime(DateTime.Today),
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(9, 30)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ScheduleTemplateId)
            .WithErrorMessage("Schedule template ID is required.");
    }

    [Fact]
    public void Validate_EndTimeBeforeStartTime_ShouldFail()
    {
        var command = new CreateAppointmentSlotCommand
        {
            ScheduleTemplateId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.Today),
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(9, 0)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EndTime)
            .WithErrorMessage("End time must be after start time.");
    }

    [Fact]
    public void Validate_EndTimeEqualToStartTime_ShouldFail()
    {
        var command = new CreateAppointmentSlotCommand
        {
            ScheduleTemplateId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.Today),
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(10, 0)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EndTime)
            .WithErrorMessage("End time must be after start time.");
    }

    [Fact]
    public void Validate_NegativeCost_ShouldFail()
    {
        var command = new CreateAppointmentSlotCommand
        {
            ScheduleTemplateId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.Today),
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(9, 30),
            Cost = -1m
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Cost)
            .WithErrorMessage("Cost cannot be negative.");
    }

    [Fact]
    public void Validate_ZeroCost_ShouldPass()
    {
        var command = new CreateAppointmentSlotCommand
        {
            ScheduleTemplateId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.Today),
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(9, 30),
            Cost = 0m
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Cost);
    }

    [Fact]
    public void Validate_NullCost_ShouldPass()
    {
        var command = new CreateAppointmentSlotCommand
        {
            ScheduleTemplateId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.Today),
            StartTime = new TimeOnly(9, 0),
            EndTime = new TimeOnly(9, 30),
            Cost = null
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Cost);
    }
}
