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

        RuleFor(x => x.DiagnosesCode)
            .NotEmpty().WithMessage("Diagnosis code is required.")
            .MaximumLength(50).WithMessage("Diagnosis code must not exceed 50 characters.");

        RuleFor(x => x.DiagnosesText)
            .NotEmpty().WithMessage("Diagnosis text is required.")
            .MaximumLength(2000).WithMessage("Diagnosis text must not exceed 2000 characters.");

        RuleFor(x => x.TreatmentPlan)
            .MaximumLength(2000).When(x => x.TreatmentPlan is not null)
            .WithMessage("Treatment plan must not exceed 2000 characters.");
    }
}
