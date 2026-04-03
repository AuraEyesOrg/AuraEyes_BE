using FluentValidation;

namespace Application.OrganisationPatients.Commands.CreateWalkInPatient;

public class CreateWalkInPatientCommandValidator : AbstractValidator<CreateWalkInPatientCommand>
{
    private const int MinimumAgeYears = 16;

    public CreateWalkInPatientCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full Name is required")
            .MaximumLength(100).WithMessage("Full Name must not exceed 100 characters");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is required")
            .Must(g => new[] { "Male", "Female", "Other" }.Contains(g) || new[] { "M", "F", "O" }.Contains(g))
            .WithMessage("Gender must be 'Male', 'Female', or 'Other'");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of Birth is required")
            .LessThan(DateTime.UtcNow.Date).WithMessage("Date of Birth cannot be in the future")
            .Must(BeAtLeastMinimumAge).WithMessage($"Patient must be at least {MinimumAgeYears} years old");
            
        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Invalid email format");
    }

    private static bool BeAtLeastMinimumAge(DateTime dateOfBirth)
    {
        var today = DateTime.UtcNow.Date;
        return dateOfBirth.Date <= today.AddYears(-MinimumAgeYears);
    }
}
