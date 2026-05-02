using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Commands.CreateAdHocSlotAndRebook;

public class CreateAdHocSlotAndRebookCommandHandler
    : ICommandHandler<CreateAdHocSlotAndRebookCommand, CreateAdHocSlotAndRebookResult>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAdHocSlotAndRebookCommandHandler(
        IAppointmentRepository appointmentRepository,
        IAppointmentSlotRepository appointmentSlotRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateAdHocSlotAndRebookResult>> Handle(
        CreateAdHocSlotAndRebookCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. Load old appointment with slot
            var oldAppointment = await _appointmentRepository.GetByIdWithDetailsAsync(
                request.AppointmentId, cancellationToken);
            if (oldAppointment is null)
                return Result<CreateAdHocSlotAndRebookResult>.NotFound(
                    $"Appointment '{request.AppointmentId}' not found.");

            // 2. Cancel old, release old slot
            oldAppointment.MarkLateAndRelease(oldAppointment.PatientId);
            if (oldAppointment.AppointmentSlot is not null)
                oldAppointment.AppointmentSlot.CancelBooking();

            await _appointmentRepository.UpdateAsync(oldAppointment, cancellationToken);
            if (oldAppointment.AppointmentSlot is not null)
                await _appointmentSlotRepository.UpdateAsync(
                    oldAppointment.AppointmentSlot, cancellationToken);

            // 3. Create ad-hoc slot (ScheduleTemplateId = null)
            var adHocSlot = AppointmentSlot.CreateAdHoc(
                request.Date,
                request.StartTime,
                request.EndTime,
                request.MaxCapacity,
                request.DoctorId,
                request.Cost);

            await _appointmentSlotRepository.AddAsync(adHocSlot, cancellationToken);

            // 4. Book and create new appointment
            adHocSlot.BookWithCapacity();
            var newAppointment = new Appointment(
                oldAppointment.PatientId,
                adHocSlot.Id,
                oldAppointment.Price,
                oldAppointment.PricingType,
                request.DoctorId ?? oldAppointment.RequestedDoctorId,
                oldAppointment.VisitReason);

            await _appointmentRepository.AddAsync(newAppointment, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result<CreateAdHocSlotAndRebookResult>.Success(
                new CreateAdHocSlotAndRebookResult
                {
                    NewAppointmentId = newAppointment.Id,
                    NewSlotId = adHocSlot.Id
                });
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<CreateAdHocSlotAndRebookResult>.Failure(ex.Message);
        }
    }
}
