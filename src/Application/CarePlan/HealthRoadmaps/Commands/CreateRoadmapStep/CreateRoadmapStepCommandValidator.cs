using FluentValidation;

namespace Application.CarePlan.HealthRoadmaps.Commands.CreateRoadmapStep;

public class CreateRoadmapStepCommandValidator : AbstractValidator<CreateRoadmapStepCommand>
{
    public CreateRoadmapStepCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must be at most 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must be at most 2000 characters.");

        RuleFor(x => x.StepType)
            .IsInEnum().WithMessage("Step type is invalid.");

        RuleFor(x => x.PlannedDate)
            .NotEqual(default(DateOnly)).WithMessage("Planned date is required.");
    }
}
