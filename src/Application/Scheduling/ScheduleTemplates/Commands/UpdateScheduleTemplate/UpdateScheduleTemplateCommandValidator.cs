using FluentValidation;

namespace Application.Scheduling.ScheduleTemplates.Commands.UpdateScheduleTemplate;

public class UpdateScheduleTemplateCommandValidator : AbstractValidator<UpdateScheduleTemplateCommand>
{
    public UpdateScheduleTemplateCommandValidator()
    {
        RuleFor(x => x.ScheduleTemplateId)
            .NotEmpty().WithMessage("Schedule template ID is required.");

        RuleFor(x => x.DayOfWeek)
            .IsInEnum().WithMessage("Invalid day of week.");

        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime).WithMessage("Start time must be before end time.");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time.");

        RuleFor(x => x.SlotDuration)
            .GreaterThanOrEqualTo(1).WithMessage("Slot duration must be at least 1 minute.");

        RuleFor(x => x.MaxCapacity)
            .GreaterThanOrEqualTo(1).WithMessage("Max capacity must be at least 1.");

        RuleFor(x => x.Cost)
            .GreaterThan(0)
            .When(x => x.Cost.HasValue)
            .WithMessage("Cost must be a positive value.");

        RuleFor(x => x.Cost)
            .Must(cost => !cost.HasValue || decimal.Truncate(cost.Value) == cost.Value)
            .WithMessage("Cost must be an integer value.");
    }
}
