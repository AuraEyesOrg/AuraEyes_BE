using Domain.Entities.Users;

namespace Application.Scheduling.ScheduleTemplates.Interfaces;

public interface IFullTimeTemplateProvisioningService
{
    Task<int> EnsureSystemGeneratedTemplatesAsync(
        Ophthalmologist ophthalmologist,
        CancellationToken cancellationToken = default);
}
