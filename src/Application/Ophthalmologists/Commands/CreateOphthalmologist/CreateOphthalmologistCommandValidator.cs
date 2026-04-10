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

        RuleFor(x => x.EmploymentType)
            .IsInEnum()
            .WithMessage("Employment type is invalid.");

        RuleFor(x => x.WorkingHoursPerWeek)
            .InclusiveBetween(1, 112)
            .When(x => x.WorkingHoursPerWeek.HasValue)
            .WithMessage("Working hours per week must be between 1 and 112.");

        RuleFor(x => x.ExpectedMonthlySalary)
            .GreaterThanOrEqualTo(0)
            .When(x => x.ExpectedMonthlySalary.HasValue)
            .WithMessage("Expected monthly salary cannot be negative.");
    }
}
