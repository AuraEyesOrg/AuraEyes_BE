using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Commands.RebookLatePatientToExistingSlot;

public class RebookLatePatientToExistingSlotCommandHandler
    : ICommandHandler<RebookLatePatientToExistingSlotCommand, RebookLatePatientResult>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RebookLatePatientToExistingSlotCommandHandler(
        IAppointmentRepository appointmentRepository,
        IAppointmentSlotRepository appointmentSlotRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RebookLatePatientResult>> Handle(
        RebookLatePatientToExistingSlotCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. Load old appointment with slot
            var oldAppointment = await _appointmentRepository.GetByIdWithDetailsAsync(
                request.AppointmentId, cancellationToken);
            if (oldAppointment is null)
                return Result<RebookLatePatientResult>.NotFound($"Appointment '{request.AppointmentId}' not found.");

            // 2. Load new slot with lock
            var newSlot = await _appointmentSlotRepository.GetByIdWithLockAsync(
                request.NewSlotId, cancellationToken);
            if (newSlot is null)
                return Result<RebookLatePatientResult>.NotFound($"Slot '{request.NewSlotId}' not found.");

            if (newSlot.Status != ScheduleStatus.Available || !newSlot.HasCapacity())
                return Result<RebookLatePatientResult>.Conflict("The selected slot is no longer available.");

            // 3. Cancel old, release old slot
            oldAppointment.MarkLateAndRelease(oldAppointment.PatientId);
            if (oldAppointment.AppointmentSlot is not null)
                oldAppointment.AppointmentSlot.CancelBooking();

            await _appointmentRepository.UpdateAsync(oldAppointment, cancellationToken);
            if (oldAppointment.AppointmentSlot is not null)
                await _appointmentSlotRepository.UpdateAsync(oldAppointment.AppointmentSlot, cancellationToken);

            // 4. Book new slot and create new appointment
            newSlot.BookWithCapacity();
            var newAppointment = new Appointment(
                oldAppointment.PatientId,
                newSlot.Id,
                oldAppointment.Price,
                oldAppointment.PricingType,
                oldAppointment.RequestedDoctorId,
                oldAppointment.VisitReason);

            await _appointmentSlotRepository.UpdateAsync(newSlot, cancellationToken);
            await _appointmentRepository.AddAsync(newAppointment, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result<RebookLatePatientResult>.Success(new RebookLatePatientResult
            {
                NewAppointmentId = newAppointment.Id,
                NewSlotId = newSlot.Id
            });
        }
        catch (Domain.Common.ConcurrencyException)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<RebookLatePatientResult>.Conflict(
                "Slot was updated by another request. Please retry.");
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<RebookLatePatientResult>.Failure(ex.Message);
        }
    }
}
