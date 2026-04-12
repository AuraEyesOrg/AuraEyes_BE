using FluentValidation;

namespace Application.Scheduling.AppointmentSlots.Commands.UpdateAppointmentSlot;

public class UpdateAppointmentSlotCommandValidator : AbstractValidator<UpdateAppointmentSlotCommand>
{
    public UpdateAppointmentSlotCommandValidator()
    {
        RuleFor(x => x.AppointmentSlotId)
            .NotEmpty().WithMessage("Appointment slot ID is required.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required.");

        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime).WithMessage("Start time must be before end time.");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time.");

        RuleFor(x => x.Cost)
            .GreaterThan(0)
            .When(x => x.Cost.HasValue)
            .WithMessage("Cost must be a positive value.");

        RuleFor(x => x.Cost)
            .Must(cost => !cost.HasValue || decimal.Truncate(cost.Value) == cost.Value)
            .WithMessage("Cost must be an integer value.");
    }
}
