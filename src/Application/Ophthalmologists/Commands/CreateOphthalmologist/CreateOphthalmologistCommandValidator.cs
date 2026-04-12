using FluentValidation;

namespace Application.Ophthalmologists.Commands.CreateOphthalmologist;

/// <summary>
/// Validator for CreateOphthalmologistCommand.
/// </summary>
public class CreateOphthalmologistCommandValidator : AbstractValidator<CreateOphthalmologistCommand>
{
    public CreateOphthalmologistCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");

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
