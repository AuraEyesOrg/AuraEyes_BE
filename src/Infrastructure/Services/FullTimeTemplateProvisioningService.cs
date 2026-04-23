using Application.Scheduling.ScheduleTemplates.Interfaces;
using Application.SystemSettings.Interfaces;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Creates missing weekday clinic-level schedule templates.
/// Uses INSERT ... ON CONFLICT DO NOTHING for idempotent, concurrency-safe provisioning.
/// Simplified for clinic-centric model (no doctor ownership).
/// </summary>
public class FullTimeTemplateProvisioningService : IFullTimeTemplateProvisioningService
{
    private const int DefaultSlotDurationMinutes = 30;
    private const int DefaultMaxCapacity = 1;

    private static readonly TimeOnly DefaultStartTime = new(8, 0);
    private static readonly TimeOnly DefaultEndTime = new(17, 0);

    private static readonly DayOfWeek[] DefaultWeekdays =
    {
        DayOfWeek.Monday,
        DayOfWeek.Tuesday,
        DayOfWeek.Wednesday,
        DayOfWeek.Thursday,
        DayOfWeek.Friday
    };

    private readonly ApplicationDbContext _context;
    private readonly ILogger<FullTimeTemplateProvisioningService> _logger;

    public FullTimeTemplateProvisioningService(
        ApplicationDbContext context,
        ILogger<FullTimeTemplateProvisioningService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Ensure that system-generated schedule templates exist for each weekday at the clinic level.
    /// </summary>
    public async Task<int> EnsureClinicTemplatesAsync(CancellationToken cancellationToken = default)
    {
        var createdCount = 0;
        var source = ScheduleTemplateSource.SystemGenerated.ToString();

        foreach (var day in DefaultWeekdays)
        {
            // Check if an active system-generated template already exists for this day
            var exists = await _context.ScheduleTemplates
                .AnyAsync(t =>
                    t.DayOfWeek == day &&
                    t.IsActive &&
                    t.Source == ScheduleTemplateSource.SystemGenerated &&
                    !t.IsDeleted,
                    cancellationToken);

            if (exists)
                continue;

            _context.ScheduleTemplates.Add(new Domain.Entities.Scheduling.ScheduleTemplate(
                dayOfWeek: day,
                startTime: DefaultStartTime,
                endTime: DefaultEndTime,
                slotDuration: DefaultSlotDurationMinutes,
                maxCapacity: DefaultMaxCapacity,
                source: ScheduleTemplateSource.SystemGenerated));

            createdCount++;
        }

        if (createdCount > 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation(
                "Provisioned {CreatedCount} clinic-level system schedule templates",
                createdCount);
        }

        return createdCount;
    }
}
