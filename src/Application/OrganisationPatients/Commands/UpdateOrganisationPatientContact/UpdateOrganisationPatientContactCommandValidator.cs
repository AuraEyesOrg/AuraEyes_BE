using FluentValidation;

namespace Application.OrganisationPatients.Commands.UpdateOrganisationPatientContact;

public class UpdateOrganisationPatientContactCommandValidator : AbstractValidator<UpdateOrganisationPatientContactCommand>
{
    public UpdateOrganisationPatientContactCommandValidator()
    {
        RuleFor(x => x.OrgAdminUserId)
            .NotEmpty().WithMessage("Org admin user ID is required");

        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient ID is required");

        // Walk-in admin fields
        RuleFor(x => x.FullName)
            .MaximumLength(200).When(x => x.FullName is not null)
            .WithMessage("Full Name must not exceed 200 characters");

        RuleFor(x => x.Gender)
            .Must(g => new[] { "Male", "Female", "Other", "M", "F", "O" }.Contains(g))
            .When(x => !string.IsNullOrWhiteSpace(x.Gender))
            .WithMessage("Gender must be 'Male', 'Female', or 'Other'");

        RuleFor(x => x.CitizenId)
            .MaximumLength(20).When(x => x.CitizenId is not null)
            .WithMessage("Citizen ID must not exceed 20 characters");

        // Shared fields
        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("Phone number must not exceed 20 characters");

        RuleFor(x => x.Address)
            .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Address))
            .WithMessage("Address must not exceed 500 characters");

        RuleFor(x => x.Bmi)
            .InclusiveBetween(10m, 100m).When(x => x.Bmi.HasValue)
            .WithMessage("BMI must be between 10 and 100");

        RuleFor(x => x.DiseaseHistory)
            .MaximumLength(1000).When(x => x.DiseaseHistory is not null)
            .WithMessage("Disease history must not exceed 1000 characters");

        RuleFor(x => x)
            .Must(HaveAtLeastOneField)
            .WithMessage("At least one field must be provided for update");
    }

    private static bool HaveAtLeastOneField(UpdateOrganisationPatientContactCommand cmd)
    {
        return cmd.FullName is not null
               || cmd.DateOfBirth is not null
               || cmd.Gender is not null
               || cmd.CitizenId is not null
               || cmd.PhoneNumber is not null
               || cmd.Address is not null
               || cmd.Bmi is not null
               || cmd.DiseaseHistory is not null;
    }
}