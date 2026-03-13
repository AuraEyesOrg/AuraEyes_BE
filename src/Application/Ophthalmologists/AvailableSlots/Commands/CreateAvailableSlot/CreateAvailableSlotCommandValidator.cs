using FluentValidation;

namespace Application.Ophthalmologists.AvailableSlots.Commands.CreateAvailableSlot;

/// <summary>
/// Validator for CreateAvailableSlotCommand.
/// </summary>
public class CreateAvailableSlotCommandValidator : AbstractValidator<CreateAvailableSlotCommand>
{
    public CreateAvailableSlotCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => x.OrganisationId.HasValue || x.OphthalmologistId.HasValue)
            .WithMessage("At least one of OrganisationId or OphthalmologistId must be provided.");

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .WithMessage("Start time is required.")
            .Must(startTime => startTime > DateTime.UtcNow)
            .WithMessage("Start time must be in the future.");

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
