using FluentValidation;

namespace Application.ClinicStaffs.Commands.UpdateClinicStaffProfile;

public class UpdateClinicStaffProfileCommandValidator : AbstractValidator<UpdateClinicStaffProfileCommand>
{
    private static readonly string[] ValidGenders = { "male", "female", "other", "prefernottotsay" };

    public UpdateClinicStaffProfileCommandValidator()
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

        RuleFor(x => x.CitizenId)
            .Matches("^[0-9]{12}$").WithMessage("Citizen ID must be 12 digits.")
            .When(x => x.CitizenId is not null);

        RuleFor(x => x.Department)
            .MaximumLength(100).WithMessage("Department must not exceed 100 characters.")
            .When(x => x.Department is not null);

        RuleFor(x => x.EmployeeCode)
            .MaximumLength(50).WithMessage("Employee code must not exceed 50 characters.")
            .When(x => x.EmployeeCode is not null);
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

