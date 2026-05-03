using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Commands.CancelLateAndGrantDiscount;

public class CancelLateAndGrantDiscountCommandHandler
    : ICommandHandler<CancelLateAndGrantDiscountCommand, CancelLateAndGrantDiscountResult>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IUnitOfWork _unitOfWork;

    private const decimal LateArrivalDiscountRate = 0.10m; // 10%
    private const int DiscountExpiryDays = 30;

    public CancelLateAndGrantDiscountCommandHandler(
        IAppointmentRepository appointmentRepository,
        IAppointmentSlotRepository appointmentSlotRepository,
        IRepository<Patient> patientRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CancelLateAndGrantDiscountResult>> Handle(
        CancelLateAndGrantDiscountCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. Load appointment with slot
            var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(
                request.AppointmentId, cancellationToken);
            if (appointment is null)
                return Result<CancelLateAndGrantDiscountResult>.NotFound(
                    $"Appointment '{request.AppointmentId}' not found.");

            // 2. Cancel appointment (mark as late)
            appointment.MarkLateAndRelease(appointment.PatientId);

            // 3. Release old slot capacity
            if (appointment.AppointmentSlot is not null)
            {
                appointment.AppointmentSlot.CancelBooking();
                await _appointmentSlotRepository.UpdateAsync(
                    appointment.AppointmentSlot, cancellationToken);
            }

            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

            // 4. Grant discount to patient for next online booking
            var patient = await _patientRepository.GetByIdAsync(
                appointment.PatientId, cancellationToken);
            if (patient is null)
                return Result<CancelLateAndGrantDiscountResult>.NotFound(
                    $"Patient '{appointment.PatientId}' not found.");

            patient.GrantDiscount(LateArrivalDiscountRate, DiscountExpiryDays);
            await _patientRepository.UpdateAsync(patient, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result<CancelLateAndGrantDiscountResult>.Success(
                new CancelLateAndGrantDiscountResult
                {
                    CancelledAppointmentId = appointment.Id,
                    DiscountRate = LateArrivalDiscountRate,
                    DiscountExpiryDate = patient.DiscountExpiryDate!.Value,
                });
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<CancelLateAndGrantDiscountResult>.Failure(ex.Message);
        }
    }
}
