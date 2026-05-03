using Application.SystemSettings.Interfaces;
using Domain.Entities.Scheduling;
using Domain.Entities.Users;
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

        var templates = await _context.ScheduleTemplates
            .Where(t => t.IsActive && !t.IsDeleted)
            .ToListAsync(cancellationToken);

        var doctors = await _context.Ophthalmologists
            .Where(o => !o.IsDeleted)
            .ToListAsync(cancellationToken);

        var stats = new GenerationStats();

        foreach (var template in templates)
        {
            if (!IsTemplateValid(template))
            {
                stats.SkippedInvalidTemplates++;
                LogInvalidTemplate(template);
                continue;
            }

            try
            {
                var createdCount = await ProcessTemplateAsync(template, fromDate, toDate, doctors, cancellationToken);
                stats.CreatedSlots += createdCount;
            }
            catch (Exception ex)
            {
                stats.SkippedInvalidTemplates++;
                _logger.LogError(ex, "Error generating slots for template {TemplateId}", template.Id);
            }
        }

        if (stats.CreatedSlots > 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation(
            "Completed clinic slot rolling-window generation. TemplatesProcessed={TemplateCount}, SlotsCreated={SlotsCreated}, SkippedInvalidTemplates={SkippedInvalidTemplates}",
            templates.Count, stats.CreatedSlots, stats.SkippedInvalidTemplates);
    }

    private static bool IsTemplateValid(ScheduleTemplate template)
    {
        return template.SlotDuration > 0 
            && template.EndTime > template.StartTime 
            && template.MaxCapacity > 0;
    }

    private void LogInvalidTemplate(ScheduleTemplate template)
    {
        _logger.LogWarning(
            "Skipping invalid template {TemplateId}. SlotDuration={SlotDuration}, StartTime={StartTime}, EndTime={EndTime}, MaxCapacity={MaxCapacity}",
            template.Id, template.SlotDuration, template.StartTime, template.EndTime, template.MaxCapacity);
    }

    private async Task<int> ProcessTemplateAsync(
        ScheduleTemplate template, 
        DateOnly fromDate, 
        DateOnly toDate, 
        List<Ophthalmologist> doctors,
        CancellationToken cancellationToken)
    {
        var existingSlots = await _context.AppointmentSlots
            .Where(s => s.ScheduleTemplateId == template.Id && !s.IsDeleted)
            .Where(s => s.Date >= fromDate && s.Date <= toDate)
            .Select(s => new { s.Date, s.StartTime, s.OphthalId })
            .ToListAsync(cancellationToken);

        var existingSlotMap = existingSlots
            .GroupBy(s => (s.Date, s.OphthalId))
            .ToDictionary(g => g.Key, g => g.Select(s => s.StartTime).ToHashSet());

        var slotDuration = TimeSpan.FromMinutes(template.SlotDuration);
        var templateStart = template.StartTime.ToTimeSpan();
        var templateEnd = template.EndTime.ToTimeSpan();
        var totalCreated = 0;

        for (var currentDate = fromDate; currentDate <= toDate; currentDate = currentDate.AddDays(1))
        {
            if (currentDate.DayOfWeek != template.DayOfWeek) continue;

            for (var currentStart = templateStart; currentStart + slotDuration <= templateEnd; currentStart += slotDuration)
            {
                var slotStart = TimeOnly.FromTimeSpan(currentStart);
                var slotEnd = TimeOnly.FromTimeSpan(currentStart + slotDuration);

                if (doctors.Any())
                {
                    totalCreated += await CreateDoctorSlotsAsync(template, currentDate, slotStart, slotEnd, doctors, existingSlotMap, cancellationToken);
                }
                else
                {
                    totalCreated += CreateGenericSlot(template, currentDate, slotStart, slotEnd, existingSlotMap);
                }
            }
        }

        return totalCreated;
    }

    private async Task<int> CreateDoctorSlotsAsync(
        ScheduleTemplate template,
        DateOnly date,
        TimeOnly slotStart,
        TimeOnly slotEnd,
        List<Ophthalmologist> doctors,
        Dictionary<(DateOnly Date, Guid? OphthalId), HashSet<TimeOnly>> existingSlotMap,
        CancellationToken cancellationToken)
    {
        var createdCount = 0;
        foreach (var doctor in doctors)
        {
            if (HasExistingSlot(existingSlotMap, date, doctor.Id, slotStart)) continue;

            await TryCleanupGenericSlotAsync(template, date, slotStart, existingSlotMap, cancellationToken);

            var slot = new AppointmentSlot(template.Id, date, slotStart, slotEnd, 1);
            slot.UpdateOphthalId(doctor.Id);
            slot.UpdateCost(doctor.ConsultationFee > 0 ? doctor.ConsultationFee : (template.Cost ?? 0));

            _context.AppointmentSlots.Add(slot);
            createdCount++;
        }
        return createdCount;
    }

    private int CreateGenericSlot(
        ScheduleTemplate template,
        DateOnly date,
        TimeOnly slotStart,
        TimeOnly slotEnd,
        Dictionary<(DateOnly Date, Guid? OphthalId), HashSet<TimeOnly>> existingSlotMap)
    {
        if (HasExistingSlot(existingSlotMap, date, null, slotStart)) return 0;

        _context.AppointmentSlots.Add(new AppointmentSlot(template.Id, date, slotStart, slotEnd, template.MaxCapacity));
        return 1;
    }

    private static bool HasExistingSlot(Dictionary<(DateOnly Date, Guid? OphthalId), HashSet<TimeOnly>> map, DateOnly date, Guid? ophthalId, TimeOnly startTime)
    {
        return map.TryGetValue((date, ophthalId), out var times) && times.Contains(startTime);
    }

    private async Task TryCleanupGenericSlotAsync(
        ScheduleTemplate template,
        DateOnly date,
        TimeOnly slotStart,
        Dictionary<(DateOnly Date, Guid? OphthalId), HashSet<TimeOnly>> existingSlotMap,
        CancellationToken cancellationToken)
    {
        if (HasExistingSlot(existingSlotMap, date, null, slotStart))
        {
            var genericSlot = await _context.AppointmentSlots
                .FirstOrDefaultAsync(s => s.ScheduleTemplateId == template.Id 
                    && s.Date == date && s.StartTime == slotStart 
                    && s.OphthalId == null && !s.IsDeleted, cancellationToken);

            if (genericSlot != null)
            {
                _context.AppointmentSlots.Remove(genericSlot);
            }

            if (existingSlotMap.TryGetValue((date, null), out var genericTimes))
            {
                genericTimes.Remove(slotStart);
            }
        }
    }

    private class GenerationStats
    {
        public int CreatedSlots { get; set; }
        public int SkippedInvalidTemplates { get; set; }
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
