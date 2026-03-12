using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.ClinicAppointments.Commands.CancelClinicAppointment;

/// <summary>
/// Handler for CancelClinicAppointmentCommand.
/// Cancels the appointment and decrements the slot's booked count.
/// </summary>
public class CancelClinicAppointmentCommandHandler : ICommandHandler<CancelClinicAppointmentCommand, bool>
{
    private readonly IClinicAppointmentRepository _clinicAppointmentRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelClinicAppointmentCommandHandler> _logger;

    public CancelClinicAppointmentCommandHandler(
        IClinicAppointmentRepository clinicAppointmentRepository,
        IAppointmentSlotRepository appointmentSlotRepository,
        IUnitOfWork unitOfWork,
        ILogger<CancelClinicAppointmentCommandHandler> logger)
    {
        _clinicAppointmentRepository = clinicAppointmentRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(CancelClinicAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _clinicAppointmentRepository.GetByIdWithDetailsAsync(
            request.AppointmentId, cancellationToken);

        if (appointment is null)
        {
            return Result<bool>.NotFound("Appointment not found.");
        }

        if (!appointment.CanBeCancelled())
        {
            return Result<bool>.Failure($"Cannot cancel appointment with status {appointment.Status}.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Cancel the appointment
            appointment.Cancel(request.CancelledBy, request.Reason);

            // Decrement the slot's booked count
            var slot = await _appointmentSlotRepository.GetByIdAsync(
                appointment.AppointmentSlotId, cancellationToken);

            if (slot is not null && slot.BookedCount > 0)
            {
                slot.CancelBooking();
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Cancelled clinic appointment {AppointmentId} by user {CancelledBy}. Reason: {Reason}",
                request.AppointmentId, request.CancelledBy, request.Reason ?? "Not specified");

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Error cancelling clinic appointment {AppointmentId}", request.AppointmentId);
            throw;
        }
    }
}
