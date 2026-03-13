using FluentValidation;

namespace Application.Ophthalmologists.Schedules.Commands.UpdateScheduleCost;

public class UpdateScheduleCostCommandValidator : AbstractValidator<UpdateScheduleCostCommand>
{
    public UpdateScheduleCostCommandValidator()
    {
        RuleFor(x => x.ScheduleId)
            .NotEmpty()
            .WithMessage("Schedule ID is required.");

        RuleFor(x => x.Cost)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Cost.HasValue)
            .WithMessage("Cost must be non-negative.");
    }
}
