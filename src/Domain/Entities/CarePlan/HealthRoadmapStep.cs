using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Entities.Users;
using Domain.Enums;

namespace Domain.Entities.CarePlan;

/// <summary>
/// A single structured step in a patient's <see cref="HealthRoadmap"/>.
/// Authored by a doctor. Each step has a required <see cref="PlannedDate"/> that drives timeline rendering.
/// </summary>
public class HealthRoadmapStep : BaseEntity
{
    /// <summary>FK to parent <see cref="HealthRoadmap"/>.</summary>
    public Guid RoadmapId { get; private set; }

    /// <summary>Short title of the step (e.g. "Follow-up visit", "Re-check vision").</summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>Optional longer description / instruction.</summary>
    public string? Description { get; private set; }

    /// <summary>Type of step (see <see cref="RoadmapStepType"/>).</summary>
    public RoadmapStepType StepType { get; private set; }

    /// <summary>Required planned date. Drives timeline rendering and Overdue derivation.</summary>
    public DateOnly PlannedDate { get; private set; }

    /// <summary>Persisted status. Overdue is NOT persisted - derived on read.</summary>
    public RoadmapStepStatus Status { get; private set; }

    /// <summary>FK to Ophthalmologist who created the step.</summary>
    public Guid CreatedByDoctorId { get; private set; }

    /// <summary>Optional FK to the <see cref="PatientVisit"/> that originated this step.</summary>
    public Guid? CreatedFromVisitId { get; private set; }

    /// <summary>When the step was marked Completed.</summary>
    public DateTime? CompletedAt { get; private set; }

    /// <summary>Fallback ordering when two steps share the same <see cref="PlannedDate"/>.</summary>
    public int OrderIndex { get; private set; }

    // Navigation properties
    public HealthRoadmap? Roadmap { get; private set; }
    public Ophthalmologist? CreatedByDoctor { get; private set; }
    public PatientVisit? CreatedFromVisit { get; private set; }

    private HealthRoadmapStep() { } // EF Core

    /// <summary>
    /// Factory: build a new step. <paramref name="plannedDate"/> is required.
    /// </summary>
    public static HealthRoadmapStep Create(
        Guid roadmapId,
        string title,
        string? description,
        RoadmapStepType stepType,
        DateOnly plannedDate,
        Guid createdByDoctorId,
        Guid? createdFromVisitId = null,
        int orderIndex = 0)
    {
        if (roadmapId == Guid.Empty)
            throw new ArgumentException("Roadmap ID is required.", nameof(roadmapId));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));
        if (createdByDoctorId == Guid.Empty)
            throw new ArgumentException("Doctor ID is required.", nameof(createdByDoctorId));

        return new HealthRoadmapStep
        {
            RoadmapId = roadmapId,
            Title = title.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            StepType = stepType,
            PlannedDate = plannedDate,
            Status = RoadmapStepStatus.Upcoming,
            CreatedByDoctorId = createdByDoctorId,
            CreatedFromVisitId = createdFromVisitId,
            OrderIndex = orderIndex
        };
    }

    /// <summary>
    /// Update mutable fields. Only allowed when <see cref="Status"/> is Upcoming.
    /// </summary>
    public void UpdateDetails(
        string? title,
        string? description,
        RoadmapStepType? stepType,
        DateOnly? plannedDate,
        int? orderIndex)
    {
        if (Status == RoadmapStepStatus.Completed)
            throw new InvalidOperationException("Completed steps cannot be edited.");

        if (title is not null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.", nameof(title));
            Title = title.Trim();
        }

        if (description is not null)
        {
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        }

        if (stepType.HasValue) StepType = stepType.Value;
        if (plannedDate.HasValue) PlannedDate = plannedDate.Value;
        if (orderIndex.HasValue) OrderIndex = orderIndex.Value;

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Mark this step as Completed.</summary>
    public void MarkCompleted()
    {
        if (Status == RoadmapStepStatus.Completed)
            throw new InvalidOperationException("Step is already completed.");
        if (Status == RoadmapStepStatus.Cancelled)
            throw new InvalidOperationException("Cannot complete a cancelled step.");

        Status = RoadmapStepStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Cancel this step. Completed steps cannot be cancelled.</summary>
    public void Cancel()
    {
        if (Status == RoadmapStepStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed step.");

        Status = RoadmapStepStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Indicates whether this step would be reported as Overdue today.
    /// Pure projection; never persisted.
    /// </summary>
    public bool IsOverdue(DateOnly today)
        => Status == RoadmapStepStatus.Upcoming && PlannedDate < today;
}
