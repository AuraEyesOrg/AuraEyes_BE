using Domain.Common;
using Domain.Entities.CarePlan;

namespace Domain.Repositories;

/// <summary>
/// Repository contract for <see cref="HealthRoadmap"/> aggregate root.
/// </summary>
public interface IHealthRoadmapRepository : IRepository<HealthRoadmap>
{
    /// <summary>Get the (single) roadmap for a patient without steps.</summary>
    Task<HealthRoadmap?> GetByPatientAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);

    /// <summary>Get the roadmap for a patient with all its steps eagerly loaded.</summary>
    Task<HealthRoadmap?> GetByPatientWithStepsAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);

    /// <summary>Get a single step by ID, with its roadmap loaded.</summary>
    Task<HealthRoadmapStep?> GetStepByIdAsync(
        Guid stepId,
        CancellationToken cancellationToken = default);

    /// <summary>Hard-delete a step from the database. Used when business rules permit it.</summary>
    void RemoveStep(HealthRoadmapStep step);
}
