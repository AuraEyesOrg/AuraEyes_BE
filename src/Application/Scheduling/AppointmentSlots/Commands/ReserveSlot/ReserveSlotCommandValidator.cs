using FluentValidation;

namespace Application.Scheduling.AppointmentSlots.Commands.ReserveSlot;

public class ReserveSlotCommandValidator : AbstractValidator<ReserveSlotCommand>
{
    public ReserveSlotCommandValidator()
    {
        RuleFor(x => x.AppointmentSlotId)
            .NotEmpty()
            .WithMessage("Appointment slot ID is required.");

        RuleFor(x => x.PatientId)
            .NotEmpty()
            .WithMessage("Patient ID is required.");

        RuleFor(x => x.ReservationMinutes)
            .InclusiveBetween(1, 15)
            .WithMessage("Reservation duration must be between 1 and 15 minutes.");
    }
}
