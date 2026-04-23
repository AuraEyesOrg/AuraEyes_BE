namespace Application.Scheduling.ScheduleTemplates.Interfaces;

/// <summary>
/// Service interface for clinic-level template provisioning.
/// </summary>
public interface IFullTimeTemplateProvisioningService
{
    /// <summary>
    /// Ensure system-generated schedule templates exist for each weekday at the clinic level.
    /// </summary>
    Task<int> EnsureClinicTemplatesAsync(CancellationToken cancellationToken = default);
}
