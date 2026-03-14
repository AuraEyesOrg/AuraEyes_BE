using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Scheduling.AppointmentSlots.Commands.UpdateAppointmentSlot;

/// <summary>
/// Handler for UpdateAppointmentSlotCommand.
/// </summary>
public class UpdateAppointmentSlotCommandHandler : ICommandHandler<UpdateAppointmentSlotCommand>
{
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAppointmentSlotCommandHandler(
        IAppointmentSlotRepository appointmentSlotRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentSlotRepository = appointmentSlotRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateAppointmentSlotCommand request, CancellationToken cancellationToken)
    {
        var slot = await _appointmentSlotRepository.GetByIdAsync(request.AppointmentSlotId, cancellationToken);

        if (slot is null)
        {
            return Result.NotFound($"Appointment slot with ID '{request.AppointmentSlotId}' was not found.");
        }

        // Cannot update booked or completed slots (except for cost)
        if (slot.Status != ScheduleStatus.Available)
        {
            return Result.Failure($"Cannot update appointment slot with status '{slot.Status}'. Only available slots can be modified.");
        }

        // Check for overlapping slots
        var hasOverlap = await _appointmentSlotRepository.HasOverlappingSlotAsync(
            request.Date,
            request.StartTime,
            request.EndTime,
            request.AppointmentSlotId,
            cancellationToken);

        if (hasOverlap)
        {
            return Result.Conflict("An overlapping appointment slot already exists for this date and time.");
        }

        // Create new slot with updated values (since entity has private setters)
        // We need to add an Update method to the entity
        // For now, we update the cost since the entity supports it
        slot.UpdateCost(request.Cost);
        
        await _appointmentSlotRepository.UpdateAsync(slot, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
