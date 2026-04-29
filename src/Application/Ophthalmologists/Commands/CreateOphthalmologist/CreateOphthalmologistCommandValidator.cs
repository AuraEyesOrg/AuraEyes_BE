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

        RuleFor(x => x.Bio)
            .MaximumLength(2000)
            .WithMessage("Bio cannot exceed 2000 characters.");

        RuleFor(x => x.EmploymentType)
            .IsInEnum()
            .WithMessage("Employment type is invalid.");
    }
}
