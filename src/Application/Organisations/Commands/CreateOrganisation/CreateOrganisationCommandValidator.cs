using Domain.Enums;
using FluentValidation;

namespace Application.Organisations.Commands.CreateOrganisation;

/// <summary>
/// Validator for CreateOrganisationCommand.
/// </summary>
public class CreateOrganisationCommandValidator : AbstractValidator<CreateOrganisationCommand>
{
    public CreateOrganisationCommandValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty()
            .WithMessage("OwnerId is required.");

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
