using FluentValidation;

namespace Application.OrganisationPatients.Commands.CreateWalkInPatient;

public class CreateWalkInPatientCommandValidator : AbstractValidator<CreateWalkInPatientCommand>
{
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
            .LessThan(DateTime.UtcNow).WithMessage("Date of Birth cannot be in the future");
            
        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Invalid email format");
    }
}
