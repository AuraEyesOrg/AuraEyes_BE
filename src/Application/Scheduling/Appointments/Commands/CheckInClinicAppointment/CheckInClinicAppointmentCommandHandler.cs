using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Commands.CheckInClinicAppointment;

public class CheckInClinicAppointmentCommandHandler : ICommandHandler<CheckInClinicAppointmentCommand>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IPatientVisitRepository _patientVisitRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CheckInClinicAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IPatientVisitRepository patientVisitRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _patientVisitRepository = patientVisitRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CheckInClinicAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(request.AppointmentId, cancellationToken);
        if (appointment is null)
        {
            return Result.NotFound($"Clinic appointment '{request.AppointmentId}' not found.");
        }

        if (appointment.Status is AppointmentStatus.Cancelled or AppointmentStatus.NoShow)
        {
            return Result.Failure("Cannot check in a cancelled or no-show appointment.");
        }

        var existingVisit = await _patientVisitRepository.GetByAppointmentIdAsync(request.AppointmentId, cancellationToken);
        if (existingVisit is not null)
        {
            return Result.Success();
        }

        if (appointment.Status == AppointmentStatus.Pending)
        {
            appointment.Confirm();
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
        }

        var visit = PatientVisit.CreateFromAppointment(appointment);
        await _patientVisitRepository.AddAsync(visit, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
