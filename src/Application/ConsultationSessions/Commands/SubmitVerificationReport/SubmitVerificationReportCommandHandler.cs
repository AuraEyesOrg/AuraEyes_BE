using Application.Common.Interfaces;
using Application.Common.Models;
using Application.PatientRoadmaps.Common;
using System.Text.Json;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;

namespace Application.ConsultationSessions.Commands.SubmitVerificationReport;

public class SubmitVerificationReportCommandHandler
    : ICommandHandler<SubmitVerificationReportCommand>
{
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IRepository<MedicalDiagnosis> _diagnosisRepository;
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<PatientRoadmap> _roadmapRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IPatientRoadmapGenerationService _roadmapGenerationService;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitVerificationReportCommandHandler(
        IConsultationSessionRepository sessionRepository,
        IRepository<MedicalDiagnosis> diagnosisRepository,
        IRepository<AiScreening> screeningRepository,
        IRepository<PatientRoadmap> roadmapRepository,
        IRepository<Patient> patientRepository,
        IPatientRoadmapGenerationService roadmapGenerationService,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _sessionRepository = sessionRepository;
        _diagnosisRepository = diagnosisRepository;
        _screeningRepository = screeningRepository;
        _roadmapRepository = roadmapRepository;
        _patientRepository = patientRepository;
        _roadmapGenerationService = roadmapGenerationService;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        SubmitVerificationReportCommand request,
        CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
        if (session is null)
            return Result.NotFound($"Session '{request.SessionId}' not found.");

        if (session.Type != ConsultationSessionType.Verification)
            return Result.Failure("Only verification sessions accept reports.");

        if (session.OphthalmologistId.HasValue && session.OphthalmologistId.Value != request.DoctorId)
            return Result.Forbidden("You are not assigned to this session.");

        if (!session.AiScreeningId.HasValue)
            return Result.Failure("Session has no linked AI screening.");

        var diagnosisCode = string.IsNullOrWhiteSpace(request.DiagnosisCode)
            ? request.DiagnosesCode
            : request.DiagnosisCode;

        var clinicalFindings = string.IsNullOrWhiteSpace(request.ClinicalFindings)
            ? request.DiagnosesText
            : request.ClinicalFindings;

        var screening = await _screeningRepository.GetByIdAsync(session.AiScreeningId.Value, cancellationToken);
        if (screening is null)
            return Result.NotFound($"AI screening '{session.AiScreeningId.Value}' was not found.");

        if (string.IsNullOrWhiteSpace(screening.RawJsonOutput))
            return Result.Failure("AI screening output is unavailable for roadmap generation.");

        var generatedRoadmap = await _roadmapGenerationService.GenerateFromDiagnosisAsync(
            new PatientRoadmapGenerationInput
            {
                PatientId = session.PatientId,
                ScreeningId = screening.Id,
                AiScreeningRawJson = screening.RawJsonOutput,
                DiagnosisCode = diagnosisCode,
                CodingSystem = request.CodingSystem,
                ClinicalFindings = clinicalFindings,
                SeverityLevel = request.SeverityLevel,
                ConfidenceLevel = request.ConfidenceLevel,
                TreatmentPlan = request.TreatmentPlan,
                Recommendations = request.Recommendations,
                LifestyleAdvice = request.LifestyleAdvice,
                IsUrgent = request.IsUrgent,
                Status = request.Status,
                FollowUpDate = request.FollowUpDate,
                IsReferralNeeded = request.IsReferralNeeded
            },
            cancellationToken);

        if (!generatedRoadmap.IsSuccess || generatedRoadmap.Data is null)
            return Result.Failure(generatedRoadmap.ErrorMessage);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var diagnosis = new MedicalDiagnosis(
                    session.AiScreeningId.Value,
                    request.DoctorId,
                    session.Id,
                    diagnosisCode,
                    request.CodingSystem,
                    clinicalFindings,
                    request.SeverityLevel,
                    request.ConfidenceLevel,
                    request.TreatmentPlan,
                    request.Recommendations,
                    request.LifestyleAdvice,
                    request.IsUrgent,
                    request.Status,
                    request.FollowUpDate,
                    request.IsReferralNeeded,
                    request.FinalizedAt);

            await _diagnosisRepository.AddAsync(diagnosis, cancellationToken);

            var roadmap = new PatientRoadmap(
                session.PatientId,
                diagnosis.Id,
                generatedRoadmap.Data.RiskLevel,
                generatedRoadmap.Data.Summary,
                JsonSerializer.Serialize(generatedRoadmap.Data.NextSteps),
                JsonSerializer.Serialize(generatedRoadmap.Data.LifestyleAdvice),
                JsonSerializer.Serialize(generatedRoadmap.Data.WarningSigns),
                generatedRoadmap.Data.FollowUpNeeded,
                generatedRoadmap.Data.FollowUpTimeframe,
                generatedRoadmap.Data.RawAiResponse,
                "AI",
                DateTime.UtcNow);

            await _roadmapRepository.AddAsync(roadmap, cancellationToken);

            session.OpenChat();
            await _sessionRepository.UpdateAsync(session, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var patient = await _patientRepository.GetByIdAsync(session.PatientId, cancellationToken);
            if (patient is not null)
            {
                // Send real-time notification to Patient [FR-46]
                await _notificationService.SendAsync(
                    patient.UserId,
                    "Kết quả tư vấn đã sẵn sàng",
                    "Bác sĩ đã hoàn tất báo cáo xác minh kết quả sàng lọc của bạn. Bạn có thể xem chi tiết và trao đổi trực tiếp với bác sĩ.",
                    NotificationType.ConsultationResultProvided,
                    new { ConsultationId = session.Id, DoctorId = request.DoctorId },
                    cancellationToken);
            }

            return Result.Success();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
