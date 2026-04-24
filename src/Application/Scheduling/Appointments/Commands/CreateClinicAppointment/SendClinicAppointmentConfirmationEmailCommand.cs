using Application.Common.Interfaces;
using Domain.Repositories;
using Domain.Common;
using Domain.Entities.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Scheduling.Appointments.Commands.CreateClinicAppointment;

public record SendClinicAppointmentConfirmationEmailCommand(Guid AppointmentId) : IRequest<bool>;

public class SendClinicAppointmentConfirmationEmailCommandHandler : IRequestHandler<SendClinicAppointmentConfirmationEmailCommand, bool>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    private readonly ILogger<SendClinicAppointmentConfirmationEmailCommandHandler> _logger;

    public SendClinicAppointmentConfirmationEmailCommandHandler(
        IAppointmentRepository appointmentRepository,
        IAppointmentSlotRepository appointmentSlotRepository,
        IRepository<Patient> patientRepository,
        IIdentityService identityService,
        IEmailService emailService,
        ILogger<SendClinicAppointmentConfirmationEmailCommandHandler> logger)
    {
        _appointmentRepository = appointmentRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
        _patientRepository = patientRepository;
        _identityService = identityService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<bool> Handle(SendClinicAppointmentConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);
            if (appointment == null) return false;

            var slot = await _appointmentSlotRepository.GetByIdAsync(appointment.AppointmentSlotId, cancellationToken);
            if (slot == null) return false;

            var patientName = "bệnh nhân";
            string? patientEmail = null;

            var patient = await _patientRepository.GetByIdAsync(appointment.PatientId, cancellationToken);
            if (patient is not null)
            {
                if (patient.IsWalkIn)
                {
                    if (!string.IsNullOrWhiteSpace(patient.FullName))
                        patientName = patient.FullName;
                }
                else if (patient.UserId.HasValue)
                {
                    var patientUser = await _identityService.GetUserByIdAsync(patient.UserId.Value, cancellationToken);
                    if (!string.IsNullOrWhiteSpace(patientUser?.FullName))
                        patientName = patientUser.FullName;
                    if (!string.IsNullOrWhiteSpace(patientUser?.Email))
                        patientEmail = patientUser.Email;
                }
            }

            if (string.IsNullOrWhiteSpace(patientEmail)) return false;

            var qrPayload =
                $"AURA-CLINIC-APPOINTMENT|{appointment.Id}|{appointment.PatientId}|{slot.Date:yyyy-MM-dd}|{slot.StartTime:HH:mm}|{slot.EndTime:HH:mm}";
            var checkInCode = appointment.Id.ToString("N")[..10].ToUpperInvariant();

            await _emailService.SendClinicAppointmentConfirmationAsync(
                patientEmail,
                new ClinicAppointmentConfirmationEmailPayload(
                    appointment.Id,
                    patientName,
                    "Aura Clinic",
                    slot.Date,
                    slot.StartTime,
                    slot.EndTime,
                    appointment.VisitReason,
                    checkInCode,
                    qrPayload),
                cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send clinic appointment email for appointment {AppointmentId}", request.AppointmentId);
            return false;
        }
    }
}
