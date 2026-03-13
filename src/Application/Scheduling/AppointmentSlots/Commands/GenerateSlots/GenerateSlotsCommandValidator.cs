using FluentValidation;

namespace Application.Scheduling.AppointmentSlots.Commands.GenerateSlots;

public class GenerateSlotsCommandValidator : AbstractValidator<GenerateSlotsCommand>
{
    public GenerateSlotsCommandValidator()
    {
        RuleFor(x => x.ScheduleTemplateId)
            .NotEmpty()
            .WithMessage("Schedule template ID is required.");

        RuleFor(x => x.FromDate)
            .NotEmpty()
            .WithMessage("From date is required.");

        RuleFor(x => x.ToDate)
            .NotEmpty()
            .WithMessage("To date is required.");

        RuleFor(x => x.ToDate)
            .GreaterThanOrEqualTo(x => x.FromDate)
            .WithMessage("To date must be greater than or equal to from date.");

        RuleFor(x => x)
            .Must(x => (x.ToDate.DayNumber - x.FromDate.DayNumber) <= 90)
            .WithMessage("Date range cannot exceed 90 days.");
    }
}
