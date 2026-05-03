using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Financial;
using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class ClinicVisitService : IClinicVisitService
{
    private readonly IPatientVisitRepository _patientVisitRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IRepository<Domain.Entities.Users.Patient> _patientRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly INotificationService _notificationService;
    private readonly IChatHubService _chatHubService;
    private readonly ILogger<ClinicVisitService> _logger;

    public ClinicVisitService(
        IPatientVisitRepository patientVisitRepository,
        IAppointmentRepository appointmentRepository,
        IConsultationSessionRepository sessionRepository,
        IRepository<Domain.Entities.Users.Patient> patientRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        INotificationService notificationService,
        IChatHubService chatHubService,
        ILogger<ClinicVisitService> logger)
    {
        _patientVisitRepository = patientVisitRepository;
        _appointmentRepository = appointmentRepository;
        _sessionRepository = sessionRepository;
        _patientRepository = patientRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _notificationService = notificationService;
        _chatHubService = chatHubService;
        _logger = logger;
    }

    public async Task ProcessPaymentCompletionAsync(Order order, string paymentMethod, CancellationToken cancellationToken)
    {
        try
        {
            var visit = await ResolveVisitAsync(order, cancellationToken);
            if (visit == null || visit.Status != PatientVisitStatus.WaitingForPayment) return;

            _logger.LogInformation("Processing clinic visit completion for Visit {VisitId} after {Method} payment.", visit.Id, paymentMethod);

            visit.Complete($"Payment received via {paymentMethod}");
            await _patientVisitRepository.UpdateAsync(visit, cancellationToken);

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

    private async Task<PatientVisit?> ResolveVisitAsync(Order order, CancellationToken cancellationToken)
    {
        if (order.AppointmentId.HasValue)
        {
            return await _patientVisitRepository.GetByAppointmentIdAsync(order.AppointmentId.Value, cancellationToken);
        }

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
            "Thanh toán hoàn tất. Bạn có thể trao đổi thêm với bác sĩ trong vòng 14 ngày qua mục Chat.",
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
}
