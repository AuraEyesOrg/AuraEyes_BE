using FluentValidation;

namespace Application.Ophthalmologists.AvailableSlots.Commands.UpdateAvailableSlot;

/// <summary>
/// Validator for UpdateAvailableSlotCommand.
/// </summary>
public class UpdateAvailableSlotCommandValidator : AbstractValidator<UpdateAvailableSlotCommand>
{
    public UpdateAvailableSlotCommandValidator()
    {
        RuleFor(x => x.AvailableSlotId)
            .NotEmpty()
            .WithMessage("Available slot ID is required.");

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .WithMessage("Start time is required.");

        RuleFor(x => x.EndTime)
            .NotEmpty()
            .WithMessage("End time is required.")
            .GreaterThan(x => x.StartTime)
            .WithMessage("End time must be after start time.");

        RuleFor(x => x.MaxCapacity)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Max capacity must be at least 1.");
    }
}
