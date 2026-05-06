using Application.Common.Interfaces;
using Application.Common.Constants;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Financial;
using Domain.Entities.MedicalRecords;
using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Services;

public class ClinicVisitService : IClinicVisitService
{
    private readonly IPatientVisitRepository _patientVisitRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IRepository<Domain.Entities.Users.Patient> _patientRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly INotificationService _notificationService;
    private readonly IIdentityService _identityService;
    private readonly IChatHubService _chatHubService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ClinicVisitService> _logger;

    public ClinicVisitService(
        IPatientVisitRepository patientVisitRepository,
        IAppointmentRepository appointmentRepository,
        IAppointmentSlotRepository appointmentSlotRepository,
        IConsultationSessionRepository sessionRepository,
        IRepository<Domain.Entities.Users.Patient> patientRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IMedicalRecordRepository medicalRecordRepository,
        INotificationService notificationService,
        IIdentityService identityService,
        IChatHubService chatHubService,
        IUnitOfWork unitOfWork,
        ILogger<ClinicVisitService> logger)
    {
        _patientVisitRepository = patientVisitRepository;
        _appointmentRepository = appointmentRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
        _sessionRepository = sessionRepository;
        _patientRepository = patientRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _medicalRecordRepository = medicalRecordRepository;
        _notificationService = notificationService;
        _identityService = identityService;
        _chatHubService = chatHubService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task ProcessPaymentCompletionAsync(Order order, string paymentMethod, CancellationToken cancellationToken)
    {
        try
        {
            var visit = await ResolveVisitAsync(order, cancellationToken);
            
            // Special handling for WALK-IN appointments: 
            // If payment completes and no visit exists yet, it means it's the UPFRONT payment.
            // We should auto-check-in the patient.
            if (visit == null && order.AppointmentId.HasValue)
            {
                var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(order.AppointmentId.Value, cancellationToken);
                if (appointment != null)
                {
                    // Ensure Patient is loaded if not already
                    if (appointment.Patient == null)
                    {
                        var patient = await _patientRepository.GetByIdAsync(appointment.PatientId, cancellationToken);
                        if (patient != null)
                        {
                            // Using reflection or a private setter if necessary, but usually just setting the property works if public
                            // appointment.Patient = patient; 
                            // Actually, let's just check the patient record directly
                            if (patient.IsWalkIn && (appointment.Status == AppointmentStatus.Pending || appointment.Status == AppointmentStatus.Confirmed))
                            {
                                await AutoCheckInWalkInAsync(appointment, cancellationToken);
                                return;
                            }
                        }
                    }
                    else if (appointment.Patient.IsWalkIn && (appointment.Status == AppointmentStatus.Pending || appointment.Status == AppointmentStatus.Confirmed))
                    {
                        await AutoCheckInWalkInAsync(appointment, cancellationToken);
                        return;
                    }
                }
            }

            if (visit == null) return;
            
            // Allow both WaitingForPayment (PayOS flow) and Completed (Cash flow updated in handler)
            if (visit.Status != PatientVisitStatus.WaitingForPayment && visit.Status != PatientVisitStatus.Completed) return;

            _logger.LogInformation("Processing clinic visit completion for Visit {VisitId} after {Method} payment.", visit.Id, paymentMethod);

            visit.Complete($"Payment received via {paymentMethod}");
            await _patientVisitRepository.UpdateAsync(visit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (visit.AppointmentId.HasValue)
            {
                await HandleAppointmentAndConsultationAsync(visit, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process clinic visit completion for order {OrderId}.", order.Id);
        }
    }

    public async Task<Appointment?> GetAppointmentByIdAsync(Guid appointmentId, CancellationToken cancellationToken)
    {
        return await _appointmentRepository.GetByIdWithDetailsAsync(appointmentId, cancellationToken);
    }

    public async Task CancelAppointmentAsync(Guid appointmentId, string? reason, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(appointmentId, cancellationToken);
        if (appointment == null || appointment.Status == AppointmentStatus.Cancelled) return;

        appointment.Cancel(Guid.Empty, reason); // Guid.Empty for system/sync cancellation

        var slot = appointment.AppointmentSlot;
        if (slot != null)
        {
            slot.CancelBooking();
            await _appointmentSlotRepository.UpdateAsync(slot, cancellationToken);
        }

        await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Appointment {AppointmentId} cancelled and slot {SlotId} released. Reason: {Reason}", 
            appointmentId, slot?.Id, reason);
    }

    private async Task<PatientVisit?> ResolveVisitAsync(Order order, CancellationToken cancellationToken)
    {
        // 1. Try resolving by AppointmentId
        if (order.AppointmentId.HasValue)
        {
            var visitByAppt = await _patientVisitRepository.GetByAppointmentIdAsync(order.AppointmentId.Value, cancellationToken);
            // If appointment is present, we ONLY want the visit linked to IT.
            // If no visit exists for this appointment yet, we should return null to allow walk-in auto-check-in.
            return visitByAppt;
        }

        // 2. Try resolving by METADATA in description (V: VisitId)
        if (!string.IsNullOrEmpty(order.Description) && order.Description.Trim().StartsWith("METADATA:", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var description = order.Description.Trim();
                var parts = description.Split('|');
                if (parts.Length > 0)
                {
                    var metadataJson = parts[0].Substring("METADATA:".Length).Trim();
                    using var doc = JsonDocument.Parse(metadataJson);
                    if (doc.RootElement.TryGetProperty("V", out var vProp))
                    {
                        var visitId = vProp.GetGuid();
                        var visitByMetadata = await _patientVisitRepository.GetByIdAsync(visitId, cancellationToken);
                        if (visitByMetadata != null) return visitByMetadata;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse VisitId from order description metadata: {Description}", order.Description);
            }
        }

        // 3. Fallback: resolve by patient's latest WaitingForPayment visit
        var patient = await _patientRepository.Query()
            .FirstOrDefaultAsync(p => p.UserId == order.UserId, cancellationToken);

        if (patient == null) return null;

        return await _patientVisitRepository.Query()
            .OrderByDescending(v => v.CreatedAt)
            .FirstOrDefaultAsync(v =>
                v.PatientId == patient.Id &&
                v.Status == PatientVisitStatus.WaitingForPayment,
                cancellationToken);
    }

    private async Task HandleAppointmentAndConsultationAsync(PatientVisit visit, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(visit.AppointmentId!.Value, cancellationToken);
        if (appointment == null) return;

        appointment.Complete();
        await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

        var existingSession = await GetExistingSessionAsync(appointment, visit.AssignedDoctorId, cancellationToken);
        if (existingSession != null) return;

        var session = await CreateFollowUpSessionAsync(appointment, visit.AssignedDoctorId.GetValueOrDefault(), cancellationToken);
        await NotifyAndBroadcastSessionAsync(session, appointment, visit, cancellationToken);
    }

    private async Task<ConsultationSession?> GetExistingSessionAsync(Appointment appointment, Guid? doctorId, CancellationToken cancellationToken)
    {
        return await _sessionRepository.Query()
            .FirstOrDefaultAsync(s => s.PatientId == appointment.PatientId &&
                                 s.OphthalmologistId == doctorId &&
                                 s.ChatStatus != ChatStatus.Archived,
                                 cancellationToken);
    }

    private async Task<ConsultationSession> CreateFollowUpSessionAsync(Appointment appointment, Guid doctorId, CancellationToken cancellationToken)
    {
        var session = ConsultationSession.CreateClinicBooking(
            appointment.PatientId,
            0,
            DateTime.UtcNow,
            doctorId);

        session.OpenChat();
        await _sessionRepository.AddAsync(session, cancellationToken);
        return session;
    }

    private async Task NotifyAndBroadcastSessionAsync(ConsultationSession session, Appointment appointment, PatientVisit visit, CancellationToken cancellationToken)
    {
        var patientData = await _patientRepository.GetByIdAsync(appointment.PatientId, cancellationToken);
        if (patientData?.UserId != null)
        {
            await SendPatientNotificationAsync(patientData.UserId.Value, session.Id, appointment.Id, cancellationToken);
        }

        var participantUserIds = await GetParticipantUserIdsAsync(patientData, visit.AssignedDoctorId, cancellationToken);
        if (participantUserIds.Count > 0)
        {
            await BroadcastRoomUpdateAsync(participantUserIds, session.Id, "SessionCreated", cancellationToken);
        }

        await BroadcastRoomUpdateAsync(Application.Common.Constants.Roles.ClinicStaff, session.Id, "VisitPaymentCompleted", cancellationToken);
    }

    private async Task SendPatientNotificationAsync(Guid userId, Guid sessionId, Guid appointmentId, CancellationToken cancellationToken)
    {
        await _notificationService.SendAsync(
            userId,
            "Kết quả khám lâm sàng & Tư vấn",
            "Thanh toán hoàn tất. Bạn có thể trao đổi thêm với bác sĩ trong vòng 7 ngày qua mục Chat.",
            NotificationType.ConsultationResultProvided,
            new { ConsultationId = sessionId, AppointmentId = appointmentId },
            cancellationToken,
            sessionId);
    }

    private async Task<List<Guid>> GetParticipantUserIdsAsync(Domain.Entities.Users.Patient? patient, Guid? doctorId, CancellationToken cancellationToken)
    {
        var userIds = new List<Guid>();
        if (patient?.UserId != null) userIds.Add(patient.UserId.Value);

        if (doctorId.HasValue)
        {
            var ophthal = await _ophthalmologistRepository.GetByIdAsync(doctorId.Value, cancellationToken);
            if (ophthal?.UserId != null) userIds.Add(ophthal.UserId);
        }
        return userIds;
    }

    private async Task BroadcastRoomUpdateAsync(object target, Guid sessionId, string @event, CancellationToken cancellationToken)
    {
        var payload = new RoomStateChangedDto
        {
            SessionId = sessionId,
            Event = @event,
            Timestamp = DateTime.UtcNow
        };

        if (target is IEnumerable<Guid> userIds)
        {
            await _chatHubService.BroadcastRoomStateChangedAsync(userIds.ToList(), payload, cancellationToken);
        }
        else if (target is string role)
        {
            await _chatHubService.BroadcastRoomStateChangedAsync(role, payload, cancellationToken);
        }
    }

    private async Task AutoCheckInWalkInAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Walk-in appointment {AppointmentId} fully paid. Auto-checking in.", appointment.Id);

        // 1. Update appointment status
        appointment.CheckIn();
        await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

        // 2. Create visit record
        var newVisit = PatientVisit.CreateFromAppointment(appointment);
        await _patientVisitRepository.AddAsync(newVisit, cancellationToken);

        // 3. Create initial medical record, aligned with standard check-in flow.
        var medicalRecordNumber = $"MT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}";
        var medicalRecord = new MedicalRecord(newVisit.PatientId, medicalRecordNumber);
        medicalRecord.LinkToPatientVisit(newVisit.Id);
        await _medicalRecordRepository.AddAsync(medicalRecord, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Notify coordinator role in realtime (same behavior as manual check-in).
        string patientName = "Patient";
        if (appointment.Patient != null)
        {
            if (appointment.Patient.UserId.HasValue)
            {
                var user = await _identityService.GetUserByIdAsync(
                    appointment.Patient.UserId.Value,
                    cancellationToken);
                patientName = user?.FullName ?? "Patient";
            }
            else if (!string.IsNullOrWhiteSpace(appointment.Patient.FullName))
            {
                patientName = appointment.Patient.FullName;
            }
        }

        await _notificationService.SendToRoleAsync(
            roleName: Roles.ClinicStaff,
            title: "New Patient in Queue",
            message: $"{patientName} has checked in and is waiting for screening.",
            type: NotificationType.SystemAlert,
            payload: new { VisitId = newVisit.Id, PatientId = newVisit.PatientId },
            cancellationToken: cancellationToken
        );
    }
}
