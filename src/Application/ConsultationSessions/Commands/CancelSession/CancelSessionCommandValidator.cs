using FluentValidation;

namespace Application.ConsultationSessions.Commands.CancelSession;

public class CancelSessionCommandValidator : AbstractValidator<CancelSessionCommand>
{
    public CancelSessionCommandValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("Session ID is required.");

        RuleFor(x => x.CancelledByUserId)
            .NotEmpty().WithMessage("Cancelled-by user ID is required.");

        RuleFor(x => x.Reason)
            .MaximumLength(200).WithMessage("Reason must not exceed 200 characters.");
    }
}
