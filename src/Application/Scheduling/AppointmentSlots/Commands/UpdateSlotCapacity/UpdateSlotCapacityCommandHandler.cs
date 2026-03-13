using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Scheduling.AppointmentSlots.Commands.UpdateSlotCapacity;

/// <summary>
/// Handler for UpdateSlotCapacityCommand.
/// </summary>
public class UpdateSlotCapacityCommandHandler : ICommandHandler<UpdateSlotCapacityCommand, bool>
{
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateSlotCapacityCommandHandler> _logger;

    public UpdateSlotCapacityCommandHandler(
        IAppointmentSlotRepository appointmentSlotRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateSlotCapacityCommandHandler> logger)
    {
        _appointmentSlotRepository = appointmentSlotRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(UpdateSlotCapacityCommand request, CancellationToken cancellationToken)
    {
        var slot = await _appointmentSlotRepository.GetByIdAsync(request.SlotId, cancellationToken);

        if (slot is null)
        {
            return Result<bool>.NotFound("Appointment slot not found.");
        }

        try
        {
            var oldCapacity = slot.MaxCapacity;
            slot.UpdateCapacity(request.NewCapacity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Updated slot {SlotId} capacity from {OldCapacity} to {NewCapacity}",
                request.SlotId, oldCapacity, request.NewCapacity);

            return Result<bool>.Success(true);
        }
        catch (ArgumentException ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}
