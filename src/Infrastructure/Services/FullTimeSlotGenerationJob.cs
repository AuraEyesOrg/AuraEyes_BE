using Application.Scheduling.ScheduleTemplates.Interfaces;
using Application.SystemSettings.Interfaces;
using Domain.Entities.Scheduling;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Recurring job that keeps a rolling window of slots for clinic-level templates.
/// Redesigned for clinic-centric, resource-based scheduling.
/// </summary>
public class FullTimeSlotGenerationJob
{
    private const string FullTimeSlotWindowDaysSettingKey = "FULLTIME_SLOT_WINDOW_DAYS";
    private const int DefaultRollingWindowDays = 14;

    private readonly ApplicationDbContext _context;
    private readonly IFullTimeTemplateProvisioningService _fullTimeTemplateProvisioningService;
    private readonly ISystemSettingService _settingService;
    private readonly ILogger<FullTimeSlotGenerationJob> _logger;

    public FullTimeSlotGenerationJob(
        ApplicationDbContext context,
        IFullTimeTemplateProvisioningService fullTimeTemplateProvisioningService,
        ISystemSettingService settingService,
        ILogger<FullTimeSlotGenerationJob> logger)
    {
        _context = context;
        _fullTimeTemplateProvisioningService = fullTimeTemplateProvisioningService;
        _settingService = settingService;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var windowDays = await GetWindowDaysAsync(cancellationToken);
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var toDate = fromDate.AddDays(windowDays - 1);

        // Ensure clinic-level templates exist for basic weekdays
        var templatesEnsured = await _fullTimeTemplateProvisioningService
            .EnsureClinicTemplatesAsync(cancellationToken);

        _logger.LogInformation(
            "Starting clinic slot rolling-window generation. FromDate={FromDate}, ToDate={ToDate}, WindowDays={WindowDays}, TemplatesEnsured={TemplatesEnsured}",
            fromDate, toDate, windowDays, templatesEnsured);

        // Get all active templates (usually SystemGenerated for rolling windows)
        var templates = await _context.ScheduleTemplates
            .Where(t => t.IsActive && !t.IsDeleted)
            .ToListAsync(cancellationToken);

        var createdSlots = 0;
        var skippedInvalidTemplates = 0;

        foreach (var template in templates)
        {
            var slotDurationMinutes = template.SlotDuration;
            var templateStart = template.StartTime.ToTimeSpan();
            var templateEnd = template.EndTime.ToTimeSpan();

            if (slotDurationMinutes <= 0 || templateEnd <= templateStart || template.MaxCapacity <= 0)
            {
                skippedInvalidTemplates++;
                _logger.LogWarning(
                    "Skipping invalid template {TemplateId}. SlotDuration={SlotDuration}, StartTime={StartTime}, EndTime={EndTime}, MaxCapacity={MaxCapacity}",
                    template.Id, template.SlotDuration, template.StartTime, template.EndTime, template.MaxCapacity);
                continue;
            }

            try
            {
                // Fetch all existing slots for this template in the window to prevent duplicates
                // Using (Date, StartTime) as the unique identity for an occurrence of a template
                var existingSlots = await _context.AppointmentSlots
                    .Where(s => s.ScheduleTemplateId == template.Id && !s.IsDeleted)
                    .Where(s => s.Date >= fromDate && s.Date <= toDate)
                    .Select(s => new { s.Date, s.StartTime })
                    .ToListAsync(cancellationToken);

                var existingSlotMap = existingSlots
                    .GroupBy(s => s.Date)
                    .ToDictionary(g => g.Key, g => g.Select(s => s.StartTime).ToHashSet());

                var slotDuration = TimeSpan.FromMinutes(slotDurationMinutes);
                var currentDate = fromDate;

                while (currentDate <= toDate)
                {
                    if (currentDate.DayOfWeek == template.DayOfWeek)
                    {
                        existingSlotMap.TryGetValue(currentDate, out var existingTimes);
                        
                        for (var currentStart = templateStart; currentStart + slotDuration <= templateEnd; currentStart += slotDuration)
                        {
                            var slotStart = TimeOnly.FromTimeSpan(currentStart);
                            
                            // PREVENT DUPLICATES: Check if a slot already exists with same template + date + start_time
                            if (existingTimes != null && existingTimes.Contains(slotStart))
                                continue;

                            var slotEndSpan = currentStart + slotDuration;
                            var slotEnd = TimeOnly.FromTimeSpan(slotEndSpan);

                            _context.AppointmentSlots.Add(new AppointmentSlot(
                                template.Id,
                                currentDate,
                                slotStart,
                                slotEnd,
                                template.MaxCapacity,
                                SlotSource.System));

                            createdSlots++;
                        }
                    }

                    currentDate = currentDate.AddDays(1);
                }
            }
            catch (Exception ex)
            {
                skippedInvalidTemplates++;
                _logger.LogError(ex, "Error generating slots for template {TemplateId}", template.Id);
            }
        }

        if (createdSlots > 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation(
            "Completed clinic slot rolling-window generation. TemplatesProcessed={TemplateCount}, SlotsCreated={SlotsCreated}, SkippedInvalidTemplates={SkippedInvalidTemplates}",
            templates.Count, createdSlots, skippedInvalidTemplates);
    }

    /// <summary>
    /// Backward-compatible overload for legacy Hangfire payloads.
    /// </summary>
    [Obsolete("Use ExecuteAsync(CancellationToken) instead.")]
    public Task ExecuteAsync() => ExecuteAsync(CancellationToken.None);

    /// <summary>
    /// Backward-compatible entry point for legacy Hangfire payloads.
    /// </summary>
    [Obsolete("Use ExecuteAsync(CancellationToken) instead.")]
    public Task Execute() => ExecuteAsync(CancellationToken.None);

    private async Task<int> GetWindowDaysAsync(CancellationToken cancellationToken)
    {
        var configured = await _settingService.GetSettingAsync(FullTimeSlotWindowDaysSettingKey, cancellationToken);
        if (int.TryParse(configured, out var configuredDays) && configuredDays > 0)
        {
            return configuredDays;
        }

        return DefaultRollingWindowDays;
    }
}
