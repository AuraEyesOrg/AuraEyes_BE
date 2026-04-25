using Domain.Entities.CarePlan;
using Domain.Enums;

namespace Application.CarePlan.HealthRoadmaps.Common;

/// <summary>
/// Pure mapping helpers between domain entities and roadmap DTOs.
/// </summary>
internal static class HealthRoadmapMapper
{
    /// <summary>String name used for the derived Overdue status (not present in <see cref="RoadmapStepStatus"/>).</summary>
    public const string OverdueStatus = "Overdue";

    public static HealthRoadmapStepDto ToDto(HealthRoadmapStep step, DateOnly today)
    {
        var effective = step.IsOverdue(today)
            ? OverdueStatus
            : step.Status.ToString();

        return new HealthRoadmapStepDto
        {
            Id = step.Id,
            RoadmapId = step.RoadmapId,
            Title = step.Title,
            Description = step.Description,
            StepType = step.StepType.ToString(),
            PlannedDate = step.PlannedDate,
            Status = step.Status.ToString(),
            EffectiveStatus = effective,
            CreatedByDoctorId = step.CreatedByDoctorId,
            CreatedFromVisitId = step.CreatedFromVisitId,
            CompletedAt = step.CompletedAt,
            OrderIndex = step.OrderIndex,
            CreatedAt = step.CreatedAt,
            UpdatedAt = step.UpdatedAt
        };
    }

    public static HealthRoadmapDto ToDto(HealthRoadmap? roadmap, Guid patientId, DateOnly today)
    {
        if (roadmap is null)
        {
            return new HealthRoadmapDto
            {
                Id = null,
                PatientId = patientId,
                CreatedAt = null,
                UpdatedAt = null,
                Steps = Array.Empty<HealthRoadmapStepDto>()
            };
        }

        var steps = roadmap.Steps
            .OrderBy(s => s.PlannedDate)
            .ThenBy(s => s.OrderIndex)
            .ThenBy(s => s.CreatedAt)
            .Select(s => ToDto(s, today))
            .ToList();

        return new HealthRoadmapDto
        {
            Id = roadmap.Id,
            PatientId = roadmap.PatientId,
            CreatedAt = roadmap.CreatedAt,
            UpdatedAt = roadmap.UpdatedAt,
            Steps = steps
        };
    }
}
