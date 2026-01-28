using FluentValidation;

namespace Application.Ophthalmologists.Commands.DeleteOphthalmologist;

/// <summary>
/// Validator for DeleteOphthalmologistCommand.
/// </summary>
public class DeleteOphthalmologistCommandValidator : AbstractValidator<DeleteOphthalmologistCommand>
{
    public DeleteOphthalmologistCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Ophthalmologist ID is required.");
    }
}
