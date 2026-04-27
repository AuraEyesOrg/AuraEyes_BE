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
            PatientVisit? visit = null;

            // Priority 1: Use AppointmentId if present
            if (order.AppointmentId.HasValue)
            {
                visit = await _patientVisitRepository.GetByAppointmentIdAsync(order.AppointmentId.Value, cancellationToken);
            }

            // Priority 2: Fallback to finding by Patient UserId
            if (visit == null)
            {
                var patient = await _patientRepository.Query()
                    .FirstOrDefaultAsync(p => p.UserId == order.UserId, cancellationToken);

                if (patient != null)
                {
                    visit = await _patientVisitRepository.Query()
                        .OrderByDescending(v => v.CreatedAt)
                        .FirstOrDefaultAsync(v => 
                            v.PatientId == patient.Id && 
                            v.Status == PatientVisitStatus.WaitingForPayment, 
                            cancellationToken);
                }
            }
            
            if (visit != null && visit.Status == PatientVisitStatus.WaitingForPayment)
            {
                _logger.LogInformation("Processing clinic visit completion for Visit {VisitId} after {Method} payment.", visit.Id, paymentMethod);
                
                visit.Complete($"Payment received via {paymentMethod}");
                await _patientVisitRepository.UpdateAsync(visit, cancellationToken);

                if (visit.AppointmentId.HasValue)
                {
                    var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(visit.AppointmentId.Value, cancellationToken);
                    if (appointment != null)
                    {
                        appointment.Complete();
                        await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

                        // Check if consultation session already exists
                        var existingSession = await _sessionRepository.Query()
                            .FirstOrDefaultAsync(s => s.PatientId == appointment.PatientId && 
                                                 s.OphthalmologistId == visit.AssignedDoctorId &&
                                                 s.ChatStatus != ChatStatus.Archived, 
                                                 cancellationToken);

                        if (existingSession == null)
                        {
                            // Create Consultation Chat Session (Post-visit follow-up)
                            var session = ConsultationSession.CreateClinicBooking(
                                appointment.PatientId,
                                0,
                                DateTime.UtcNow,
                                visit.AssignedDoctorId.GetValueOrDefault());

                            session.OpenChat();
                            await _sessionRepository.AddAsync(session, cancellationToken);
                            
                            // Notify patient
                            var patientData = await _patientRepository.GetByIdAsync(appointment.PatientId, cancellationToken);
                            if (patientData?.UserId != null)
                            {
                                await _notificationService.SendAsync(
                                    patientData.UserId.Value,
                                    "Kết quả khám lâm sàng & Tư vấn",
                                    "Thanh toán hoàn tất. Bạn có thể trao đổi thêm với bác sĩ trong vòng 14 ngày qua mục Chat.",
                                    NotificationType.ConsultationResultProvided,
                                    new { ConsultationId = session.Id, AppointmentId = appointment.Id },
                                    cancellationToken,
                                    session.Id);
                            }

                            // Broadcast real-time update
                            var userIds = new List<Guid>();
                            if (patientData?.UserId != null) userIds.Add(patientData.UserId.Value);
                            
                            if (visit.AssignedDoctorId.HasValue)
                            {
                                var ophthal = await _ophthalmologistRepository.GetByIdAsync(visit.AssignedDoctorId.Value, cancellationToken);
                                if (ophthal?.UserId != null) userIds.Add(ophthal.UserId);
                            }

                            if (userIds.Count > 0)
                            {
                                await _chatHubService.BroadcastRoomStateChangedAsync(
                                    userIds,
                                    new RoomStateChangedDto
                                    {
                                        SessionId = session.Id,
                                        Event = "SessionCreated",
                                        Timestamp = DateTime.UtcNow
                                    }, cancellationToken);
                            }

                            // Also notify Clinic Staff role for UI refresh
                            await _chatHubService.BroadcastRoomStateChangedAsync(
                                Application.Common.Constants.Roles.ClinicStaff,
                                new RoomStateChangedDto
                                {
                                    SessionId = session.Id,
                                    Event = "VisitPaymentCompleted",
                                    Timestamp = DateTime.UtcNow
                                }, cancellationToken);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process clinic visit completion for order {OrderId}.", order.Id);
        }
    }
}
