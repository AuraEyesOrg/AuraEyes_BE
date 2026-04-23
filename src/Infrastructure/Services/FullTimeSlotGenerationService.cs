using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class FullTimeSlotGenerationService : IFullTimeSlotGenerationService
{
    private readonly FullTimeSlotGenerationJob _fullTimeSlotGenerationJob;
    private readonly ILogger<FullTimeSlotGenerationService> _logger;

    public FullTimeSlotGenerationService(
        FullTimeSlotGenerationJob fullTimeSlotGenerationJob,
        ILogger<FullTimeSlotGenerationService> logger)
    {
        _fullTimeSlotGenerationJob = fullTimeSlotGenerationJob;
        _logger = logger;
    }

    public async Task TriggerForOphthalmologistAsync(
        Guid ophthalmologistId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Targeted full-time slot generation for ophthalmologist {OphthalmologistId} is obsolete.",
            ophthalmologistId);
    }
}
