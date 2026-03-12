using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;

namespace Application.Ophthalmologists.AvailableSlots.Commands.UpdateAvailableSlot;

/// <summary>
/// Handler for UpdateAvailableSlotCommand.
/// </summary>
public class UpdateAvailableSlotCommandHandler : ICommandHandler<UpdateAvailableSlotCommand>
{
    private readonly IAvailableSlotRepository _availableSlotRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAvailableSlotCommandHandler(
        IAvailableSlotRepository availableSlotRepository,
        IUnitOfWork unitOfWork)
    {
        _availableSlotRepository = availableSlotRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateAvailableSlotCommand request, CancellationToken cancellationToken)
    {
        var availableSlot = await _availableSlotRepository.GetByIdWithSchedulesAsync(
            request.AvailableSlotId, cancellationToken);

        if (availableSlot is null)
        {
            return Result.NotFound($"Available slot with ID '{request.AvailableSlotId}' was not found.");
        }

        // Check for overlapping slots (excluding current slot)
        var hasOverlap = await _availableSlotRepository.HasOverlappingSlotAsync(
            availableSlot.OphthalmologistId,
            availableSlot.OrganisationId,
            request.StartTime,
            request.EndTime,
            request.AvailableSlotId,
            cancellationToken);

        if (hasOverlap)
        {
            return Result.Conflict("An overlapping available slot already exists for this time period.");
        }

        // Check that new capacity is not less than current bookings
        var activeBookings = availableSlot.Schedules.Count(s => 
            s.Status != Domain.Enums.ScheduleStatus.Cancelled);
        
        if (request.MaxCapacity < activeBookings)
        {
            return Result.Failure($"Cannot reduce capacity below current active bookings ({activeBookings}).");
        }

        availableSlot.Update(request.StartTime, request.EndTime, request.MaxCapacity);
        
        await _availableSlotRepository.UpdateAsync(availableSlot, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
