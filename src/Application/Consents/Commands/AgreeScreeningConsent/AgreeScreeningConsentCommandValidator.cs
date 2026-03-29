using FluentValidation;

namespace Application.Consents.Commands.AgreeScreeningConsent;

public class AgreeScreeningConsentCommandValidator : AbstractValidator<AgreeScreeningConsentCommand>
{
    public AgreeScreeningConsentCommandValidator()
    {
        RuleFor(x => x.ScreeningId)
            .NotEmpty()
            .WithMessage("ScreeningId is required.");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Consent content is required.")
            .MaximumLength(4000)
            .WithMessage("Consent content must not exceed 4000 characters.");
    }
}
