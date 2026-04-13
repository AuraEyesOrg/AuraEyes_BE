using FluentValidation;

namespace Application.Scheduling.AppointmentSlots.Commands.CreateAppointmentSlot;

public class CreateAppointmentSlotCommandValidator : AbstractValidator<CreateAppointmentSlotCommand>
{
    public CreateAppointmentSlotCommandValidator()
    {
        RuleFor(x => x.ScheduleTemplateId)
            .NotEmpty()
            .WithMessage("Schedule template ID is required.");

        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date is required.");

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .WithMessage("Start time is required.");

        RuleFor(x => x.EndTime)
            .NotEmpty()
            .WithMessage("End time is required.")
            .GreaterThan(x => x.StartTime)
            .WithMessage("End time must be after start time.");

        RuleFor(x => x.Cost)
            .GreaterThan(0)
            .When(x => x.Cost.HasValue)
            .WithMessage("Cost must be a positive value.");

        RuleFor(x => x.Cost)
            .Must(cost => !cost.HasValue || decimal.Truncate(cost.Value) == cost.Value)
            .WithMessage("Cost must be an integer value.");
    }
}
