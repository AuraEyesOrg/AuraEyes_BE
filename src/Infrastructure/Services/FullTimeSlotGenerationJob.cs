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
        await ExecuteInternalAsync(null, cancellationToken);
    }

    public async Task ExecuteForOphthalmologistAsync(
        Guid ophthalmologistId,
        CancellationToken cancellationToken = default)
    {
        if (ophthalmologistId == Guid.Empty)
        {
            throw new ArgumentException("Ophthalmologist ID is required.", nameof(ophthalmologistId));
        }

        await ExecuteInternalAsync(new[] { ophthalmologistId }, cancellationToken);
    }

    private async Task ExecuteInternalAsync(
        IReadOnlyCollection<Guid>? targetOphthalmologistIds,
        CancellationToken cancellationToken)
    {
        var windowDays = await GetWindowDaysAsync(cancellationToken);
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var toDate = fromDate.AddDays(windowDays - 1);

        IQueryable<Domain.Entities.Users.Ophthalmologist> ophthalmologistQuery = _context.Ophthalmologists
            .Where(ophthal => ophthal.EmploymentType == OphthalmologistEmploymentType.FullTime);

        if (targetOphthalmologistIds is { Count: > 0 })
        {
            ophthalmologistQuery = ophthalmologistQuery
                .Where(ophthal => targetOphthalmologistIds.Contains(ophthal.Id));
        }

        var fullTimeOphthalmologists = await ophthalmologistQuery.ToListAsync(cancellationToken);

        if (fullTimeOphthalmologists.Count == 0)
        {
            _logger.LogInformation(
                "No full-time ophthalmologists matched slot generation scope. Targeted={IsTargeted}, RequestedDoctorCount={RequestedDoctorCount}",
                targetOphthalmologistIds is { Count: > 0 },
                targetOphthalmologistIds?.Count ?? 0);
            return;
        }

        var matchedDoctorIds = fullTimeOphthalmologists.Select(ophthal => ophthal.Id).ToArray();

        var templatesEnsured = 0;
        foreach (var ophthalmologist in fullTimeOphthalmologists)
        {
            templatesEnsured += await _fullTimeTemplateProvisioningService
                .EnsureSystemGeneratedTemplatesAsync(ophthalmologist, cancellationToken);
        }

        _logger.LogInformation(
            "Starting full-time slot rolling-window generation. FromDate={FromDate}, ToDate={ToDate}, WindowDays={WindowDays}, TemplatesEnsured={TemplatesEnsured}, Targeted={IsTargeted}, DoctorCount={DoctorCount}",
            fromDate,
            toDate,
            windowDays,
            templatesEnsured,
            targetOphthalmologistIds is { Count: > 0 },
            fullTimeOphthalmologists.Count);

        var templatesQuery =
            from template in _context.ScheduleTemplates
            where template.OphthalId.HasValue
            join ophthal in _context.Ophthalmologists on template.OphthalId!.Value equals ophthal.Id
            where template.Source == ScheduleTemplateSource.SystemGenerated
                && template.IsActive
                && ophthal.EmploymentType == OphthalmologistEmploymentType.FullTime
            select template;

        if (targetOphthalmologistIds is { Count: > 0 })
        {
            templatesQuery = templatesQuery.Where(template =>
                template.OphthalId.HasValue && matchedDoctorIds.Contains(template.OphthalId.Value));
        }

        var templates = await templatesQuery.ToListAsync(cancellationToken);

        var createdSlots = 0;
        var skippedInvalidTemplates = 0;

        var approvedLeaveRangesByDoctor = await _context.OphthalmologistLeaveRequests
            .Where(x => x.Status == OphthalmologistLeaveRequestStatus.Approved)
            .Where(x => x.StartDate <= toDate && x.EndDate >= fromDate)
            .Where(x => targetOphthalmologistIds == null || matchedDoctorIds.Contains(x.OphthalmologistId))
            .Select(x => new
            {
                x.OphthalmologistId,
                x.StartDate,
                x.EndDate
            })
            .ToListAsync(cancellationToken);

        var leaveDateLookup = approvedLeaveRangesByDoctor
            .GroupBy(x => x.OphthalmologistId)
            .ToDictionary(
                x => x.Key,
                x => x.Select(range => (range.StartDate, range.EndDate)).ToList());

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
                    if (template.OphthalId.HasValue
                        && leaveDateLookup.TryGetValue(template.OphthalId.Value, out var leaveRanges)
                        && leaveRanges.Any(range => currentDate >= range.StartDate && currentDate <= range.EndDate))
                    {
                        currentDate = currentDate.AddDays(1);
                        continue;
                    }

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
            "Completed full-time slot rolling-window generation. Templates={TemplateCount}, TemplatesEnsured={TemplatesEnsured}, SlotsCreated={SlotsCreated}, SkippedInvalidTemplates={SkippedInvalidTemplates}, Targeted={IsTargeted}, DoctorCount={DoctorCount}",
            templates.Count,
            templatesEnsured,
            createdSlots,
            skippedInvalidTemplates,
            targetOphthalmologistIds is { Count: > 0 },
            fullTimeOphthalmologists.Count);
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
