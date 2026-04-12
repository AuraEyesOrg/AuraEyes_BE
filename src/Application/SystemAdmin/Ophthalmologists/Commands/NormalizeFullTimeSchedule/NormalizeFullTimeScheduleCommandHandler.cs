using Application.Common.Models;
using Application.Scheduling.ScheduleTemplates.Interfaces;
using Application.SystemSettings.Interfaces;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Ophthalmologists.Commands.NormalizeFullTimeSchedule;

/// <summary>
/// Normalizes one full-time ophthalmologist schedule into canonical weekday templates and reconciles future slots.
/// </summary>
public class NormalizeFullTimeScheduleCommandHandler : IRequestHandler<NormalizeFullTimeScheduleCommand, Result<NormalizeFullTimeScheduleResultDto>>
{
    private const string FullTimeSlotWindowDaysSettingKey = "FULLTIME_SLOT_WINDOW_DAYS";
    private const int DefaultWindowDays = 30;
    private const int MaxAllowedWindowDays = 180;

    private const int CanonicalSlotDurationMinutes = 30;
    private const int CanonicalMaxCapacity = 1;
    private static readonly TimeOnly CanonicalStartTime = new(8, 0);
    private static readonly TimeOnly CanonicalEndTime = new(17, 0);
    private static readonly DayOfWeek[] CanonicalWeekdays =
    {
        DayOfWeek.Monday,
        DayOfWeek.Tuesday,
        DayOfWeek.Wednesday,
        DayOfWeek.Thursday,
        DayOfWeek.Friday
    };

    private static readonly HashSet<ScheduleStatus> DeletableStatuses = new()
    {
        ScheduleStatus.Available,
        ScheduleStatus.Blocked,
        ScheduleStatus.Expired
    };

    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IScheduleTemplateRepository _scheduleTemplateRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IFullTimeTemplateProvisioningService _fullTimeTemplateProvisioningService;
    private readonly ISystemSettingService _settingService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NormalizeFullTimeScheduleCommandHandler> _logger;

    public NormalizeFullTimeScheduleCommandHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        IScheduleTemplateRepository scheduleTemplateRepository,
        IAppointmentSlotRepository appointmentSlotRepository,
        IFullTimeTemplateProvisioningService fullTimeTemplateProvisioningService,
        ISystemSettingService settingService,
        IUnitOfWork unitOfWork,
        ILogger<NormalizeFullTimeScheduleCommandHandler> logger)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _scheduleTemplateRepository = scheduleTemplateRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
        _fullTimeTemplateProvisioningService = fullTimeTemplateProvisioningService;
        _settingService = settingService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<NormalizeFullTimeScheduleResultDto>> Handle(
        NormalizeFullTimeScheduleCommand request,
        CancellationToken cancellationToken)
    {
        if (request.OphthalmologistId == Guid.Empty)
        {
            return Result<NormalizeFullTimeScheduleResultDto>.Failure("Ophthalmologist ID is required.");
        }

        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(request.OphthalmologistId, cancellationToken);
        if (ophthalmologist is null)
        {
            return Result<NormalizeFullTimeScheduleResultDto>.NotFound(
                $"Ophthalmologist '{request.OphthalmologistId}' was not found.");
        }

        if (ophthalmologist.EmploymentType != OphthalmologistEmploymentType.FullTime)
        {
            return Result<NormalizeFullTimeScheduleResultDto>.Conflict(
                "Normalize is only supported for FullTime ophthalmologists.");
        }

        var windowDays = await ResolveWindowDaysAsync(request.WindowDays, cancellationToken);
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var toDate = fromDate.AddDays(windowDays - 1);

        var templatesEnsured = await _fullTimeTemplateProvisioningService.EnsureSystemGeneratedTemplatesAsync(
            ophthalmologist,
            cancellationToken);

        var activeTemplates = await _scheduleTemplateRepository.GetByOphthalmologistIdAsync(
            ophthalmologist.Id,
            cancellationToken);

        var canonicalTemplatesByDay = CanonicalWeekdays
            .Select(day => new
            {
                Day = day,
                Template = activeTemplates
                    .Where(t => t.Source == ScheduleTemplateSource.SystemGenerated && t.DayOfWeek == day)
                    .OrderBy(t => t.CreatedAt)
                    .FirstOrDefault()
            })
            .ToDictionary(x => x.Day, x => x.Template);

        if (canonicalTemplatesByDay.Any(x => x.Value is null))
        {
            return Result<NormalizeFullTimeScheduleResultDto>.Failure(
                "Unable to resolve canonical system-generated templates for all weekdays (Mon-Fri).");
        }

        var canonicalTemplates = canonicalTemplatesByDay.Values
            .Where(t => t is not null)
            .Cast<ScheduleTemplate>()
            .ToList();

        var canonicalTemplateIds = canonicalTemplates
            .Select(t => t.Id)
            .ToHashSet();

        var templatesUpdated = 0;
        foreach (var template in canonicalTemplates)
        {
            if (template.StartTime != CanonicalStartTime
                || template.EndTime != CanonicalEndTime
                || template.SlotDuration != CanonicalSlotDurationMinutes
                || template.MaxCapacity != CanonicalMaxCapacity)
            {
                template.Update(
                    template.DayOfWeek,
                    CanonicalStartTime,
                    CanonicalEndTime,
                    CanonicalSlotDurationMinutes,
                    CanonicalMaxCapacity,
                    template.Cost);

                await _scheduleTemplateRepository.UpdateAsync(template, cancellationToken);
                templatesUpdated++;
            }
        }

        var templatesToDeactivate = activeTemplates
            .Where(t => !canonicalTemplateIds.Contains(t.Id))
            .ToList();

        foreach (var template in templatesToDeactivate)
        {
            template.Deactivate();
            await _scheduleTemplateRepository.UpdateAsync(template, cancellationToken);
        }

        var doctorSlotsInWindow = (await _appointmentSlotRepository.GetByOphthalmologistAsync(
            ophthalmologist.Id,
            fromDate,
            toDate,
            null,
            cancellationToken)).ToList();

        var slotsDeleted = 0;
        var slotsCreated = 0;
        var slotsProtected = 0;

        var deactivatedTemplateIds = templatesToDeactivate.Select(t => t.Id).ToHashSet();
        if (deactivatedTemplateIds.Count > 0)
        {
            var deactivatedTemplateSlots = doctorSlotsInWindow
                .Where(s => deactivatedTemplateIds.Contains(s.ScheduleTemplateId))
                .ToList();

            foreach (var slot in deactivatedTemplateSlots)
            {
                if (DeletableStatuses.Contains(slot.Status))
                {
                    await _appointmentSlotRepository.DeleteAsync(slot, cancellationToken);
                    doctorSlotsInWindow.Remove(slot);
                    slotsDeleted++;
                }
                else
                {
                    slotsProtected++;
                }
            }
        }

        foreach (var template in canonicalTemplates)
        {
            var expectedWindows = BuildExpectedWindows(template);
            if (expectedWindows.Count == 0)
            {
                continue;
            }

            var currentDate = fromDate;
            while (currentDate <= toDate)
            {
                if (currentDate.DayOfWeek != template.DayOfWeek)
                {
                    currentDate = currentDate.AddDays(1);
                    continue;
                }

                var slotsForTemplateDate = doctorSlotsInWindow
                    .Where(s => s.ScheduleTemplateId == template.Id && s.Date == currentDate)
                    .ToList();

                var expectedWindowKeys = expectedWindows
                    .Select(w => GetWindowKey(w.Start, w.End))
                    .ToHashSet();

                foreach (var slot in slotsForTemplateDate)
                {
                    var key = GetWindowKey(slot.StartTime, slot.EndTime);
                    if (expectedWindowKeys.Contains(key))
                    {
                        continue;
                    }

                    if (DeletableStatuses.Contains(slot.Status))
                    {
                        await _appointmentSlotRepository.DeleteAsync(slot, cancellationToken);
                        doctorSlotsInWindow.Remove(slot);
                        slotsDeleted++;
                    }
                    else
                    {
                        slotsProtected++;
                    }
                }

                foreach (var window in expectedWindows)
                {
                    var exactExists = doctorSlotsInWindow.Any(s =>
                        s.ScheduleTemplateId == template.Id
                        && s.Date == currentDate
                        && s.StartTime == window.Start
                        && s.EndTime == window.End);

                    if (exactExists)
                    {
                        continue;
                    }

                    var overlapsExisting = doctorSlotsInWindow.Any(s =>
                        s.Date == currentDate
                        && IsOverlapping(window.Start, window.End, s.StartTime, s.EndTime));

                    if (overlapsExisting)
                    {
                        slotsProtected++;
                        continue;
                    }

                    var slot = new AppointmentSlot(
                        template.Id,
                        currentDate,
                        window.Start,
                        window.End,
                        template.MaxCapacity,
                        template.Cost,
                        SlotSource.System);

                    await _appointmentSlotRepository.AddAsync(slot, cancellationToken);
                    doctorSlotsInWindow.Add(slot);
                    slotsCreated++;
                }

                currentDate = currentDate.AddDays(1);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Normalized full-time schedule for ophthalmologist {OphthalmologistId}. TemplatesEnsured={TemplatesEnsured}, CanonicalTemplates={CanonicalTemplates}, TemplatesUpdated={TemplatesUpdated}, TemplatesDeactivated={TemplatesDeactivated}, SlotsDeleted={SlotsDeleted}, SlotsCreated={SlotsCreated}, SlotsProtected={SlotsProtected}, FromDate={FromDate}, ToDate={ToDate}",
            ophthalmologist.Id,
            templatesEnsured,
            canonicalTemplates.Count,
            templatesUpdated,
            templatesToDeactivate.Count,
            slotsDeleted,
            slotsCreated,
            slotsProtected,
            fromDate,
            toDate);

        return Result<NormalizeFullTimeScheduleResultDto>.Success(new NormalizeFullTimeScheduleResultDto
        {
            OphthalmologistId = ophthalmologist.Id,
            FromDate = fromDate,
            ToDate = toDate,
            WindowDays = windowDays,
            TemplatesEnsured = templatesEnsured,
            CanonicalTemplates = canonicalTemplates.Count,
            TemplatesUpdated = templatesUpdated,
            TemplatesDeactivated = templatesToDeactivate.Count,
            SlotsDeleted = slotsDeleted,
            SlotsCreated = slotsCreated,
            SlotsProtected = slotsProtected
        });
    }

    private static List<(TimeOnly Start, TimeOnly End)> BuildExpectedWindows(ScheduleTemplate template)
    {
        var windows = new List<(TimeOnly Start, TimeOnly End)>();
        var duration = TimeSpan.FromMinutes(template.SlotDuration);
        var currentStart = template.StartTime.ToTimeSpan();
        var end = template.EndTime.ToTimeSpan();

        while (currentStart + duration <= end)
        {
            var currentEnd = currentStart + duration;
            windows.Add((TimeOnly.FromTimeSpan(currentStart), TimeOnly.FromTimeSpan(currentEnd)));
            currentStart = currentEnd;
        }

        return windows;
    }

    private static bool IsOverlapping(
        TimeOnly startA,
        TimeOnly endA,
        TimeOnly startB,
        TimeOnly endB)
    {
        return startA < endB && endA > startB;
    }

    private static string GetWindowKey(TimeOnly start, TimeOnly end)
    {
        return $"{start:HH\\:mm}-{end:HH\\:mm}";
    }

    private async Task<int> ResolveWindowDaysAsync(int? requestedWindowDays, CancellationToken cancellationToken)
    {
        if (requestedWindowDays.HasValue)
        {
            if (requestedWindowDays.Value < 1 || requestedWindowDays.Value > MaxAllowedWindowDays)
            {
                return DefaultWindowDays;
            }

            return requestedWindowDays.Value;
        }

        var envValue = Environment.GetEnvironmentVariable(FullTimeSlotWindowDaysSettingKey);
        if (int.TryParse(envValue, out var envDays) && envDays > 0)
        {
            return Math.Min(envDays, MaxAllowedWindowDays);
        }

        var configuredValue = await _settingService.GetSettingAsync(FullTimeSlotWindowDaysSettingKey, cancellationToken);
        if (int.TryParse(configuredValue, out var configuredDays) && configuredDays > 0)
        {
            return Math.Min(configuredDays, MaxAllowedWindowDays);
        }

        return DefaultWindowDays;
    }
}
