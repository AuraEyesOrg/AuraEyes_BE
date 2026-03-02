using FluentValidation;

namespace Application.ConsultationSessions.Commands.EndSession;

public class EndSessionCommandValidator : AbstractValidator<EndSessionCommand>
{
    public EndSessionCommandValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("Session ID is required.");

        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("Doctor ID is required.");
    }
}
