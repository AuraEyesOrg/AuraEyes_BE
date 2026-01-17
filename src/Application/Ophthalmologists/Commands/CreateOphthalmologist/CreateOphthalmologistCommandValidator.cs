using FluentValidation;

namespace Application.Ophthalmologists.Commands.CreateOphthalmologist;

public class CreateOphthalmologistCommandValidator : AbstractValidator<CreateOphthalmologistCommand>
{
    public CreateOphthalmologistCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(x => x.Bio)
            .MaximumLength(2000).WithMessage("Bio must not exceed 2000 characters");

        RuleFor(x => x.YearsOfExperience)
            .GreaterThanOrEqualTo(0).WithMessage("Years of experience cannot be negative")
            .LessThanOrEqualTo(100).WithMessage("Years of experience must not exceed 100");
    }
}
