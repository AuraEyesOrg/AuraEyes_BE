using FluentValidation;

namespace Application.Ophthalmologists.Schedules.Commands.CreateSchedule;

/// <summary>
/// Validator for CreateScheduleCommand.
/// </summary>
public class CreateScheduleCommandValidator : AbstractValidator<CreateScheduleCommand>
{
    public CreateScheduleCommandValidator()
    {
        RuleFor(x => x.OphthalmologistId)
            .NotEmpty()
            .WithMessage("Ophthalmologist ID is required.");

        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date is required.")
            .Must(date => date >= DateOnly.FromDateTime(DateTime.UtcNow.Date))
            .WithMessage("Date cannot be in the past.");

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .WithMessage("Start time is required.");

        RuleFor(x => x.EndTime)
            .NotEmpty()
            .WithMessage("End time is required.")
            .GreaterThan(x => x.StartTime)
            .WithMessage("End time must be after start time.");

        RuleFor(x => x.SlotType)
            .IsInEnum()
            .WithMessage("Invalid slot type.");

        RuleFor(x => x.Cost)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Cost.HasValue)
            .WithMessage("Cost cannot be negative.");
    }
}
