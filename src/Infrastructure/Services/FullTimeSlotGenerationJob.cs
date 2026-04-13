using Application.Scheduling.ScheduleTemplates.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Hangfire recurring job that keeps a rolling window of slots available
/// for full-time ophthalmologists.
/// </summary>
public class FullTimeSlotGenerationJob
{
    private readonly IFullTimeTemplateProvisioningService _provisioningService;
    private readonly ILogger<FullTimeSlotGenerationJob> _logger;

    public FullTimeSlotGenerationJob(
        IFullTimeTemplateProvisioningService provisioningService,
        ILogger<FullTimeSlotGenerationJob> logger)
    {
        _provisioningService = provisioningService;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        const int rollingWindowDays = 14;

        var createdSlots = await _provisioningService.EnsureFutureSlotsForFullTimeAsync(
            rollingWindowDays,
            cancellationToken);

        _logger.LogInformation(
            "Full-time slot generation job completed. Created {CreatedSlotCount} slot(s)",
            createdSlots);
    }
}
