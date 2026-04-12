using Application.Scheduling.AppointmentSlots.Commands.GenerateSlots;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class GenerateSlotsCommandValidatorTests
{
    private readonly GenerateSlotsCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new GenerateSlotsCommand
        {
            ScheduleTemplateId = Guid.NewGuid(),
            FromDate = new DateOnly(2026, 5, 1),
            ToDate = new DateOnly(2026, 5, 31)
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyScheduleTemplateId_ShouldFail()
    {
        var command = new GenerateSlotsCommand
        {
            ScheduleTemplateId = Guid.Empty,
            FromDate = new DateOnly(2026, 5, 1),
            ToDate = new DateOnly(2026, 5, 31)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ScheduleTemplateId)
            .WithErrorMessage("Schedule template ID is required.");
    }

    [Fact]
    public void Validate_ToDateBeforeFromDate_ShouldFail()
    {
        var command = new GenerateSlotsCommand
        {
            ScheduleTemplateId = Guid.NewGuid(),
            FromDate = new DateOnly(2026, 6, 1),
            ToDate = new DateOnly(2026, 5, 1)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ToDate)
            .WithErrorMessage("To date must be greater than or equal to from date.");
    }

    [Fact]
    public void Validate_SameFromAndToDate_ShouldPass()
    {
        var command = new GenerateSlotsCommand
        {
            ScheduleTemplateId = Guid.NewGuid(),
            FromDate = new DateOnly(2026, 5, 15),
            ToDate = new DateOnly(2026, 5, 15)
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_DateRangeExceeds90Days_ShouldFail()
    {
        var command = new GenerateSlotsCommand
        {
            ScheduleTemplateId = Guid.NewGuid(),
            FromDate = new DateOnly(2026, 1, 1),
            ToDate = new DateOnly(2026, 4, 2) // 91 days
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveAnyValidationError()
            .WithErrorMessage("Date range cannot exceed 90 days.");
    }

    [Fact]
    public void Validate_DateRangeExactly90Days_ShouldPass()
    {
        var command = new GenerateSlotsCommand
        {
            ScheduleTemplateId = Guid.NewGuid(),
            FromDate = new DateOnly(2026, 1, 1),
            ToDate = new DateOnly(2026, 4, 1) // 90 days
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
