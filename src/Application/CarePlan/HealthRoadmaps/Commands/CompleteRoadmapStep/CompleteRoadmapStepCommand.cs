using Application.CarePlan.HealthRoadmaps.Common;
using Application.Common.Interfaces;

namespace Application.CarePlan.HealthRoadmaps.Commands.CompleteRoadmapStep;

/// <summary>Mark a roadmap step as Completed.</summary>
public record CompleteRoadmapStepCommand(Guid StepId) : ICommand<HealthRoadmapStepDto>;
