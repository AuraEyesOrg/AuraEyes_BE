using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Scheduling.AppointmentSlots.Commands.GenerateSlots;

/// <summary>
/// Handler for GenerateSlotsCommand.
/// Generates appointment slots from a schedule template for a given date range.
/// </summary>
public class GenerateSlotsCommandHandler : ICommandHandler<GenerateSlotsCommand, int>
{
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IScheduleTemplateRepository _scheduleTemplateRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GenerateSlotsCommandHandler> _logger;

    public GenerateSlotsCommandHandler(
        IAppointmentSlotRepository appointmentSlotRepository,
        IScheduleTemplateRepository scheduleTemplateRepository,
        IUnitOfWork unitOfWork,
        ILogger<GenerateSlotsCommandHandler> logger)
    {
        _appointmentSlotRepository = appointmentSlotRepository;
        _scheduleTemplateRepository = scheduleTemplateRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<int>> Handle(GenerateSlotsCommand request, CancellationToken cancellationToken)
    {
        // Validate date range
        if (request.ToDate < request.FromDate)
        {
            return Result<int>.Failure("ToDate must be greater than or equal to FromDate.");
        }

        // Limit date range to prevent excessive slot creation
        var maxDays = 90;
        if (request.ToDate.DayNumber - request.FromDate.DayNumber > maxDays)
        {
            return Result<int>.Failure($"Date range cannot exceed {maxDays} days.");
        }

        // Get the template
        var template = await _scheduleTemplateRepository.GetByIdAsync(request.ScheduleTemplateId, cancellationToken);
        if (template is null)
        {
            return Result<int>.NotFound($"Schedule template '{request.ScheduleTemplateId}' not found.");
        }

        // Get existing slots if we need to skip existing dates
        HashSet<DateOnly> existingDates = new();
        if (request.SkipExistingDates)
        {
            var existingSlots = await _appointmentSlotRepository.GetByDateRangeAsync(
                request.ScheduleTemplateId,
                request.FromDate,
                request.ToDate,
                cancellationToken);
            existingDates = existingSlots.Select(s => s.Date).ToHashSet();
        }

        // Generate slots
        var slotsCreated = 0;
        var currentDate = request.FromDate;

        while (currentDate <= request.ToDate)
        {
            // Only generate slots for the template's day of week
            if (currentDate.DayOfWeek == template.DayOfWeek)
            {
                // Skip if date already has slots (and SkipExistingDates is true)
                if (!existingDates.Contains(currentDate))
                {
                    // Generate slots for this day
                    var slotStart = template.StartTime;
                    while (slotStart.Add(TimeSpan.FromMinutes(template.SlotDuration)) <= template.EndTime)
                    {
                        var slotEnd = slotStart.Add(TimeSpan.FromMinutes(template.SlotDuration));

                        var slot = new AppointmentSlot(
                            template.Id,
                            currentDate,
                            slotStart,
                            slotEnd,
                            template.MaxCapacity,
                            template.Cost);

                        await _appointmentSlotRepository.AddAsync(slot, cancellationToken);
                        slotsCreated++;

                        slotStart = slotEnd;
                    }
                }
            }

            currentDate = currentDate.AddDays(1);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Generated {SlotsCreated} slots for template {TemplateId} from {FromDate} to {ToDate}",
            slotsCreated, request.ScheduleTemplateId, request.FromDate, request.ToDate);

        return Result<int>.Success(slotsCreated);
    }
}
