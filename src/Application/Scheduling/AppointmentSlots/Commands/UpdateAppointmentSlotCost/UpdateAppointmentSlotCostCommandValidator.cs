using FluentValidation;

namespace Application.Scheduling.AppointmentSlots.Commands.UpdateAppointmentSlotCost;

public class UpdateAppointmentSlotCostCommandValidator : AbstractValidator<UpdateAppointmentSlotCostCommand>
{
    public UpdateAppointmentSlotCostCommandValidator()
    {
        RuleFor(x => x.AppointmentSlotId)
            .NotEmpty()
            .WithMessage("Appointment slot ID is required.");

        RuleFor(x => x.Cost)
            .NotNull()
            .WithMessage("Cost is required.");

        RuleFor(x => x.Cost)
            .GreaterThan(0)
            .When(x => x.Cost.HasValue)
            .WithMessage("Cost must be a positive value.");

        RuleFor(x => x.Cost)
            .Must(cost => !cost.HasValue || decimal.Truncate(cost.Value) == cost.Value)
            .WithMessage("Cost must be an integer value.");
    }
}