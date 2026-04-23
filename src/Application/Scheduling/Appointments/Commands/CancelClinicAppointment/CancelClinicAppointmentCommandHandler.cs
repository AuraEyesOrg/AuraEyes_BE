using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Commands.CancelClinicAppointment;

public class CancelClinicAppointmentCommandHandler : ICommandHandler<CancelClinicAppointmentCommand>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CancelClinicAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IAppointmentSlotRepository appointmentSlotRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelClinicAppointmentCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.ProfileId is null)
        {
            return Result.Unauthorized("Patient profile is required.");
        }

        var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(request.AppointmentId, cancellationToken);
        if (appointment is null)
        {
            return Result.NotFound($"Clinic appointment '{request.AppointmentId}' not found.");
        }

        if (appointment.PatientId != _currentUser.ProfileId.Value)
        {
            return Result.Forbidden("You can only cancel your own clinic appointment.");
        }

        try
        {
            appointment.Cancel(_currentUser.ProfileId.Value, request.Reason);

            var slot = appointment.AppointmentSlot;
            if (slot is not null)
            {
                slot.CancelBooking();
                await _appointmentSlotRepository.UpdateAsync(slot, cancellationToken);
            }

            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
