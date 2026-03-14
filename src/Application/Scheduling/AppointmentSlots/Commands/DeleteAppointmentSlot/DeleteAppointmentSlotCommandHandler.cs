using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Scheduling.AppointmentSlots.Commands.DeleteAppointmentSlot;

/// <summary>
/// Handler for DeleteAppointmentSlotCommand.
/// Cancels the appointment slot (status transition to Cancelled).
/// </summary>
public class DeleteAppointmentSlotCommandHandler : ICommandHandler<DeleteAppointmentSlotCommand>
{
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteAppointmentSlotCommandHandler> _logger;

    public DeleteAppointmentSlotCommandHandler(
        IAppointmentSlotRepository appointmentSlotRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteAppointmentSlotCommandHandler> logger)
    {
        _appointmentSlotRepository = appointmentSlotRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteAppointmentSlotCommand request, CancellationToken cancellationToken)
    {
        var slot = await _appointmentSlotRepository.GetByIdAsync(request.AppointmentSlotId, cancellationToken);

        if (slot is null)
        {
            return Result.NotFound($"Appointment slot with ID '{request.AppointmentSlotId}' was not found.");
        }

        if (slot.Status == ScheduleStatus.Completed)
        {
            return Result.Failure("Cannot delete a completed appointment slot.");
        }

        if (slot.Status == ScheduleStatus.Cancelled)
        {
            return Result.Failure("Appointment slot is already cancelled.");
        }

        slot.Cancel();

        await _appointmentSlotRepository.UpdateAsync(slot, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Appointment slot {SlotId} cancelled (deleted)", request.AppointmentSlotId);

        return Result.Success();
    }
}
