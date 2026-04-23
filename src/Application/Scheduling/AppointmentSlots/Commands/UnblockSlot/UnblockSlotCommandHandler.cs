using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Scheduling.AppointmentSlots.Commands.UnblockSlot;

/// <summary>
/// Handler for UnblockSlotCommand.
/// Unblocks an appointment slot and makes it available again.
/// </summary>
public class UnblockSlotCommandHandler : ICommandHandler<UnblockSlotCommand>
{
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UnblockSlotCommandHandler> _logger;

    public UnblockSlotCommandHandler(
        IAppointmentSlotRepository appointmentSlotRepository,
        IUnitOfWork unitOfWork,
        ILogger<UnblockSlotCommandHandler> logger)
    {
        _appointmentSlotRepository = appointmentSlotRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UnblockSlotCommand request, CancellationToken cancellationToken)
    {
        var slot = await _appointmentSlotRepository.GetByIdAsync(
            request.AppointmentSlotId, cancellationToken);

        if (slot is null)
        {
            return Result.NotFound($"Appointment slot '{request.AppointmentSlotId}' not found.");
        }

        if (slot.Status != ScheduleStatus.Blocked)
        {
            return Result.Failure($"This slot is not blocked. Current status: {slot.Status}");
        }

        try
        {
            slot.Unblock();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Slot {SlotId} unblocked",
                request.AppointmentSlotId);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Database error when unblocking slot {SlotId}",
                request.AppointmentSlotId);

            return Result.Failure(ex.InnerException?.Message ?? ex.Message);
        }
    }
}
