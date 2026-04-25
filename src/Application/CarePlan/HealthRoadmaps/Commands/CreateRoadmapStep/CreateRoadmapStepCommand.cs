using Application.CarePlan.HealthRoadmaps.Common;
using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.CarePlan.HealthRoadmaps.Commands.CreateRoadmapStep;

/// <summary>
/// Command: doctor creates a new roadmap step for a patient.
/// Auto-creates the patient's <see cref="Domain.Entities.CarePlan.HealthRoadmap"/> if none exists.
/// </summary>
public record CreateRoadmapStepCommand : ICommand<HealthRoadmapStepDto>
{
    public Guid PatientId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public RoadmapStepType StepType { get; init; }
    public DateOnly PlannedDate { get; init; }
    public Guid? CreatedFromVisitId { get; init; }
    public int OrderIndex { get; init; }
}
