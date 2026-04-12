using Application.Common.Models;
using Application.Scheduling.ScheduleTemplates.Interfaces;
using Application.SystemSettings.Interfaces;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Ophthalmologists.Commands.BackfillFullTimeSchedule;

/// <summary>
/// Ensures full-time system templates and generates missing slots for one ophthalmologist.
/// </summary>
public class BackfillFullTimeScheduleCommandHandler : IRequestHandler<BackfillFullTimeScheduleCommand, Result<BackfillFullTimeScheduleResultDto>>
{
    private const string FullTimeSlotWindowDaysSettingKey = "FULLTIME_SLOT_WINDOW_DAYS";
    private const int DefaultWindowDays = 30;
    private const int MaxAllowedWindowDays = 180;

    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IFullTimeTemplateProvisioningService _fullTimeTemplateProvisioningService;
    private readonly IScheduleTemplateRepository _scheduleTemplateRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly ISystemSettingService _settingService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BackfillFullTimeScheduleCommandHandler> _logger;

    public BackfillFullTimeScheduleCommandHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        IFullTimeTemplateProvisioningService fullTimeTemplateProvisioningService,
        IScheduleTemplateRepository scheduleTemplateRepository,
        IAppointmentSlotRepository appointmentSlotRepository,
        ISystemSettingService settingService,
        IUnitOfWork unitOfWork,
        ILogger<BackfillFullTimeScheduleCommandHandler> logger)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _fullTimeTemplateProvisioningService = fullTimeTemplateProvisioningService;
        _scheduleTemplateRepository = scheduleTemplateRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
        _settingService = settingService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<BackfillFullTimeScheduleResultDto>> Handle(
        BackfillFullTimeScheduleCommand request,
        CancellationToken cancellationToken)
    {
        if (request.OphthalmologistId == Guid.Empty)
        {
            return Result<BackfillFullTimeScheduleResultDto>.Failure("Ophthalmologist ID is required.");
        }

        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(request.OphthalmologistId, cancellationToken);
        if (ophthalmologist is null)
        {
            return Result<BackfillFullTimeScheduleResultDto>.NotFound(
                $"Ophthalmologist '{request.OphthalmologistId}' was not found.");
        }

        if (ophthalmologist.EmploymentType != OphthalmologistEmploymentType.FullTime)
        {
            return Result<BackfillFullTimeScheduleResultDto>.Conflict(
                "Backfill is only supported for FullTime ophthalmologists.");
        }

        var windowDays = await ResolveWindowDaysAsync(request.WindowDays, cancellationToken);
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var toDate = fromDate.AddDays(windowDays - 1);

        var templatesEnsured = await _fullTimeTemplateProvisioningService.EnsureSystemGeneratedTemplatesAsync(
            ophthalmologist,
            cancellationToken);

        var templates = await _scheduleTemplateRepository.GetActiveSystemGeneratedByOphthalmologistIdAsync(
            ophthalmologist.Id,
            cancellationToken);

        var slotsCreated = 0;
        var skippedInvalidTemplates = 0;

        foreach (var template in templates)
        {
            if (!IsTemplateValid(template))
            {
                skippedInvalidTemplates++;
                _logger.LogWarning(
                    "Skipping invalid template {TemplateId} during backfill for ophthalmologist {OphthalmologistId}.",
                    template.Id,
                    ophthalmologist.Id);
                continue;
            }

            var existingSlots = await _appointmentSlotRepository.GetByDateRangeAsync(
                template.Id,
                fromDate,
                toDate,
                cancellationToken);

            var existingDates = existingSlots.Select(slot => slot.Date).ToHashSet();

            var slotDuration = TimeSpan.FromMinutes(template.SlotDuration);
            var startSpan = template.StartTime.ToTimeSpan();
            var endSpan = template.EndTime.ToTimeSpan();

            var currentDate = fromDate;
            while (currentDate <= toDate)
            {
                if (currentDate.DayOfWeek == template.DayOfWeek && !existingDates.Contains(currentDate))
                {
                    for (var slotStart = startSpan; slotStart + slotDuration <= endSpan; slotStart += slotDuration)
                    {
                        var slotEnd = slotStart + slotDuration;
                        var slot = new AppointmentSlot(
                            template.Id,
                            currentDate,
                            TimeOnly.FromTimeSpan(slotStart),
                            TimeOnly.FromTimeSpan(slotEnd),
                            template.MaxCapacity,
                            template.Cost,
                            SlotSource.System);

                        await _appointmentSlotRepository.AddAsync(slot, cancellationToken);
                        slotsCreated++;
                    }
                }

                currentDate = currentDate.AddDays(1);
            }
        }

        if (slotsCreated > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation(
            "Backfill full-time schedule completed for ophthalmologist {OphthalmologistId}. TemplatesEnsured={TemplatesEnsured}, ActiveTemplates={ActiveTemplates}, SlotsCreated={SlotsCreated}, SkippedInvalidTemplates={SkippedInvalidTemplates}, FromDate={FromDate}, ToDate={ToDate}",
            ophthalmologist.Id,
            templatesEnsured,
            templates.Count,
            slotsCreated,
            skippedInvalidTemplates,
            fromDate,
            toDate);

        return Result<BackfillFullTimeScheduleResultDto>.Success(new BackfillFullTimeScheduleResultDto
        {
            OphthalmologistId = ophthalmologist.Id,
            FromDate = fromDate,
            ToDate = toDate,
            WindowDays = windowDays,
            TemplatesEnsured = templatesEnsured,
            ActiveSystemTemplates = templates.Count,
            SlotsCreated = slotsCreated,
            SkippedInvalidTemplates = skippedInvalidTemplates
        });
    }

    private static bool IsTemplateValid(ScheduleTemplate template)
    {
        return template.SlotDuration > 0
            && template.MaxCapacity > 0
            && template.EndTime > template.StartTime;
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
