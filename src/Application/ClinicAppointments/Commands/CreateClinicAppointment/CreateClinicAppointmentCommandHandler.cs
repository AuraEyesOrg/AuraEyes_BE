using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.ClinicAppointments.Commands.CreateClinicAppointment;

/// <summary>
/// Handler for CreateClinicAppointmentCommand.
/// Creates a clinic appointment with atomic capacity check to prevent race conditions.
/// </summary>
public class CreateClinicAppointmentCommandHandler : ICommandHandler<CreateClinicAppointmentCommand, Guid>
{
    private readonly IClinicAppointmentRepository _clinicAppointmentRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateClinicAppointmentCommandHandler> _logger;

    public CreateClinicAppointmentCommandHandler(
        IClinicAppointmentRepository clinicAppointmentRepository,
        IAppointmentSlotRepository appointmentSlotRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateClinicAppointmentCommandHandler> logger)
    {
        _clinicAppointmentRepository = clinicAppointmentRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateClinicAppointmentCommand request, CancellationToken cancellationToken)
    {
        // Check if patient already has an appointment at this slot
        var hasExisting = await _clinicAppointmentRepository.HasExistingAppointmentAsync(
            request.PatientId, request.SlotId, cancellationToken);

        if (hasExisting)
        {
            return Result<Guid>.Failure("You already have an appointment at this time slot.");
        }

        // Begin transaction for atomic capacity check
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Get slot with row-level lock to prevent race conditions
            var slot = await _appointmentSlotRepository.GetByIdWithLockAsync(request.SlotId, cancellationToken);

            if (slot is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<Guid>.NotFound("Appointment slot not found.");
            }

            // Verify slot belongs to the organisation
            if (slot.ScheduleTemplate?.OrgId != request.OrganisationId)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<Guid>.Failure("Slot does not belong to the specified organisation.");
            }

            // Check slot status
            if (slot.Status == ScheduleStatus.Blocked)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<Guid>.Failure("This slot is currently blocked and not available for booking.");
            }

            if (slot.Status != ScheduleStatus.Available)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<Guid>.Failure($"Slot is not available. Current status: {slot.Status}");
            }

            // Check capacity
            if (!slot.HasCapacity())
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<Guid>.Failure("This slot has reached maximum capacity. Please select another time.");
            }

            // Atomic increment of booked count
            slot.BookWithCapacity();

            // Create the appointment
            var appointment = new ClinicAppointment(
                request.PatientId,
                request.OrganisationId,
                request.SlotId,
                request.VisitReason);

            await _clinicAppointmentRepository.AddAsync(appointment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Created clinic appointment {AppointmentId} for patient {PatientId} at slot {SlotId}. Booked: {BookedCount}/{MaxCapacity}",
                appointment.Id, request.PatientId, request.SlotId, slot.BookedCount, slot.MaxCapacity);

            return Result<Guid>.Success(appointment.Id);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("capacity") || ex.Message.Contains("available"))
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogWarning(ex, "Business rule violation while booking slot {SlotId}", request.SlotId);
            return Result<Guid>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Error creating clinic appointment for patient {PatientId} at slot {SlotId}", 
                request.PatientId, request.SlotId);
            throw;
        }
    }
}
