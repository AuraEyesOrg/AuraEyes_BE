using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Scheduling.Appointments.Commands.RebookLatePatientToExistingSlot;

public class RebookLatePatientToExistingSlotCommandHandler
    : ICommandHandler<RebookLatePatientToExistingSlotCommand, RebookLatePatientResult>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RebookLatePatientToExistingSlotCommandHandler> _logger;

    public RebookLatePatientToExistingSlotCommandHandler(
        IAppointmentRepository appointmentRepository,
        IAppointmentSlotRepository appointmentSlotRepository,
        IRepository<Patient> patientRepository,
        IIdentityService identityService,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        ILogger<RebookLatePatientToExistingSlotCommandHandler> logger)
    {
        _appointmentRepository = appointmentRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
        _patientRepository = patientRepository;
        _identityService = identityService;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _logger = logger;
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

            await SendRebookConfirmationEmailAsync(newAppointment, newSlot, cancellationToken);

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

    private async Task SendRebookConfirmationEmailAsync(
        Appointment appointment,
        AppointmentSlot slot,
        CancellationToken cancellationToken)
    {
        try
        {
            var patient = await _patientRepository.GetByIdAsync(appointment.PatientId, cancellationToken);
            if (patient is null || !patient.UserId.HasValue) return;

            var user = await _identityService.GetUserByIdAsync(patient.UserId.Value, cancellationToken);
            if (user is null || string.IsNullOrWhiteSpace(user.Email)) return;

            var patientName = string.IsNullOrWhiteSpace(user.FullName) ? "bệnh nhân" : user.FullName;
            var qrPayload =
                $"AURA-CLINIC-APPOINTMENT|{appointment.Id}|{appointment.PatientId}||{slot.Date:yyyy-MM-dd}|{slot.StartTime:HH:mm}|{slot.EndTime:HH:mm}";
            var checkInCode = appointment.Id.ToString("N")[..10].ToUpperInvariant();

            await _emailService.SendClinicAppointmentConfirmationAsync(
                user.Email,
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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send rebook confirmation email for appointment {AppointmentId}", appointment.Id);
        }
    }
}
