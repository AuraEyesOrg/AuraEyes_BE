using FluentValidation;

namespace Application.Ophthalmologists.Commands.UpdateOphthalmologist;

public class UpdateOphthalmologistCommandValidator : AbstractValidator<UpdateOphthalmologistCommand>
{
    public UpdateOphthalmologistCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Ophthalmologist ID is required");

        RuleFor(x => x.Bio)
            .MaximumLength(2000).WithMessage("Bio must not exceed 2000 characters");

        RuleFor(x => x.YearsOfExperience)
            .GreaterThanOrEqualTo(0).WithMessage("Years of experience cannot be negative")
            .LessThanOrEqualTo(100).WithMessage("Years of experience must not exceed 100");
    }
}
