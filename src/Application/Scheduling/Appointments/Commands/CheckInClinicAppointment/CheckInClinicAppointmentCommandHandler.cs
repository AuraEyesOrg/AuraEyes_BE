using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;
using Application.Common.Constants;

namespace Application.Scheduling.Appointments.Commands.CheckInClinicAppointment;

public class CheckInClinicAppointmentCommandHandler : ICommandHandler<CheckInClinicAppointmentCommand>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IPatientVisitRepository _patientVisitRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public CheckInClinicAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IPatientVisitRepository patientVisitRepository,
        IUnitOfWork unitOfWork,
        INotificationService notificationService)
    {
        _appointmentRepository = appointmentRepository;
        _patientVisitRepository = patientVisitRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
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
        }

        appointment.CheckIn();
        await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

        var visit = PatientVisit.CreateFromAppointment(appointment);
        await _patientVisitRepository.AddAsync(visit, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notify Coordinator (ClinicStaff) that a patient has checked in
        await _notificationService.SendToRoleAsync(
            roleName: Roles.ClinicStaff,
            title: "New Patient in Queue",
            message: $"Patient {appointment.Patient?.FullName ?? "Unknown"} has checked in and is waiting for screening.",
            type: NotificationType.SystemAlert,
            payload: new { VisitId = visit.Id, PatientId = visit.PatientId },
            cancellationToken: cancellationToken
        );

        return Result.Success();
    }
}
