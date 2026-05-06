using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.MedicalRecords;
using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Commands.CheckInClinicAppointment;

public class CheckInClinicAppointmentCommandHandler : ICommandHandler<CheckInClinicAppointmentCommand>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IPatientVisitRepository _patientVisitRepository;
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IIdentityService _identityService;

    public CheckInClinicAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IOrderRepository orderRepository,
        IPatientVisitRepository patientVisitRepository,
        IMedicalRecordRepository medicalRecordRepository,
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IIdentityService identityService)
    {
        _appointmentRepository = appointmentRepository;
        _orderRepository = orderRepository;
        _patientVisitRepository = patientVisitRepository;
        _medicalRecordRepository = medicalRecordRepository;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _identityService = identityService;
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

        var orders = await _orderRepository.GetByAppointmentIdsAsync(
            new[] { request.AppointmentId },
            cancellationToken);

        foreach (var order in orders)
        {
            if (!order.IsClinicDepositSatisfiedForCheckIn())
            {
                return Result.PaymentRequired("DepositNotPaid");
            }
        }

        if (appointment.Status == AppointmentStatus.Pending)
        {
            appointment.Confirm();
        }

        appointment.CheckIn();
        await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

        var visit = PatientVisit.CreateFromAppointment(appointment);
        await _patientVisitRepository.AddAsync(visit, cancellationToken);

        // Create initial empty Medical Record (Step 1 requirement)
        var medicalRecordNumber = $"MT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}";
        var medicalRecord = new Domain.Entities.MedicalRecords.MedicalRecord(visit.PatientId, medicalRecordNumber);
        medicalRecord.LinkToPatientVisit(visit.Id);
        
        await _medicalRecordRepository.AddAsync(medicalRecord, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Resolve patient name for notification
        string patientName = request.PatientName ?? "Patient";
        if (string.IsNullOrWhiteSpace(request.PatientName) && appointment.Patient != null)
        {
            if (appointment.Patient.UserId.HasValue)
            {
                var user = await _identityService.GetUserByIdAsync(appointment.Patient.UserId.Value, cancellationToken);
                patientName = user?.FullName ?? "Patient";
            }
            else if (!string.IsNullOrWhiteSpace(appointment.Patient.FullName))
            {
                patientName = appointment.Patient.FullName;
            }
        }

        // Notify Coordinator (ClinicStaff) that a patient has checked in
        await _notificationService.SendToRoleAsync(
            roleName: Roles.ClinicStaff,
            title: "New Patient in Queue",
            message: $"{patientName} has checked in and is waiting for screening.",
            type: NotificationType.SystemAlert,
            payload: new { VisitId = visit.Id, PatientId = visit.PatientId, PatientName = patientName },
            cancellationToken: cancellationToken
        );

        return Result.Success();
    }
}
