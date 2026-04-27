using Domain.Common;
using Domain.Entities.Users;

namespace Domain.Entities.CarePlan;

/// <summary>
/// HealthRoadmap - doctor-authored, structured care plan timeline for a patient.
/// One roadmap per patient (1:1). Auto-created when the first step is added.
/// Distinct from <c>Domain.Entities.Screening.PatientRoadmap</c> (which is AI-generated).
/// </summary>
public class HealthRoadmap : BaseEntity, IAggregateRoot
{
    /// <summary>FK to Patient. Unique - one roadmap per patient.</summary>
    public Guid PatientId { get; private set; }

    // Navigation properties
    public Patient? Patient { get; private set; }

    private readonly List<HealthRoadmapStep> _steps = new();
    public IReadOnlyCollection<HealthRoadmapStep> Steps => _steps.AsReadOnly();

    private HealthRoadmap() { } // EF Core

    /// <summary>
    /// Create a new roadmap for a patient. Use this once when the first step is being added.
    /// </summary>
    public static HealthRoadmap CreateForPatient(Guid patientId)
    {
        if (patientId == Guid.Empty)
            throw new ArgumentException("Patient ID is required.", nameof(patientId));

        return new HealthRoadmap
        {
            PatientId = patientId
        };
    }

    /// <summary>
    /// Append a step to this roadmap. The step is constructed by the caller via
    /// <see cref="HealthRoadmapStep.Create"/>; this method just attaches it and bumps UpdatedAt.
    /// </summary>
    public void AddStep(HealthRoadmapStep step)
    {
        if (step is null) throw new ArgumentNullException(nameof(step));
        _steps.Add(step);
        UpdatedAt = DateTime.UtcNow;
    }
}
