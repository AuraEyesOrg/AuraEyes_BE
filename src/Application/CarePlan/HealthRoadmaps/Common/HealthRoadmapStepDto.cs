namespace Application.CarePlan.HealthRoadmaps.Common;

/// <summary>
/// DTO for a single roadmap step.
/// <see cref="EffectiveStatus"/> reflects the derived status (Overdue when applicable);
/// <see cref="Status"/> is the raw persisted status.
/// </summary>
public class HealthRoadmapStepDto
{
    public Guid Id { get; set; }
    public Guid RoadmapId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string StepType { get; set; } = string.Empty;
    public DateOnly PlannedDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string EffectiveStatus { get; set; } = string.Empty;
    public Guid CreatedByDoctorId { get; set; }
    public Guid? CreatedFromVisitId { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int OrderIndex { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
