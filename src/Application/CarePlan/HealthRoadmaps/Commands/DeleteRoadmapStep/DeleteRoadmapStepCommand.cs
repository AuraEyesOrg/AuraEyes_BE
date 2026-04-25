using Application.Common.Interfaces;

namespace Application.CarePlan.HealthRoadmaps.Commands.DeleteRoadmapStep;

/// <summary>Delete a roadmap step. Only allowed when the step is not Completed.</summary>
public record DeleteRoadmapStepCommand(Guid StepId) : ICommand;
