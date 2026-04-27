using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Commands.CompleteClinicAppointment;

public class CompleteClinicAppointmentCommandHandler : ICommandHandler<CompleteClinicAppointmentCommand>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IPatientVisitRepository _patientVisitRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteClinicAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IPatientVisitRepository patientVisitRepository,
        IRepository<Patient> patientRepository,
        IConsultationSessionRepository sessionRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _patientVisitRepository = patientVisitRepository;
        _patientRepository = patientRepository;
        _sessionRepository = sessionRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CompleteClinicAppointmentCommand request, CancellationToken cancellationToken)
    {
        var visit = await _patientVisitRepository.GetByAppointmentIdAsync(request.AppointmentId, cancellationToken);
        if (visit is null)
        {
            return Result.NotFound("Patient visit not found. Check-in is required first.");
        }

        var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(request.AppointmentId, cancellationToken);
        if (appointment is null)
        {
            return Result.NotFound("Appointment not found.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Transition visit to WaitingForPayment instead of Completed
            // The actual Completion will happen after the financial system confirms payment
            visit.FinishConsultation(request.Notes);
            
            // Appointment can be marked as InProgress or stay as is, 
            // but we'll mark it as Completed when the Visit is truly done after payment.
            // For now, let's keep it InProgress to indicate it's not archived yet.
            // Actually, appointment.Complete() is fine if we consider 'Appointment' the booking part.
            // But let's keep it consistent with the Visit status.
            
            await _patientVisitRepository.UpdateAsync(visit, cancellationToken);
            // appointment.Complete(); // Don't complete appointment yet if visit is not done
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

            // ── 8. Create Consultation Chat Session ───────────────────────────
            // This allows the patient to chat with the doctor for 14 days post-visit
            if (appointment.AppointmentSlot?.ScheduleTemplate != null)
            {
                var session = ConsultationSession.CreateClinicBooking(
                    appointment.PatientId,
                    0, // Clinic sessions are already paid or handled at clinic
                    DateTime.UtcNow,
                    visit.AssignedDoctorId.GetValueOrDefault());

                session.OpenChat();
                
                await _sessionRepository.AddAsync(session, cancellationToken);

                // Notify patient if they are a registered user
                var patient = await _patientRepository.GetByIdAsync(appointment.PatientId, cancellationToken);
                if (patient?.UserId != null)
                {
                    await _notificationService.SendAsync(
                        patient.UserId.Value,
                        "Kết quả khám lâm sàng",
                        "Khám lâm sàng của bạn đã hoàn tất. Bạn có thể trao đổi thêm với bác sĩ trong vòng 14 ngày qua mục Chat.",
                        NotificationType.ConsultationResultProvided,
                        new { ConsultationId = session.Id, AppointmentId = appointment.Id },
                        cancellationToken,
                        session.Id);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure(ex.Message);
        }
    }
}
