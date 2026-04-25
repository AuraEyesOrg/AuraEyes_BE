using FluentValidation;

namespace Application.CarePlan.HealthRoadmaps.Commands.UpdateRoadmapStep;

public class UpdateRoadmapStepCommandValidator : AbstractValidator<UpdateRoadmapStepCommand>
{
    public UpdateRoadmapStepCommandValidator()
    {
        RuleFor(x => x.StepId)
            .NotEmpty().WithMessage("Step ID is required.");

        When(x => x.Title is not null, () =>
        {
            RuleFor(x => x.Title!)
                .NotEmpty().WithMessage("Title cannot be empty.")
                .MaximumLength(200).WithMessage("Title must be at most 200 characters.");
        });

        When(x => x.Description is not null, () =>
        {
            RuleFor(x => x.Description!)
                .MaximumLength(2000).WithMessage("Description must be at most 2000 characters.");
        });

        When(x => x.StepType.HasValue, () =>
        {
            RuleFor(x => x.StepType!.Value)
                .IsInEnum().WithMessage("Step type is invalid.");
        });

        When(x => x.PlannedDate.HasValue, () =>
        {
            RuleFor(x => x.PlannedDate!.Value)
                .NotEqual(default(DateOnly)).WithMessage("Planned date is required.");
        });
    }
}
