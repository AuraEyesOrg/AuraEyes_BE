using Application.Scheduling.ScheduleTemplates.Interfaces;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Provisions baseline weekly schedule templates and rolling-window slots
/// for full-time ophthalmologists.
/// </summary>
public class FullTimeTemplateProvisioningService : IFullTimeTemplateProvisioningService
{
    private const int DefaultSlotDurationMinutes = 30;
    private const int DefaultMaxCapacity = 1;

    private static readonly (DayOfWeek Day, TimeOnly Start, TimeOnly End)[] BaselineWindows =
    {
        (DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(12, 0)),
        (DayOfWeek.Monday, new TimeOnly(13, 0), new TimeOnly(17, 0)),
        (DayOfWeek.Tuesday, new TimeOnly(8, 0), new TimeOnly(12, 0)),
        (DayOfWeek.Tuesday, new TimeOnly(13, 0), new TimeOnly(17, 0)),
        (DayOfWeek.Wednesday, new TimeOnly(8, 0), new TimeOnly(12, 0)),
        (DayOfWeek.Wednesday, new TimeOnly(13, 0), new TimeOnly(17, 0)),
        (DayOfWeek.Thursday, new TimeOnly(8, 0), new TimeOnly(12, 0)),
        (DayOfWeek.Thursday, new TimeOnly(13, 0), new TimeOnly(17, 0)),
        (DayOfWeek.Friday, new TimeOnly(8, 0), new TimeOnly(12, 0)),
        (DayOfWeek.Friday, new TimeOnly(13, 0), new TimeOnly(17, 0))
    };

    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IScheduleTemplateRepository _scheduleTemplateRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FullTimeTemplateProvisioningService> _logger;

    public FullTimeTemplateProvisioningService(
        IOphthalmologistRepository ophthalmologistRepository,
        IScheduleTemplateRepository scheduleTemplateRepository,
        IAppointmentSlotRepository appointmentSlotRepository,
        IUnitOfWork unitOfWork,
        ILogger<FullTimeTemplateProvisioningService> logger)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _scheduleTemplateRepository = scheduleTemplateRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<int> EnsureSystemGeneratedTemplatesAsync(
        Ophthalmologist ophthalmologist,
        CancellationToken cancellationToken = default)
    {
        if (ophthalmologist.EmploymentType != OphthalmologistEmploymentType.FullTime)
        {
            return 0;
        }

        var created = 0;

        foreach (var window in BaselineWindows)
        {
            var hasOverlap = await _scheduleTemplateRepository.HasOverlappingTemplateAsync(
                ophthalId: ophthalmologist.Id,
                orgId: null,
                dayOfWeek: window.Day,
                startTime: window.Start,
                endTime: window.End,
                cancellationToken: cancellationToken);

            if (hasOverlap)
            {
                continue;
            }

            var template = new ScheduleTemplate(
                dayOfWeek: window.Day,
                startTime: window.Start,
                endTime: window.End,
                slotDuration: DefaultSlotDurationMinutes,
                maxCapacity: DefaultMaxCapacity,
                orgId: null,
                ophthalId: ophthalmologist.Id,
                cost: null);

            await _scheduleTemplateRepository.AddAsync(template, cancellationToken);
            created++;
        }

        if (created > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return created;
    }

    public async Task<int> EnsureFutureSlotsForFullTimeAsync(
        int rollingWindowDays,
        CancellationToken cancellationToken = default)
    {
        if (rollingWindowDays < 1)
        {
            rollingWindowDays = 14;
        }

        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(rollingWindowDays);

        var fullTimeDoctors = await _ophthalmologistRepository.Query()
            .Where(o => o.EmploymentType == OphthalmologistEmploymentType.FullTime)
            .Select(o => new { o.Id })
            .ToListAsync(cancellationToken);

        if (fullTimeDoctors.Count == 0)
        {
            return 0;
        }

        var totalCreated = 0;

        foreach (var doctor in fullTimeDoctors)
        {
            var templates = await _scheduleTemplateRepository.GetByOphthalmologistIdAsync(
                doctor.Id,
                cancellationToken);

            foreach (var template in templates)
            {
                var existingSlots = await _appointmentSlotRepository.GetByDateRangeAsync(
                    template.Id,
                    startDate,
                    endDate,
                    cancellationToken);

                var existingDates = existingSlots
                    .Select(s => s.Date)
                    .ToHashSet();

                var currentDate = startDate;
                while (currentDate <= endDate)
                {
                    if (currentDate.DayOfWeek == template.DayOfWeek && !existingDates.Contains(currentDate))
                    {
                        var slotStart = template.StartTime;
                        var slotSpan = TimeSpan.FromMinutes(template.SlotDuration);

                        while (slotStart.Add(slotSpan) <= template.EndTime)
                        {
                            var slotEnd = slotStart.Add(slotSpan);

                            var slot = new AppointmentSlot(
                                scheduleTemplateId: template.Id,
                                date: currentDate,
                                startTime: slotStart,
                                endTime: slotEnd,
                                maxCapacity: template.MaxCapacity,
                                cost: template.Cost);

                            await _appointmentSlotRepository.AddAsync(slot, cancellationToken);
                            totalCreated++;
                            slotStart = slotEnd;
                        }
                    }

                    currentDate = currentDate.AddDays(1);
                }
            }
        }

        if (totalCreated > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation(
            "Full-time slot provisioning completed. Created {CreatedSlotCount} slots over {RollingWindowDays} days",
            totalCreated,
            rollingWindowDays);

        return totalCreated;
    }
}
