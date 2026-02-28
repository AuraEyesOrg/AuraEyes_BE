using FluentValidation;

namespace Application.ConsultationSessions.Commands.CreateVideoCallSession;

public class CreateVideoCallSessionCommandValidator
    : AbstractValidator<CreateVideoCallSessionCommand>
{
    public CreateVideoCallSessionCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient ID is required.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");

        RuleFor(x => x.AppointmentTime)
            .GreaterThan(DateTime.UtcNow).WithMessage("Appointment time must be in the future.");
    }
}
