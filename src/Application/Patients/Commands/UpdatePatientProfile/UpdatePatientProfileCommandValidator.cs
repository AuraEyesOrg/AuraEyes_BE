using FluentValidation;

namespace Application.Patients.Commands.UpdatePatientProfile;

public class UpdatePatientProfileCommandValidator : AbstractValidator<UpdatePatientProfileCommand>
{
    private static readonly string[] ValidGenders = { "male", "female", "other", "prefernottotsay" };

    public UpdatePatientProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(200).WithMessage("Full name must not exceed 200 characters.");

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.")
            .When(x => x.Phone is not null);

        RuleFor(x => x.DateOfBirth)
            .Must(BeAValidDate).WithMessage("Invalid date of birth format.")
            .When(x => x.DateOfBirth is not null);

        RuleFor(x => x.Gender)
            .Must(BeAValidGender).WithMessage("Gender must be 'male', 'female', or 'other'.")
            .When(x => x.Gender is not null);

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters.")
            .When(x => x.Address is not null);
    }

    private static bool BeAValidDate(string? dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString)) return true;
        return DateTime.TryParse(dateString, out var date) && date < DateTime.UtcNow;
    }

    private static bool BeAValidGender(string? gender)
    {
        if (string.IsNullOrWhiteSpace(gender)) return true;
        return ValidGenders.Contains(gender.ToLowerInvariant());
    }
}
