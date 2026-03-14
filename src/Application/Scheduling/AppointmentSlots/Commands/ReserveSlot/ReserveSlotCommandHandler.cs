using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Scheduling.AppointmentSlots.Commands.ReserveSlot;

/// <summary>
/// Handler for ReserveSlotCommand.
/// Uses database transaction with row locking to prevent race conditions.
/// </summary>
public class ReserveSlotCommandHandler : ICommandHandler<ReserveSlotCommand, ReserveSlotResult>
{
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReserveSlotCommandHandler> _logger;

    public ReserveSlotCommandHandler(
        IAppointmentSlotRepository appointmentSlotRepository,
        IUnitOfWork unitOfWork,
        ILogger<ReserveSlotCommandHandler> logger)
    {
        _appointmentSlotRepository = appointmentSlotRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ReserveSlotResult>> Handle(ReserveSlotCommand request, CancellationToken cancellationToken)
    {
        // Start a transaction to ensure atomicity
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Get the slot with a row lock to prevent concurrent modifications
            var slot = await _appointmentSlotRepository.GetByIdWithLockAsync(
                request.AppointmentSlotId, cancellationToken);

            if (slot is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ReserveSlotResult>.NotFound($"Appointment slot '{request.AppointmentSlotId}' not found.");
            }

            // Check if slot is available for reservation
            if (slot.Status != ScheduleStatus.Available)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                // Provide specific error messages based on status
                return slot.Status switch
                {
                    ScheduleStatus.Reserved => Result<ReserveSlotResult>.Conflict(
                        "This slot is already reserved by another patient. Please try a different slot."),
                    ScheduleStatus.Booked => Result<ReserveSlotResult>.Conflict(
                        "This slot is already booked. Please try a different slot."),
                    ScheduleStatus.Blocked => Result<ReserveSlotResult>.Conflict(
                        "This slot is not available. Please try a different slot."),
                    _ => Result<ReserveSlotResult>.Conflict(
                        $"This slot is not available. Current status: {slot.Status}")
                };
            }

            // Calculate expiration time
            var expirationTime = DateTime.UtcNow.AddMinutes(request.ReservationMinutes);

            // Reserve the slot
            slot.Reserve(request.PatientId, expirationTime);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Slot {SlotId} reserved by patient {PatientId}, expires at {ExpiresAt}",
                request.AppointmentSlotId, request.PatientId, expirationTime);

            return Result<ReserveSlotResult>.Success(new ReserveSlotResult
            {
                SlotId = slot.Id,
                ExpiresAt = expirationTime,
                RemainingSeconds = request.ReservationMinutes * 60
            });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Error reserving slot {SlotId}", request.AppointmentSlotId);
            throw;
        }
    }
}
