using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Scheduling.Appointments.Commands.CreateAdHocSlotAndRebook;

public class CreateAdHocSlotAndRebookCommandHandler
    : ICommandHandler<CreateAdHocSlotAndRebookCommand, CreateAdHocSlotAndRebookResult>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateAdHocSlotAndRebookCommandHandler> _logger;

    public CreateAdHocSlotAndRebookCommandHandler(
        IAppointmentRepository appointmentRepository,
        IAppointmentSlotRepository appointmentSlotRepository,
        IRepository<Patient> patientRepository,
        IIdentityService identityService,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        ILogger<CreateAdHocSlotAndRebookCommandHandler> logger)
    {
        _appointmentRepository = appointmentRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
        _patientRepository = patientRepository;
        _identityService = identityService;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _logger = logger;
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

            await SendRebookConfirmationEmailAsync(newAppointment, adHocSlot, cancellationToken);

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
            _logger.LogError(ex, "Failed to send ad-hoc rebook confirmation email for appointment {AppointmentId}", appointment.Id);
        }
    }
}
