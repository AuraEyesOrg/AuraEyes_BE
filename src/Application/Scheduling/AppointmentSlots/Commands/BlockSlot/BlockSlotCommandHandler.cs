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
        var slot = await _appointmentSlotRepository.GetByIdAsync(
            request.AppointmentSlotId, cancellationToken);

        if (slot is null)
        {
            return Result.NotFound($"Appointment slot '{request.AppointmentSlotId}' not found.");
        }

        if (slot.BookedCount > 0)
        {
            return Result.Conflict(
                "Cannot block a slot that has existing bookings. Please cancel the appointments first.");
        }

        if (slot.Status == ScheduleStatus.Blocked)
        {
            return Result.Failure("This slot is already blocked.");
        }

        try
        {
            slot.Block();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Slot {SlotId} blocked. Reason: {Reason}",
                request.AppointmentSlotId, request.Reason ?? "Not specified");

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Database error when blocking slot {SlotId}",
                request.AppointmentSlotId);

            return Result.Failure(ex.InnerException?.Message ?? ex.Message);
        }
    }
}
