using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Scheduling.AppointmentSlots.Commands.ConfirmReservation;

/// <summary>
/// Handler for ConfirmReservationCommand.
/// Confirms a slot reservation and creates a consultation session.
/// </summary>
public class ConfirmReservationCommandHandler : ICommandHandler<ConfirmReservationCommand, ConfirmReservationResult>
{
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IConsultationSessionRepository _consultationSessionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ConfirmReservationCommandHandler> _logger;

    public ConfirmReservationCommandHandler(
        IAppointmentSlotRepository appointmentSlotRepository,
        IConsultationSessionRepository consultationSessionRepository,
        IUnitOfWork unitOfWork,
        ILogger<ConfirmReservationCommandHandler> logger)
    {
        _appointmentSlotRepository = appointmentSlotRepository;
        _consultationSessionRepository = consultationSessionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ConfirmReservationResult>> Handle(
        ConfirmReservationCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Get slot with lock
            var slot = await _appointmentSlotRepository.GetByIdWithLockAsync(
                request.AppointmentSlotId, cancellationToken);

            if (slot is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ConfirmReservationResult>.NotFound(
                    $"Appointment slot '{request.AppointmentSlotId}' not found.");
            }

            // Verify slot is reserved
            if (slot.Status != ScheduleStatus.Reserved)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ConfirmReservationResult>.Failure(
                    $"Slot is not in reserved state. Current status: {slot.Status}");
            }

            // Verify the reservation belongs to this patient
            if (slot.ReservedBy != request.PatientId)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ConfirmReservationResult>.Forbidden(
                    "This slot was reserved by a different patient.");
            }

            // Check if reservation has expired
            if (slot.IsReservationExpired())
            {
                // Release the reservation
                slot.ReleaseReservation();
                await _appointmentSlotRepository.UpdateAsync(slot, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<ConfirmReservationResult>.Failure(
                    "Your reservation has expired. Please try booking again.");
            }

            // Get doctor ID from template
            var ophthalmologistId = slot.ScheduleTemplate?.OphthalId;

            // Calculate appointment time
            var appointmentTime = slot.Date.ToDateTime(slot.StartTime, DateTimeKind.Utc);

            // Confirm the reservation (slot becomes Booked)
            slot.ConfirmReservation(request.PatientId);

            // Create consultation session
            var session = ConsultationSession.CreateVideoCall(
                patientId: request.PatientId,
                price: slot.Cost ?? 0,
                appointmentTime: appointmentTime,
                ophthalmologistId: ophthalmologistId,
                appointmentSlotId: slot.Id);

            // Update sharing preferences
            // Note: This would need methods on ConsultationSession to set these values
            // For now we rely on the factory defaults

            await _appointmentSlotRepository.UpdateAsync(slot, cancellationToken);
            await _consultationSessionRepository.AddAsync(session, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Reservation confirmed for slot {SlotId}, consultation session {SessionId} created",
                slot.Id, session.Id);

            return Result<ConfirmReservationResult>.Success(new ConfirmReservationResult
            {
                ConsultationSessionId = session.Id,
                AppointmentSlotId = slot.Id,
                AppointmentTime = appointmentTime
            });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Error confirming reservation for slot {SlotId}", request.AppointmentSlotId);
            throw;
        }
    }
}
