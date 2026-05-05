using FluentValidation;

namespace Application.Patients.Commands.CreateWalkInPatient;

public class CreateWalkInPatientCommandValidator : AbstractValidator<CreateWalkInPatientCommand>
{
    public CreateWalkInPatientCommandValidator()
    {
        RuleFor(v => v.FullName)
            .NotEmpty().WithMessage("Full Name is required.")
            .MaximumLength(200).WithMessage("Full Name must not exceed 200 characters.");

        RuleFor(v => v.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(v => v.CitizenId)
            .Must(id => string.IsNullOrWhiteSpace(id) || (id.Length == 12 && id.All(char.IsDigit)))
            .WithMessage("Citizen ID must be exactly 12 digits.");

        RuleFor(v => v.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.");

        RuleFor(v => v.Address)
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters.");
    }
}
