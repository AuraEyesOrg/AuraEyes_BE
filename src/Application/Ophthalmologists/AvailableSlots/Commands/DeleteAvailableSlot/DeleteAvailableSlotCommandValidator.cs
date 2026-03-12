using FluentValidation;

namespace Application.Ophthalmologists.AvailableSlots.Commands.DeleteAvailableSlot;

/// <summary>
/// Validator for DeleteAvailableSlotCommand.
/// </summary>
public class DeleteAvailableSlotCommandValidator : AbstractValidator<DeleteAvailableSlotCommand>
{
    public DeleteAvailableSlotCommandValidator()
    {
        RuleFor(x => x.AvailableSlotId)
            .NotEmpty()
            .WithMessage("Available slot ID is required.");
    }
}
