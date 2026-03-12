using FluentValidation;

namespace Application.Scheduling.AppointmentSlots.Commands.ConfirmReservation;

public class ConfirmReservationCommandValidator : AbstractValidator<ConfirmReservationCommand>
{
    public ConfirmReservationCommandValidator()
    {
        RuleFor(x => x.AppointmentSlotId)
            .NotEmpty()
            .WithMessage("Appointment slot ID is required.");

        RuleFor(x => x.PatientId)
            .NotEmpty()
            .WithMessage("Patient ID is required.");
    }
}
