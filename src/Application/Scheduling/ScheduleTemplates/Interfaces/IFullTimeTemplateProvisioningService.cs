using Domain.Entities.Users;

namespace Application.Scheduling.ScheduleTemplates.Interfaces;

/// <summary>
/// Ensures default system-generated schedule templates exist for full-time ophthalmologists.
/// </summary>
public interface IFullTimeTemplateProvisioningService
{
    Task<int> EnsureSystemGeneratedTemplatesAsync(
        Ophthalmologist ophthalmologist,
        CancellationToken cancellationToken = default);
}
