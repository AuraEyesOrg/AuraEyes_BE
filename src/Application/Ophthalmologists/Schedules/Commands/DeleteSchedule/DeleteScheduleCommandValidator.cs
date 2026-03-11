using FluentValidation;

namespace Application.Ophthalmologists.Schedules.Commands.DeleteSchedule;

public class DeleteScheduleCommandValidator : AbstractValidator<DeleteScheduleCommand>
{
    public DeleteScheduleCommandValidator()
    {
        RuleFor(x => x.ScheduleId)
            .NotEmpty()
            .WithMessage("Schedule ID is required.");
    }
}
