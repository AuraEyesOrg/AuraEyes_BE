using FluentValidation;

namespace Application.Organisations.Commands.DeleteOrganisation;

/// <summary>
/// Validator for DeleteOrganisationCommand.
/// </summary>
public class DeleteOrganisationCommandValidator : AbstractValidator<DeleteOrganisationCommand>
{
    public DeleteOrganisationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Organisation ID is required.");
    }
}
