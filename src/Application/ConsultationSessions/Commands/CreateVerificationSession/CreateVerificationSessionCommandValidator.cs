using FluentValidation;

namespace Application.ConsultationSessions.Commands.CreateVerificationSession;

public class CreateVerificationSessionCommandValidator
    : AbstractValidator<CreateVerificationSessionCommand>
{
    public CreateVerificationSessionCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient ID is required.");

        RuleFor(x => x.AiScreeningId)
            .NotEmpty().WithMessage("AI Screening ID is required.");

        RuleFor(x => x.PlatformFee)
            .GreaterThanOrEqualTo(0).WithMessage("PlatformFee cannot be negative.");
    }
}
