using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Scheduling.AppointmentSlots.Commands.GenerateSlots;

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

        var slotsByDate = new Dictionary<DateOnly, List<(TimeOnly Start, TimeOnly End)>>();
        var currentDate = request.FromDate;

        while (currentDate <= request.ToDate)
        {
            // Only generate slots for the template's day of week
            if (currentDate.DayOfWeek == template.DayOfWeek)
            {
                // Skip if date already has slots (and SkipExistingDates is true)
                if (!existingDates.Contains(currentDate))
                {
                    var slotStart = template.StartTime;
                    var daySlots = new List<(TimeOnly Start, TimeOnly End)>();
                    while (slotStart.Add(TimeSpan.FromMinutes(template.SlotDuration)) <= template.EndTime)
                    {
                        var slotEnd = slotStart.Add(TimeSpan.FromMinutes(template.SlotDuration));
                        daySlots.Add((slotStart, slotEnd));
                        slotStart = slotEnd;
                    }

                    if (daySlots.Count > 0)
                    {
                        slotsByDate[currentDate] = daySlots;
                    }
                }
            }

            currentDate = currentDate.AddDays(1);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var slotsCreated = 0;

            foreach (var kvp in slotsByDate.OrderBy(x => x.Key))
            {
                foreach (var timeWindow in kvp.Value)
                {
                    var slot = new AppointmentSlot(
                        template.Id,
                        kvp.Key,
                        timeWindow.Start,
                        timeWindow.End,
                        template.MaxCapacity,
                        template.Cost,
                        SlotSource.System);

                    await _appointmentSlotRepository.AddAsync(slot, cancellationToken);
                    slotsCreated++;
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Generated {SlotsCreated} slots for template {TemplateId} from {FromDate} to {ToDate}",
                slotsCreated,
                request.ScheduleTemplateId,
                request.FromDate,
                request.ToDate);

            return Result<int>.Success(slotsCreated);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(
                ex,
                "Error generating slots for template {TemplateId} from {FromDate} to {ToDate}",
                request.ScheduleTemplateId,
                request.FromDate,
                request.ToDate);
            throw;
        }
    }
}
