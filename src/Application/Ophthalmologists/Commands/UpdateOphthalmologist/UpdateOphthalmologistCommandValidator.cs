using FluentValidation;

namespace Application.Ophthalmologists.Commands.UpdateOphthalmologist;

/// <summary>
/// Validator for UpdateOphthalmologistCommand.
/// </summary>
public class UpdateOphthalmologistCommandValidator : AbstractValidator<UpdateOphthalmologistCommand>
{
    public UpdateOphthalmologistCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Ophthalmologist ID is required.");

        RuleFor(x => x.YearsOfExperience)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Years of experience cannot be negative.")
            .LessThanOrEqualTo(70)
            .WithMessage("Years of experience cannot exceed 70 years.");

        RuleFor(x => x.Bio)
            .MaximumLength(2000)
            .WithMessage("Bio cannot exceed 2000 characters.");
    }
}
