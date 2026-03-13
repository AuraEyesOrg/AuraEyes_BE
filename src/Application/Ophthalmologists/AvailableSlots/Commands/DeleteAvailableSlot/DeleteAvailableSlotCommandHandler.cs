using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Ophthalmologists.AvailableSlots.Commands.DeleteAvailableSlot;

/// <summary>
/// Handler for DeleteAvailableSlotCommand.
/// </summary>
public class DeleteAvailableSlotCommandHandler : ICommandHandler<DeleteAvailableSlotCommand>
{
    private readonly IAvailableSlotRepository _availableSlotRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAvailableSlotCommandHandler(
        IAvailableSlotRepository availableSlotRepository,
        IUnitOfWork unitOfWork)
    {
        _availableSlotRepository = availableSlotRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteAvailableSlotCommand request, CancellationToken cancellationToken)
    {
        var availableSlot = await _availableSlotRepository.GetByIdWithSchedulesAsync(
            request.AvailableSlotId, cancellationToken);

        if (availableSlot is null)
        {
            return Result.NotFound($"Available slot with ID '{request.AvailableSlotId}' was not found.");
        }

        // Check if there are any active bookings
        var hasActiveBookings = availableSlot.Schedules.Any(s => 
            s.Status != ScheduleStatus.Cancelled && s.Status != ScheduleStatus.Completed);

        if (hasActiveBookings)
        {
            return Result.Conflict("Cannot delete an available slot with active bookings. Cancel the bookings first.");
        }

        await _availableSlotRepository.DeleteAsync(availableSlot, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
