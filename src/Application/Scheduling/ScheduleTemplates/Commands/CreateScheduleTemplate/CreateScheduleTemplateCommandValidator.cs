using FluentValidation;

namespace Application.Scheduling.ScheduleTemplates.Commands.CreateScheduleTemplate;

public class CreateScheduleTemplateCommandValidator : AbstractValidator<CreateScheduleTemplateCommand>
{
    public CreateScheduleTemplateCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => x.OrgId.HasValue || x.OphthalId.HasValue)
            .WithMessage("At least one of OrgId or OphthalId must be provided.");

        RuleFor(x => x.DayOfWeek)
            .IsInEnum()
            .WithMessage("Invalid day of week.");

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .WithMessage("Start time is required.");

        RuleFor(x => x.EndTime)
            .NotEmpty()
            .WithMessage("End time is required.")
            .GreaterThan(x => x.StartTime)
            .WithMessage("End time must be after start time.");

        RuleFor(x => x.SlotDuration)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Slot duration must be at least 1 minute.");

        RuleFor(x => x.MaxCapacity)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Max capacity must be at least 1.");
    }
}
