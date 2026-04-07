using Application.Common.Constants;
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

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var windowDays = await GetWindowDaysAsync(cancellationToken);
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var toDate = fromDate.AddDays(windowDays - 1);

        _logger.LogInformation(
            "Starting full-time slot rolling-window generation. FromDate={FromDate}, ToDate={ToDate}, WindowDays={WindowDays}",
            fromDate,
            toDate,
            windowDays);

        var templates = await (
            from template in _context.ScheduleTemplates
            where template.OphthalId.HasValue
            join ophthal in _context.Ophthalmologists on template.OphthalId!.Value equals ophthal.Id
            where template.Source == ScheduleTemplateSource.SystemGenerated
                  && ophthal.EmploymentType == OphthalmologistEmploymentType.FullTime
            select template)
            .ToListAsync(cancellationToken);

        var createdSlots = 0;
        foreach (var template in templates)
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
                    var slotStart = template.StartTime;
                    while (slotStart.Add(TimeSpan.FromMinutes(template.SlotDuration)) <= template.EndTime)
                    {
                        var slotEnd = slotStart.Add(TimeSpan.FromMinutes(template.SlotDuration));

                        _context.AppointmentSlots.Add(new AppointmentSlot(
                            template.Id,
                            currentDate,
                            slotStart,
                            slotEnd,
                            template.MaxCapacity,
                            template.Cost,
                            SlotSource.System));

                        createdSlots++;
                        slotStart = slotEnd;
                    }
                }

                currentDate = currentDate.AddDays(1);
            }
        }

        if (createdSlots > 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation(
            "Completed full-time slot rolling-window generation. Templates={TemplateCount}, SlotsCreated={SlotsCreated}",
            templates.Count,
            createdSlots);
    }

    private async Task<int> GetWindowDaysAsync(CancellationToken cancellationToken)
    {
        var envValue = Environment.GetEnvironmentVariable(SystemSettingKeys.FullTimeSlotWindowDays);
        if (int.TryParse(envValue, out var envDays) && envDays > 0)
        {
            return envDays;
        }

        var configured = await _settingService.GetSettingAsync(SystemSettingKeys.FullTimeSlotWindowDays, cancellationToken);
        if (int.TryParse(configured, out var configuredDays) && configuredDays > 0)
        {
            return configuredDays;
        }

        return 30;
    }
}