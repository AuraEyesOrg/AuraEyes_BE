using FluentValidation;

namespace Application.OrganisationScreenings.Commands.CreateOrgScreeningSession;

public class CreateOrgScreeningSessionCommandValidator : AbstractValidator<CreateOrgScreeningSessionCommand>
{
    public CreateOrgScreeningSessionCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("PatientId is required.");

        RuleFor(x => x.RetinalImages)
            .NotEmpty().WithMessage("At least one retinal image is required.")
            .Must(imgs => imgs.Count <= 10).WithMessage("Maximum 10 images allowed per session.");
    }
}
