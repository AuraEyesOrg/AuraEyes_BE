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

        RuleFor(x => x.SlotType)
            .IsInEnum()
            .WithMessage("Invalid slot type.");

        RuleFor(x => x.Cost)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Cost.HasValue)
            .WithMessage("Cost cannot be negative.");
    }
}
