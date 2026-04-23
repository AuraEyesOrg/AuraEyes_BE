using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;

namespace Application.Scheduling.AppointmentSlots.Commands.UpdateAppointmentSlot;

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

        if (slot.BookedCount > 0)
        {
            return Result.Failure("Cannot update slot that has existing bookings.");
        }

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

        // Note: Slot is effectively immutable for time once created according to typical scheduling patterns,
        // but since this command exists we allow cost updates.
        slot.UpdateCost(request.Cost);

        await _appointmentSlotRepository.UpdateAsync(slot, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
