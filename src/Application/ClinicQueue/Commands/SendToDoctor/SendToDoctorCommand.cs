using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Scheduling;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.ClinicQueue.Commands.SendToDoctor;

public record SendToDoctorCommand : ICommand<SendToDoctorResponse>
{
    public Guid VisitId { get; init; }
    public Guid ScreeningId { get; init; }
    public Guid? DoctorId { get; init; }
    public Guid RequestedByUserId { get; init; }
    public string? Notes { get; init; }
}

public class SendToDoctorCommandHandler
    : ICommandHandler<SendToDoctorCommand, SendToDoctorResponse>
{
    private readonly IPatientVisitRepository _patientVisitRepository;
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IConsultationSessionRepository _consultationSessionRepository;
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IRepository<Ophthalmologist> _ophthalmologistRepository;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public SendToDoctorCommandHandler(
        IPatientVisitRepository patientVisitRepository,
        IRepository<AiScreening> screeningRepository,
        IConsultationSessionRepository consultationSessionRepository,
        IMedicalRecordRepository medicalRecordRepository,
        IRepository<Ophthalmologist> ophthalmologistRepository,
        IIdentityService identityService,
        IUnitOfWork unitOfWork,
        INotificationService notificationService)
    {
        _patientVisitRepository = patientVisitRepository;
        _screeningRepository = screeningRepository;
        _consultationSessionRepository = consultationSessionRepository;
        _medicalRecordRepository = medicalRecordRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<Result<SendToDoctorResponse>> Handle(
        SendToDoctorCommand request,
        CancellationToken cancellationToken)
    {
        if (request.RequestedByUserId == Guid.Empty)
            return Result<SendToDoctorResponse>.Failure("Invalid requester.");

        // Digital Clinic model: No need to check for organisation association.

        var visit = await _patientVisitRepository
            .Query()
            .Include(v => v.Patient)
            .Include(v => v.Appointment)
                .ThenInclude(a => a!.AppointmentSlot)
                    .ThenInclude(s => s!.ScheduleTemplate)
            .FirstOrDefaultAsync(v => v.Id == request.VisitId, cancellationToken);

        if (visit == null)
            return Result<SendToDoctorResponse>.NotFound("Visit not found.");

        if (visit.Status == PatientVisitStatus.Completed)
            return Result<SendToDoctorResponse>.Failure("Cannot send completed visit to doctor.");

        var screening = await _screeningRepository
            .Query()
            .Include(s => s.ScreeningResults)
            .FirstOrDefaultAsync(
                s => s.Id == request.ScreeningId && !s.IsDeleted,
                cancellationToken);

        if (screening == null)
            return Result<SendToDoctorResponse>.NotFound("Screening not found.");

        if (screening.PatientId != visit.PatientId)
            return Result<SendToDoctorResponse>.Failure("Screening does not belong to this patient.");

        if (screening.PatientVisitId.HasValue && screening.PatientVisitId.Value != visit.Id)
            return Result<SendToDoctorResponse>.Failure("Screening is linked to a different visit.");

        if (!screening.ScreeningResults.Any())
            return Result<SendToDoctorResponse>.Failure("Screening has no AI results. Please run AI analysis first.");

        Ophthalmologist? doctor = null;
        if (request.DoctorId.HasValue)
        {
            doctor = await _ophthalmologistRepository.GetByIdAsync(
                request.DoctorId.Value,
                cancellationToken);

            if (doctor == null)
                return Result<SendToDoctorResponse>.NotFound("Specified doctor not found.");
        }

        var existingConsultation = await _consultationSessionRepository
            .Query()
            .FirstOrDefaultAsync(cs =>
                cs.PatientId == visit.PatientId &&
                cs.AiScreeningId == request.ScreeningId &&
                (cs.Type == ConsultationSessionType.Verification ||
                 cs.Type == ConsultationSessionType.VideoCall ||
                 cs.Type == ConsultationSessionType.ClinicBooking) &&
                cs.Status != SessionStatus.Cancelled,
                cancellationToken);

        if (existingConsultation != null)
        {
            var sessionBoundToAnotherVisit = await _medicalRecordRepository
                .Query()
                .AnyAsync(
                    mr =>
                        mr.ConsultationSessionId == existingConsultation.Id &&
                        mr.PatientVisitId.HasValue &&
                        mr.PatientVisitId.Value != visit.Id,
                    cancellationToken);

            if (sessionBoundToAnotherVisit)
            {
                return Result<SendToDoctorResponse>.Failure(
                    "This screening is already linked to another check-in (visit). Create a new screening for this visit, or open the correct queue row.");
            }

            if (request.DoctorId.HasValue &&
                existingConsultation.OphthalmologistId != request.DoctorId.Value)
            {
                existingConsultation.AssignDoctor(request.DoctorId.Value);
                visit.AssignDoctor(request.DoctorId.Value);

                await _patientVisitRepository.UpdateAsync(visit, cancellationToken);

                var patientName = visit.Patient?.FullName;
                if (string.IsNullOrEmpty(patientName) && visit.Patient?.UserId != null)
                {
                    var userDto = await _identityService.GetUserByIdAsync(visit.Patient.UserId.Value, cancellationToken);
                    patientName = userDto?.FullName;
                }
                patientName ??= "Bệnh nhân";

                await _notificationService.SendAsync(
                    userId: doctor!.UserId,
                    title: "New Case Assigned",
                    message: $"You have been assigned a new case for patient {patientName}.",
                    type: NotificationType.NewConsultationRequest,
                    payload: new
                    {
                        ConsultationSessionId = existingConsultation.Id,
                        VisitId = visit.Id,
                        ScreeningId = request.ScreeningId,
                        RouteHint = $"/ophthalmologist/screenings/{request.ScreeningId}/review"
                    },
                    cancellationToken: cancellationToken,
                    referenceId: existingConsultation.Id
                );
            }

            await _consultationSessionRepository.UpdateAsync(existingConsultation, cancellationToken);
            
            // Link existing medical record to session if not already linked
            var medicalRecord = await ((Domain.Repositories.IMedicalRecordRepository)_medicalRecordRepository).GetByVisitIdAsync(visit.Id, cancellationToken);
            if (medicalRecord != null)
            {
                medicalRecord.LinkToConsultation(existingConsultation.Id);
                medicalRecord.StartDoctorFilling();
                await _medicalRecordRepository.UpdateAsync(medicalRecord, cancellationToken);
            }

            StartConsultationFlow(visit);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<SendToDoctorResponse>.Success(new SendToDoctorResponse
            {
                ConsultationSessionId = existingConsultation.Id,
                AssignedDoctorId = existingConsultation.OphthalmologistId,
                Message = "Consultation session already exists."
            });
        }

        var consultationSession = ConsultationSession.CreateVerification(
            patientId: visit.PatientId,
            aiScreeningId: request.ScreeningId,
            ophthalmologistId: request.DoctorId,
            price: 0m
        );

        await _consultationSessionRepository.AddAsync(consultationSession, cancellationToken);

        if (request.DoctorId.HasValue)
            visit.AssignDoctor(request.DoctorId.Value);

        await _patientVisitRepository.UpdateAsync(visit, cancellationToken);

        // Link medical record to the new session
        var mr = await ((Domain.Repositories.IMedicalRecordRepository)_medicalRecordRepository).GetByVisitIdAsync(visit.Id, cancellationToken);
        if (mr != null)
        {
            mr.LinkToConsultation(consultationSession.Id);
            mr.StartDoctorFilling();
            await _medicalRecordRepository.UpdateAsync(mr, cancellationToken);
        }

        StartConsultationFlow(visit);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (request.DoctorId.HasValue)
        {
            var patientName = visit.Patient?.FullName;
            if (string.IsNullOrEmpty(patientName) && visit.Patient?.UserId != null)
            {
                var userDto = await _identityService.GetUserByIdAsync(visit.Patient.UserId.Value, cancellationToken);
                patientName = userDto?.FullName;
            }
            patientName ??= "Bệnh nhân";

            await _notificationService.SendAsync(
                userId: doctor!.UserId, // doctor is queried earlier
                title: "New Case Assigned",
                message: $"You have been assigned a new case for patient {patientName}.",
                type: NotificationType.NewConsultationRequest, // or ConsultationAssigned if it exists
                payload: new
                {
                    ConsultationSessionId = consultationSession.Id,
                    VisitId = visit.Id,
                    ScreeningId = request.ScreeningId,
                    RouteHint = $"/ophthalmologist/screenings/{request.ScreeningId}/review"
                },
                cancellationToken: cancellationToken,
                referenceId: consultationSession.Id
            );
        }

        return Result<SendToDoctorResponse>.Success(new SendToDoctorResponse
        {
            ConsultationSessionId = consultationSession.Id,
            AssignedDoctorId = request.DoctorId,
            Message = "Case sent to doctor successfully."
        });
    }

    /// <summary>
    /// Canonical transition: coordinator sends patient to doctor — consultation phase begins.
    /// </summary>
    private static void StartConsultationFlow(PatientVisit visit)
    {
        if (visit.Status != PatientVisitStatus.CheckedIn)
            return;

        visit.Start();

        if (visit.Appointment is { Status: AppointmentStatus.CheckedIn })
            visit.Appointment.Start();
    }
}

public class SendToDoctorResponse
{
    public Guid ConsultationSessionId { get; set; }
    public Guid? AssignedDoctorId { get; set; }
    public string Message { get; set; } = string.Empty;
}
