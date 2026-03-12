using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Ophthalmologists.Schedules.Commands.BookSchedule;

/// <summary>
/// Handler for BookScheduleCommand.
/// Loads the schedule with its AvailableSlot and validates capacity before booking.
/// </summary>
public class BookScheduleCommandHandler : ICommandHandler<BookScheduleCommand>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BookScheduleCommandHandler> _logger;

    public BookScheduleCommandHandler(
        IScheduleRepository scheduleRepository,
        IUnitOfWork unitOfWork,
        ILogger<BookScheduleCommandHandler> logger)
    {
        _scheduleRepository = scheduleRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(BookScheduleCommand request, CancellationToken cancellationToken)
    {
        // Load schedule with AvailableSlot to perform capacity check in one query
        var schedule = await _scheduleRepository.GetByIdWithSlotAsync(request.ScheduleId, cancellationToken);
        if (schedule is null)
            return Result.NotFound($"Schedule '{request.ScheduleId}' not found.");

        if (schedule.PatientId != request.PatientId)
            return Result.Forbidden("You are not authorized to book this schedule.");

        if (schedule.Status != ScheduleStatus.Available)
            return Result.Failure($"Schedule is not available for booking. Current status: {schedule.Status}.");

        if (schedule.AvailableSlot is null)
            return Result.NotFound($"AvailableSlot for schedule '{request.ScheduleId}' not found.");

        // Count active bookings against this slot and compare to capacity
        var activeCount = await _scheduleRepository.GetActiveCountByAvailableSlotAsync(
            schedule.AvailableSlotId, cancellationToken);

        if (activeCount >= schedule.AvailableSlot.MaxCapacity)
            return Result.Conflict("This time slot is fully booked. No capacity remaining.");

        try
        {
            schedule.Book();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Schedule {ScheduleId} booked by Patient {PatientId}",
            request.ScheduleId, request.PatientId);

        return Result.Success();
    }
}
