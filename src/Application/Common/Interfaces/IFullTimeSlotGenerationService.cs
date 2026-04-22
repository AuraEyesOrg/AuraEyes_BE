namespace Application.Common.Interfaces;

public interface IFullTimeSlotGenerationService
{
    Task TriggerForOphthalmologistAsync(Guid ophthalmologistId, CancellationToken cancellationToken = default);
}
