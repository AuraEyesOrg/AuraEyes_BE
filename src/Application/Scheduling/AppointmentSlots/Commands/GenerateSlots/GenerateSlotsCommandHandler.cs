using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemSettings.Interfaces;
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
    private const int MaxRetryAttempts = 3;

    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IScheduleTemplateRepository _scheduleTemplateRepository;
    private readonly ISystemSettingService _settingService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GenerateSlotsCommandHandler> _logger;

    public GenerateSlotsCommandHandler(
        IAppointmentSlotRepository appointmentSlotRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IScheduleTemplateRepository scheduleTemplateRepository,
        ISystemSettingService settingService,
        IUnitOfWork unitOfWork,
        ILogger<GenerateSlotsCommandHandler> logger)
    {
        _appointmentSlotRepository = appointmentSlotRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _scheduleTemplateRepository = scheduleTemplateRepository;
        _settingService = settingService;
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

        Domain.Entities.Users.Ophthalmologist? ophthalmologist = null;
        if (template.OphthalId.HasValue)
        {
            ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(template.OphthalId.Value, cancellationToken);
            if (ophthalmologist is null)
            {
                return Result<int>.NotFound($"Ophthalmologist '{template.OphthalId.Value}' not found.");
            }

            if (ophthalmologist.EmploymentType == OphthalmologistEmploymentType.FullTime)
            {
                return Result<int>.Forbidden("Full-time ophthalmologists cannot manually create slots.");
            }
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

        // Build slots grouped by date first so quota can be reserved before inserts.
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

        for (var attempt = 1; attempt <= MaxRetryAttempts; attempt++)
        {
            var slotsCreated = 0;
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                if (ophthalmologist?.EmploymentType == OphthalmologistEmploymentType.PartTime)
                {
                    var quota = await GetPartTimeDailyQuotaAsync(cancellationToken);

                    foreach (var kvp in slotsByDate.OrderBy(x => x.Key))
                    {
                        var reserveResult = await _settingService.TryReservePartTimeSlotsAsync(
                            kvp.Key,
                            kvp.Value.Count,
                            quota,
                            cancellationToken);

                        if (!reserveResult.Success)
                        {
                            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                            _logger.LogWarning(
                                "Part-time quota exceeded while generating slots. Date={Date}, Quota={Quota}, CurrentCount={CurrentCount}, Requested={Requested}",
                                kvp.Key,
                                reserveResult.Quota,
                                reserveResult.UsedSlots,
                                kvp.Value.Count);

                            return Result<int>.Conflict("Daily slot quota for part-time doctors has been reached");
                        }

                        var nearLimitThreshold = Math.Max(1, (int)Math.Ceiling(reserveResult.Quota * 0.1));
                        if (reserveResult.RemainingSlots <= nearLimitThreshold)
                        {
                            _logger.LogWarning(
                                "Part-time quota near limit after bulk reserve. Date={Date}, Quota={Quota}, Used={Used}, Remaining={Remaining}",
                                kvp.Key,
                                reserveResult.Quota,
                                reserveResult.UsedSlots,
                                reserveResult.RemainingSlots);
                        }
                        else
                        {
                            _logger.LogInformation(
                                "Reserved part-time quota for bulk generation. Date={Date}, Requested={Requested}, Quota={Quota}, Used={Used}, Remaining={Remaining}",
                                kvp.Key,
                                kvp.Value.Count,
                                reserveResult.Quota,
                                reserveResult.UsedSlots,
                                reserveResult.RemainingSlots);
                        }
                    }
                }

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
                            SlotSource.Doctor);

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
            catch (ConcurrencyException ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                if (attempt == MaxRetryAttempts)
                {
                    return Result<int>.Conflict("Slot generation conflicted with another request. Please retry.");
                }

                _logger.LogWarning(
                    ex,
                    "Retrying slot generation due to concurrency conflict. Attempt {Attempt}/{MaxAttempts}",
                    attempt,
                    MaxRetryAttempts);

                await Task.Delay(TimeSpan.FromMilliseconds(100 * attempt), cancellationToken);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        return Result<int>.Conflict("Slot generation conflicted with another request. Please retry.");
    }

    private async Task<int> GetPartTimeDailyQuotaAsync(CancellationToken cancellationToken)
    {
        var configured = await _settingService.GetSettingAsync(SystemSettingKeys.PartTimeMaxSlotsPerDay, cancellationToken);
        return int.TryParse(configured, out var value) && value > 0 ? value : 100;
    }
}
