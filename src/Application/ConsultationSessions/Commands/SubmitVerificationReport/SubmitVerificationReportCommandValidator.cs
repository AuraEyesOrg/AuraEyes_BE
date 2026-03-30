using FluentValidation;

namespace Application.ConsultationSessions.Commands.SubmitVerificationReport;

public class SubmitVerificationReportCommandValidator
    : AbstractValidator<SubmitVerificationReportCommand>
{
    public SubmitVerificationReportCommandValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("Session ID is required.");

        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("Doctor ID is required.");

        RuleFor(x => x)
            .Must(x =>
                !string.IsNullOrWhiteSpace(x.DiagnosisCode) ||
                !string.IsNullOrWhiteSpace(x.DiagnosesCode))
            .WithMessage("Diagnosis code is required.");

        RuleFor(x => x)
            .Must(x =>
                !string.IsNullOrWhiteSpace(x.ClinicalFindings) ||
                !string.IsNullOrWhiteSpace(x.DiagnosesText))
            .WithMessage("Clinical findings are required.");

        RuleFor(x => x.DiagnosisCode)
            .MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.DiagnosisCode))
            .WithMessage("Diagnosis code must not exceed 50 characters.");

        RuleFor(x => x.DiagnosesCode)
            .MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.DiagnosesCode))
            .WithMessage("Diagnosis code must not exceed 50 characters.");

        RuleFor(x => x.CodingSystem)
            .MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.CodingSystem))
            .WithMessage("Coding system must not exceed 50 characters.");

        RuleFor(x => x.ClinicalFindings)
            .MaximumLength(2000).When(x => !string.IsNullOrWhiteSpace(x.ClinicalFindings))
            .WithMessage("Clinical findings must not exceed 2000 characters.");

        RuleFor(x => x.DiagnosesText)
            .MaximumLength(2000).When(x => !string.IsNullOrWhiteSpace(x.DiagnosesText))
            .WithMessage("Clinical findings must not exceed 2000 characters.");

        RuleFor(x => x.SeverityLevel)
            .MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.SeverityLevel))
            .WithMessage("Severity level must not exceed 50 characters.");

        RuleFor(x => x.ConfidenceLevel)
            .InclusiveBetween(0, 100).When(x => x.ConfidenceLevel.HasValue)
            .WithMessage("Confidence level must be between 0 and 100.");

        RuleFor(x => x.TreatmentPlan)
            .MaximumLength(2000).When(x => x.TreatmentPlan is not null)
            .WithMessage("Treatment plan must not exceed 2000 characters.");

        RuleFor(x => x.Recommendations)
            .MaximumLength(2000).When(x => !string.IsNullOrWhiteSpace(x.Recommendations))
            .WithMessage("Recommendations must not exceed 2000 characters.");

        RuleFor(x => x.LifestyleAdvice)
            .MaximumLength(2000).When(x => !string.IsNullOrWhiteSpace(x.LifestyleAdvice))
            .WithMessage("Lifestyle advice must not exceed 2000 characters.");

        RuleFor(x => x.Status)
            .MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.Status))
            .WithMessage("Status must not exceed 50 characters.");
    }
}
