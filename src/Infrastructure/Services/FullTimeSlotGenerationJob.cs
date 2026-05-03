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
    private readonly ISystemSettingService _settingService;
    private readonly ILogger<FullTimeSlotGenerationJob> _logger;

    public FullTimeSlotGenerationJob(
        ApplicationDbContext context,
        ISystemSettingService settingService,
        ILogger<FullTimeSlotGenerationJob> logger)
    {
        _context = context;
        _settingService = settingService;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var windowDays = await GetWindowDaysAsync(cancellationToken);
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var toDate = fromDate.AddDays(windowDays - 1);

        _logger.LogInformation(
            "Starting clinic slot rolling-window generation. FromDate={FromDate}, ToDate={ToDate}, WindowDays={WindowDays}",
            fromDate, toDate, windowDays);

        // Get all active templates (usually SystemGenerated for rolling windows)
        var templates = await _context.ScheduleTemplates
            .Where(t => t.IsActive && !t.IsDeleted)
            .ToListAsync(cancellationToken);

        // Fetch doctors once for clinic-wide templates
        var doctors = await _context.Ophthalmologists
            .Where(o => !o.IsDeleted)
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
                    .Select(s => new { s.Date, s.StartTime, s.OphthalId })
                    .ToListAsync(cancellationToken);

                var existingSlotMap = existingSlots
                    .GroupBy(s => new { s.Date, s.OphthalId })
                    .ToDictionary(g => g.Key, g => g.Select(s => s.StartTime).ToHashSet());

                var slotDuration = TimeSpan.FromMinutes(slotDurationMinutes);
                var currentDate = fromDate;

                while (currentDate <= toDate)
                {
                    if (currentDate.DayOfWeek == template.DayOfWeek)
                    {
                        for (var currentStart = templateStart; currentStart + slotDuration <= templateEnd; currentStart += slotDuration)
                        {
                            var slotStart = TimeOnly.FromTimeSpan(currentStart);
                            var slotEndSpan = currentStart + slotDuration;
                            var slotEnd = TimeOnly.FromTimeSpan(slotEndSpan);
                            
                            if (doctors.Any())
                            {
                                foreach (var doctor in doctors)
                                {
                                    // Check if this doctor already has a slot for this time
                                    if (existingSlotMap.TryGetValue(new { Date = currentDate, OphthalId = (Guid?)doctor.Id }, out var docTimes) 
                                        && docTimes.Contains(slotStart))
                                        continue;

                                    // Check if there is a "generic" slot (OphthalId is null) for this time and remove it
                                    // to make room for doctor-specific slots and avoid confusion
                                    if (existingSlotMap.TryGetValue(new { Date = currentDate, OphthalId = (Guid?)null }, out var genericTimes)
                                        && genericTimes.Contains(slotStart))
                                    {
                                        var genericSlot = await _context.AppointmentSlots
                                            .FirstOrDefaultAsync(s => s.ScheduleTemplateId == template.Id 
                                                && s.Date == currentDate && s.StartTime == slotStart 
                                                && s.OphthalId == null && !s.IsDeleted, cancellationToken);
                                        if (genericSlot != null) _context.AppointmentSlots.Remove(genericSlot);
                                        
                                        // Remove from map so we don't try to delete it again for the next doctor
                                        genericTimes.Remove(slotStart);
                                    }

                                    var slot = new AppointmentSlot(
                                        template.Id,
                                        currentDate,
                                        slotStart,
                                        slotEnd,
                                        1);
                                    
                                    slot.UpdateOphthalId(doctor.Id);
                                    var cost = doctor.ConsultationFee > 0 ? doctor.ConsultationFee : (template.Cost ?? 0);
                                    slot.UpdateCost(cost);

                                    _context.AppointmentSlots.Add(slot);
                                    createdSlots++;
                                }
                            }
                            else
                            {
                                // Check if generic slot already exists
                                if (existingSlotMap.TryGetValue(new { Date = currentDate, OphthalId = (Guid?)null }, out var genericTimes) 
                                    && genericTimes.Contains(slotStart))
                                    continue;

                                _context.AppointmentSlots.Add(new AppointmentSlot(
                                    template.Id,
                                    currentDate,
                                    slotStart,
                                    slotEnd,
                                    template.MaxCapacity));

                                createdSlots++;
                            }
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
