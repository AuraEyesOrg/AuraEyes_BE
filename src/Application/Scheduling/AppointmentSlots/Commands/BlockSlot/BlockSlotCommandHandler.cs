using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Scheduling.AppointmentSlots.Commands.BlockSlot;

/// <summary>
/// Handler for BlockSlotCommand.
/// Blocks an appointment slot so it's not available for patients.
/// </summary>
public class BlockSlotCommandHandler : ICommandHandler<BlockSlotCommand>
{
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BlockSlotCommandHandler> _logger;

    public BlockSlotCommandHandler(
        IAppointmentSlotRepository appointmentSlotRepository,
        IUnitOfWork unitOfWork,
        ILogger<BlockSlotCommandHandler> logger)
    {
        _appointmentSlotRepository = appointmentSlotRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(BlockSlotCommand request, CancellationToken cancellationToken)
    {
        var slot = await _appointmentSlotRepository.GetByIdWithTemplateAsync(
            request.AppointmentSlotId, cancellationToken);

        if (slot is null)
        {
            return Result.NotFound($"Appointment slot '{request.AppointmentSlotId}' not found.");
        }

        // Verify the slot belongs to this ophthalmologist
        if (slot.ScheduleTemplate?.OphthalId != request.OphthalmologistId)
        {
            return Result.Forbidden("You are not authorized to modify this slot.");
        }

        // Check current status
        if (slot.Status == ScheduleStatus.Booked)
        {
            return Result.Conflict(
                "Cannot block a slot that is already booked. Please cancel the appointment first.");
        }

        if (slot.Status == ScheduleStatus.Reserved)
        {
            return Result.Conflict(
                "Cannot block a slot that is currently reserved. Please wait for the reservation to expire.");
        }

        if (slot.Status == ScheduleStatus.Blocked)
        {
            return Result.Failure("This slot is already blocked.");
        }

        try
        {
            slot.Block();
            await _appointmentSlotRepository.UpdateAsync(slot, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Slot {SlotId} blocked by ophthalmologist {OphthalmologistId}. Reason: {Reason}",
                request.AppointmentSlotId, request.OphthalmologistId, request.Reason ?? "Not specified");

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
