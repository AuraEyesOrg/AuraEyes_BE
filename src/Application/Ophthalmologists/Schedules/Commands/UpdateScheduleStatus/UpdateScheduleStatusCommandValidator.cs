using FluentValidation;

namespace Application.Ophthalmologists.Schedules.Commands.UpdateScheduleStatus;

/// <summary>
/// Validator for UpdateScheduleStatusCommand.
/// </summary>
public class UpdateScheduleStatusCommandValidator : AbstractValidator<UpdateScheduleStatusCommand>
{
    public UpdateScheduleStatusCommandValidator()
    {
        RuleFor(x => x.ScheduleId)
            .NotEmpty()
            .WithMessage("Schedule ID is required.");

        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage("Invalid schedule status.");
    }
}
