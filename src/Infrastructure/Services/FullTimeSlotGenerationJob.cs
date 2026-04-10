using Application.Scheduling.ScheduleTemplates.Interfaces;
using Application.SystemSettings.Interfaces;
using Domain.Entities.Scheduling;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Recurring job that keeps a rolling window of slots for full-time doctors.
/// </summary>
public class FullTimeSlotGenerationJob
{
    private const string FullTimeSlotWindowDaysSettingKey = "FULLTIME_SLOT_WINDOW_DAYS";
    private const int FixedRollingWindowDays = 7;

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

        var fullTimeOphthalmologists = await _context.Ophthalmologists
            .Where(ophthal => ophthal.EmploymentType == OphthalmologistEmploymentType.FullTime)
            .ToListAsync(cancellationToken);

        var templatesEnsured = 0;
        foreach (var ophthalmologist in fullTimeOphthalmologists)
        {
            templatesEnsured += await _fullTimeTemplateProvisioningService
                .EnsureSystemGeneratedTemplatesAsync(ophthalmologist, cancellationToken);
        }

        _logger.LogInformation(
            "Starting full-time slot rolling-window generation. FromDate={FromDate}, ToDate={ToDate}, WindowDays={WindowDays}, TemplatesEnsured={TemplatesEnsured}",
            fromDate,
            toDate,
            windowDays,
            templatesEnsured);

        var templates = await (
            from template in _context.ScheduleTemplates
            where template.OphthalId.HasValue
            join ophthal in _context.Ophthalmologists on template.OphthalId!.Value equals ophthal.Id
            where template.Source == ScheduleTemplateSource.SystemGenerated
                && template.IsActive
                  && ophthal.EmploymentType == OphthalmologistEmploymentType.FullTime
            select template)
            .ToListAsync(cancellationToken);

        var createdSlots = 0;
        var skippedInvalidTemplates = 0;
        foreach (var template in templates)
        {
            var slotDuration = TimeSpan.FromMinutes(template.SlotDuration);
            var templateStart = template.StartTime.ToTimeSpan();
            var templateEnd = template.EndTime.ToTimeSpan();

            if (template.SlotDuration <= 0
                || slotDuration <= TimeSpan.Zero
                || templateEnd <= templateStart
                || template.MaxCapacity <= 0)
            {
                skippedInvalidTemplates++;
                _logger.LogWarning(
                    "Skipping invalid full-time template {TemplateId}. SlotDuration={SlotDuration}, StartTime={StartTime}, EndTime={EndTime}, MaxCapacity={MaxCapacity}",
                    template.Id,
                    template.SlotDuration,
                    template.StartTime,
                    template.EndTime,
                    template.MaxCapacity);
                continue;
            }

            try
            {
                var existingDates = await _context.AppointmentSlots
                    .Where(s => s.ScheduleTemplateId == template.Id)
                    .Where(s => s.Date >= fromDate && s.Date <= toDate)
                    .Select(s => s.Date)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                var existingDateSet = existingDates.ToHashSet();

                var currentDate = fromDate;
                while (currentDate <= toDate)
                {
                    if (currentDate.DayOfWeek == template.DayOfWeek && !existingDateSet.Contains(currentDate))
                    {
                        for (var currentStart = templateStart; currentStart + slotDuration <= templateEnd; currentStart += slotDuration)
                        {
                            var slotEndSpan = currentStart + slotDuration;
                            var slotStart = TimeOnly.FromTimeSpan(currentStart);
                            var slotEnd = TimeOnly.FromTimeSpan(slotEndSpan);

                            _context.AppointmentSlots.Add(new AppointmentSlot(
                                template.Id,
                                currentDate,
                                slotStart,
                                slotEnd,
                                template.MaxCapacity,
                                template.Cost,
                                SlotSource.System));

                            createdSlots++;
                        }
                    }

                    currentDate = currentDate.AddDays(1);
                }
            }
            catch (ArgumentException ex)
            {
                skippedInvalidTemplates++;
                _logger.LogWarning(
                    ex,
                    "Skipping template {TemplateId} due to invalid data while generating full-time slots.",
                    template.Id);
            }
        }

        if (createdSlots > 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation(
            "Completed full-time slot rolling-window generation. Templates={TemplateCount}, TemplatesEnsured={TemplatesEnsured}, SlotsCreated={SlotsCreated}, SkippedInvalidTemplates={SkippedInvalidTemplates}",
            templates.Count,
            templatesEnsured,
            createdSlots,
            skippedInvalidTemplates);
    }

    /// <summary>
    /// Backward-compatible overload for legacy Hangfire payloads.
    /// </summary>
    [Obsolete("Use ExecuteAsync(CancellationToken) instead. This overload exists for Hangfire compatibility.")]
    public Task ExecuteAsync() => ExecuteAsync(CancellationToken.None);

    /// <summary>
    /// Backward-compatible entry point for legacy Hangfire payloads.
    /// </summary>
    [Obsolete("Use ExecuteAsync(CancellationToken) instead. This overload exists for Hangfire compatibility.")]
    public Task Execute() => ExecuteAsync(CancellationToken.None);

    private async Task<int> GetWindowDaysAsync(CancellationToken cancellationToken)
    {
        var configured = await _settingService.GetSettingAsync(FullTimeSlotWindowDaysSettingKey, cancellationToken);
        if (int.TryParse(configured, out var configuredDays)
            && configuredDays > 0
            && configuredDays != FixedRollingWindowDays)
        {
            _logger.LogWarning(
                "Ignoring configured {SettingKey}={ConfiguredDays}. Recurring full-time slot generation uses a fixed 7-day window.",
                FullTimeSlotWindowDaysSettingKey,
                configuredDays);
        }

        return FixedRollingWindowDays;
    }
}