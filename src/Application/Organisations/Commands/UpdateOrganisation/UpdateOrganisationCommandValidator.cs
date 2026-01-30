using FluentValidation;

namespace Application.Organisations.Commands.UpdateOrganisation;

/// <summary>
/// Validator for UpdateOrganisationCommand.
/// </summary>
public class UpdateOrganisationCommandValidator : AbstractValidator<UpdateOrganisationCommand>
{
    public UpdateOrganisationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Organisation ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Organisation name is required.")
            .MaximumLength(200)
            .WithMessage("Organisation name cannot exceed 200 characters.");

        RuleFor(x => x.Address)
            .MaximumLength(500)
            .WithMessage("Address cannot exceed 500 characters.");

        RuleFor(x => x.LicenseNumber)
            .MaximumLength(100)
            .WithMessage("License number cannot exceed 100 characters.");

        RuleFor(x => x.OrgType)
            .IsInEnum()
            .WithMessage("Invalid organisation type.");
    }
}
