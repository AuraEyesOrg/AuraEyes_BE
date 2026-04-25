using Application.CarePlan.HealthRoadmaps.Common;
using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.CarePlan.HealthRoadmaps.Commands.UpdateRoadmapStep;

/// <summary>
/// Partial update of a roadmap step. Only Upcoming steps can be edited.
/// </summary>
public record UpdateRoadmapStepCommand : ICommand<HealthRoadmapStepDto>
{
    public Guid StepId { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public RoadmapStepType? StepType { get; init; }
    public DateOnly? PlannedDate { get; init; }
    public int? OrderIndex { get; init; }
}
