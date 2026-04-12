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

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("Phone number must not exceed 20 characters");

        RuleFor(x => x.Address)
            .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Address))
            .WithMessage("Address must not exceed 500 characters");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Invalid email format");

        RuleFor(x => x)
            .Must(HaveAtLeastOneField)
            .WithMessage("At least one field must be provided for update");
    }

    private static bool HaveAtLeastOneField(UpdateOrganisationPatientContactCommand command)
    {
        return command.PhoneNumber is not null
               || command.Email is not null
               || command.Address is not null;
    }
}