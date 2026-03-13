using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Scheduling.AppointmentSlots.Commands.UpdateAppointmentSlotStatus;

/// <summary>
/// Handler for UpdateAppointmentSlotStatusCommand.
/// </summary>
public class UpdateAppointmentSlotStatusCommandHandler : ICommandHandler<UpdateAppointmentSlotStatusCommand, bool>
{
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateAppointmentSlotStatusCommandHandler> _logger;

    public UpdateAppointmentSlotStatusCommandHandler(
        IAppointmentSlotRepository appointmentSlotRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateAppointmentSlotStatusCommandHandler> logger)
    {
        _appointmentSlotRepository = appointmentSlotRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(
        UpdateAppointmentSlotStatusCommand request,
        CancellationToken cancellationToken)
    {
        var slot = await _appointmentSlotRepository.GetByIdAsync(request.AppointmentSlotId, cancellationToken);

        if (slot is null)
        {
            return Result<bool>.NotFound($"Appointment slot '{request.AppointmentSlotId}' not found.");
        }

        var previousStatus = slot.Status;

        // Use appropriate domain methods based on the new status
        try
        {
            switch (request.NewStatus)
            {
                case ScheduleStatus.Completed:
                    slot.Complete();
                    break;
                case ScheduleStatus.NoShow:
                    slot.MarkNoShow();
                    break;
                case ScheduleStatus.Cancelled:
                    slot.Cancel();
                    break;
                default:
                    slot.UpdateStatus(request.NewStatus);
                    break;
            }
        }
        catch (InvalidOperationException ex)
        {
            return Result<bool>.Failure(ex.Message);
        }

        await _appointmentSlotRepository.UpdateAsync(slot, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Appointment slot {SlotId} status updated from {OldStatus} to {NewStatus}",
            request.AppointmentSlotId, previousStatus, request.NewStatus);

        return Result<bool>.Success(true);
    }
}
