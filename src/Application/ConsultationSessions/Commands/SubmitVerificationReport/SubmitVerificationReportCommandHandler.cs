using Application.Common.Interfaces;
using Application.Common.Models;

using System.Text.Json;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Screening;
using Domain.Entities.Scheduling;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Application.ConsultationSessions.Commands.SubmitVerificationReport;

public class SubmitVerificationReportCommandHandler
    : ICommandHandler<SubmitVerificationReportCommand>
{
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IRepository<MedicalDiagnosis> _diagnosisRepository;
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly INotificationService _notificationService;
    private readonly IIdentityService _identityService;
    private readonly IPatientVisitRepository _patientVisitRepository;
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitVerificationReportCommandHandler(
        IConsultationSessionRepository sessionRepository,
        IRepository<MedicalDiagnosis> diagnosisRepository,
        IRepository<AiScreening> screeningRepository,
        IRepository<Patient> patientRepository,
        INotificationService notificationService,
        IIdentityService identityService,
        IPatientVisitRepository patientVisitRepository,
        IMedicalRecordRepository medicalRecordRepository,
        IUnitOfWork unitOfWork)
    {
        _sessionRepository = sessionRepository;
        _diagnosisRepository = diagnosisRepository;
        _screeningRepository = screeningRepository;
        _patientRepository = patientRepository;
        _notificationService = notificationService;
        _identityService = identityService;
        _patientVisitRepository = patientVisitRepository;
        _medicalRecordRepository = medicalRecordRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        SubmitVerificationReportCommand request,
        CancellationToken cancellationToken)
    {
        var sessionResult = await ValidateSessionAsync(request, cancellationToken);
        if (!sessionResult.IsSuccess) return sessionResult.ToResult();
        var session = sessionResult.Data;

        var prescriptionResult = ProcessPrescription(request);
        if (!prescriptionResult.IsSuccess) return prescriptionResult.ToResult();
        var (isFinalized, finalizedAtUtc, prescriptionSnapshot, normalizedItems) = prescriptionResult.Data;

        var doctorUser = await _identityService.GetUserByIdAsync(request.DoctorId, cancellationToken);
        var doctorName = doctorUser?.FullName?.Trim();

        var diagnosisSnapshot = CreateDiagnosisSnapshot(request, normalizedItems, doctorName, finalizedAtUtc);
        var diagnosisSnapshotJson = JsonSerializer.Serialize(diagnosisSnapshot);

        var normalizedLifestyleAdvice = CombineLifestyleAdvice(request, prescriptionSnapshot, isFinalized, diagnosisSnapshotJson);

        var screening = await _screeningRepository.GetByIdAsync(session.AiScreeningId!.Value, cancellationToken);
        if (screening is null) return Result.NotFound($"AI screening '{session.AiScreeningId.Value}' was not found.");
        if (string.IsNullOrWhiteSpace(screening.RawJsonOutput)) return Result.Failure("AI screening output is unavailable for roadmap generation.");

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var diagnosis = new MedicalDiagnosis(session.AiScreeningId.Value, request.DoctorId, session.Id, request.DiagnosisCode ?? request.DiagnosesCode, request.CodingSystem, request.ClinicalFindings ?? request.DiagnosesText, request.SeverityLevel, request.ConfidenceLevel, request.TreatmentPlan, request.Recommendations, normalizedLifestyleAdvice, request.IsUrgent, request.Status, request.FollowUpDate, request.IsReferralNeeded, finalizedAtUtc);
            await _diagnosisRepository.AddAsync(diagnosis, cancellationToken);

            session.OpenChat();
            await _sessionRepository.UpdateAsync(session, cancellationToken);

            var visitToComplete = await ResolveVisitAsync(session, screening, isFinalized, cancellationToken);
            var patient = await _patientRepository.GetByIdAsync(session.PatientId, cancellationToken);

            if (visitToComplete != null && isFinalized)
            {
                await CompleteVisitAsync(visitToComplete, patient, session, diagnosis, request.DoctorId, doctorName, normalizedItems, finalizedAtUtc, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            await NotifyPatientAsync(patient, session, request.DoctorId, cancellationToken);

            return Result.Success();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private async Task<Result<ConsultationSession>> ValidateSessionAsync(SubmitVerificationReportCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
        if (session is null) return Result<ConsultationSession>.NotFound($"Session '{request.SessionId}' not found.");

        var acceptsReport = session.Type == ConsultationSessionType.Verification || session.Type == ConsultationSessionType.VideoCall || session.Type == ConsultationSessionType.ClinicBooking;
        if (!acceptsReport) return Result<ConsultationSession>.Failure("Only verification, video call, or clinic booking sessions accept reports.");

        if (session.OphthalmologistId.HasValue && session.OphthalmologistId.Value != request.DoctorId)
            return Result<ConsultationSession>.Forbidden("You are not assigned to this session.");

        if (!session.AiScreeningId.HasValue)
            return Result<ConsultationSession>.Failure("Session has no linked AI screening.");

        return Result<ConsultationSession>.Success(session);
    }

    private static Result<(bool IsFinalized, DateTime? FinalizedAt, string? Snapshot, List<PrescriptionItemInput> Items)> ProcessPrescription(SubmitVerificationReportCommand request)
    {
        var isFinalized = request.Status?.Equals("Finalized", StringComparison.OrdinalIgnoreCase) == true;
        var finalizedAtUtc = request.FinalizedAt ?? (isFinalized ? DateTime.UtcNow : null);

        var normalizedItems = (request.PrescriptionItems ?? Array.Empty<PrescriptionItemInput>())
            .Select(item => new PrescriptionItemInput
            {
                MedicineName = item.MedicineName?.Trim(),
                Unit = item.Unit?.Trim(),
                Dosage = item.Dosage?.Trim(),
                Frequency = item.Frequency?.Trim(),
                Duration = item.Duration?.Trim(),
                Instruction = item.Instruction?.Trim()
            })
            .Where(item => !string.IsNullOrWhiteSpace(item.MedicineName) || !string.IsNullOrWhiteSpace(item.Unit) || !string.IsNullOrWhiteSpace(item.Dosage) || !string.IsNullOrWhiteSpace(item.Frequency) || !string.IsNullOrWhiteSpace(item.Duration) || !string.IsNullOrWhiteSpace(item.Instruction))
            .ToList();

        if (isFinalized && !request.NoMedicationPrescribed && normalizedItems.Count == 0)
            return Result<(bool, DateTime?, string?, List<PrescriptionItemInput>)>.Failure("At least one prescription item is required to finalize this report.");

        if (isFinalized && !request.NoMedicationPrescribed && normalizedItems.Any(item => string.IsNullOrWhiteSpace(item.MedicineName) || string.IsNullOrWhiteSpace(item.Dosage) || string.IsNullOrWhiteSpace(item.Frequency) || string.IsNullOrWhiteSpace(item.Duration)))
            return Result<(bool, DateTime?, string?, List<PrescriptionItemInput>)>.Failure("Each prescription item must include medicine name, dosage, frequency, and duration.");

        var snapshot = normalizedItems.Count > 0 || request.NoMedicationPrescribed || !string.IsNullOrWhiteSpace(request.PrescriptionNote)
            ? JsonSerializer.Serialize(new { request.NoMedicationPrescribed, PrescriptionNote = request.PrescriptionNote?.Trim(), Items = normalizedItems })
            : null;

        return Result<(bool, DateTime?, string?, List<PrescriptionItemInput>)>.Success((isFinalized, finalizedAtUtc, snapshot, normalizedItems));
    }

    private static FinalizedDiagnosisSnapshot CreateDiagnosisSnapshot(SubmitVerificationReportCommand request, List<PrescriptionItemInput> items, string? doctorName, DateTime? finalizedAt)
    {
        return new FinalizedDiagnosisSnapshot
        {
            DiagnosisCode = (request.DiagnosisCode ?? request.DiagnosesCode)?.Trim(),
            CodingSystem = request.CodingSystem?.Trim(),
            ClinicalFindings = (request.ClinicalFindings ?? request.DiagnosesText)?.Trim(),
            SeverityLevel = request.SeverityLevel?.Trim(),
            PrescriptionItems = items.Select(item => new PrescriptionItemSnapshot { MedicineName = item.MedicineName ?? string.Empty, Unit = item.Unit, Dosage = item.Dosage ?? string.Empty, Frequency = item.Frequency ?? string.Empty, Duration = item.Duration ?? string.Empty, Instruction = item.Instruction }).ToList(),
            PrescriptionNote = request.PrescriptionNote?.Trim(),
            NoMedicationPrescribed = request.NoMedicationPrescribed,
            Recommendations = request.Recommendations?.Trim(),
            FollowUpDate = request.FollowUpDate,
            DiagnosedBy = new DiagnosedBySnapshot { DoctorId = request.DoctorId, DoctorName = doctorName },
            FinalizedAt = finalizedAt
        };
    }

    private static string? CombineLifestyleAdvice(SubmitVerificationReportCommand request, string? prescriptionSnapshot, bool isFinalized, string diagnosisSnapshotJson)
    {
        var advice = request.LifestyleAdvice?.Trim();
        if (!string.IsNullOrWhiteSpace(prescriptionSnapshot))
        {
            advice = string.IsNullOrWhiteSpace(advice) ? $"PRESCRIPTION_JSON::{prescriptionSnapshot}" : $"{advice}\n\nPRESCRIPTION_JSON::{prescriptionSnapshot}";
        }
        if (isFinalized)
        {
            advice = string.IsNullOrWhiteSpace(advice) ? $"DIAGNOSIS_SNAPSHOT_JSON::{diagnosisSnapshotJson}" : $"{advice}\n\nDIAGNOSIS_SNAPSHOT_JSON::{diagnosisSnapshotJson}";
        }
        return advice;
    }

    private async Task<PatientVisit?> ResolveVisitAsync(ConsultationSession session, AiScreening screening, bool isFinalized, CancellationToken cancellationToken)
    {
        if (screening.PatientVisitId.HasValue)
        {
            var visit = await _patientVisitRepository.GetByIdAsync(screening.PatientVisitId.Value, cancellationToken);
            if (visit != null && visit.PatientId == session.PatientId) return visit;
        }

        var visitIdFromMr = await _medicalRecordRepository.Query().Where(mr => mr.ConsultationSessionId == session.Id && mr.PatientVisitId != null).Select(mr => mr.PatientVisitId!.Value).FirstOrDefaultAsync(cancellationToken);
        if (visitIdFromMr != Guid.Empty)
        {
            var visit = await _patientVisitRepository.GetByIdAsync(visitIdFromMr, cancellationToken);
            if (visit != null && visit.PatientId == session.PatientId) return visit;
        }

        if (isFinalized)
        {
            return await _patientVisitRepository.Query().Where(v => v.PatientId == session.PatientId && (v.Status == PatientVisitStatus.CheckedIn || v.Status == PatientVisitStatus.InProgress)).OrderByDescending(v => v.CheckedInAt ?? v.CreatedAt).FirstOrDefaultAsync(cancellationToken);
        }

        return null;
    }

    private async Task CompleteVisitAsync(PatientVisit visit, Patient? patient, ConsultationSession session, MedicalDiagnosis diagnosis, Guid doctorId, string? doctorName, List<PrescriptionItemInput> items, DateTime? finalizedAt, CancellationToken cancellationToken)
    {
        if (visit.Status == PatientVisitStatus.CheckedIn) visit.Start();
        visit.FinishConsultation(diagnosis.ClinicalFindings);
        await _patientVisitRepository.UpdateAsync(visit, cancellationToken);

        var patientName = patient?.FullName;
        if (string.IsNullOrEmpty(patientName) && patient?.UserId != null)
        {
            var userDto = await _identityService.GetUserByIdAsync(patient.UserId.Value, cancellationToken);
            patientName = userDto?.FullName;
        }
        patientName ??= "Bệnh nhân";

        await _notificationService.SendToRoleAsync(
            roleName: Application.Common.Constants.Roles.ClinicStaff,
            title: "Ready for Payment",
            message: $"Consultation finished. Patient {patientName} is waiting for payment.",
            type: NotificationType.SystemAlert,
            payload: new
            {
                Action = "cashier_payment_ready",
                VisitId = visit.Id,
                PatientId = visit.PatientId,
                ConsultationSessionId = session.Id,
                ScreeningId = session.AiScreeningId,
                DiagnosisId = diagnosis.Id,
                DoctorId = doctorId,
                DoctorName = doctorName,
                HasPrescription = items.Count > 0 && !diagnosis.LifestyleAdvice?.Contains("NoMedicationPrescribed\":true") == false,
                NoMedicationPrescribed = diagnosis.LifestyleAdvice?.Contains("NoMedicationPrescribed\":true") == true,
                FinalizedAt = finalizedAt?.ToString("O", CultureInfo.InvariantCulture)
            },
            cancellationToken: cancellationToken
        );
    }

    private async Task NotifyPatientAsync(Patient? patient, ConsultationSession session, Guid doctorId, CancellationToken cancellationToken)
    {
        if (patient?.UserId.HasValue == true)
        {
            await _notificationService.SendAsync(
                patient.UserId.Value,
                "Kết quả tư vấn đã sẵn sàng",
                "Bác sĩ đã hoàn tất báo cáo xác minh kết quả sàng lọc của bạn. Bạn có thể xem chi tiết và trao đổi trực tiếp với bác sĩ.",
                NotificationType.ConsultationResultProvided,
                new { ConsultationId = session.Id, DoctorId = doctorId },
                cancellationToken);
        }
    }
}

public sealed class FinalizedDiagnosisSnapshot
{
    public string? DiagnosisCode { get; init; }
    public string? CodingSystem { get; init; }
    public string? ClinicalFindings { get; init; }
    public string? SeverityLevel { get; init; }
    public IReadOnlyList<PrescriptionItemSnapshot> PrescriptionItems { get; init; } = Array.Empty<PrescriptionItemSnapshot>();
    public string? PrescriptionNote { get; init; }
    public bool NoMedicationPrescribed { get; init; }
    public string? Recommendations { get; init; }
    public DateTime? FollowUpDate { get; init; }
    public DiagnosedBySnapshot DiagnosedBy { get; init; } = new();
    public DateTime? FinalizedAt { get; init; }
}

public sealed class PrescriptionItemSnapshot
{
    public string MedicineName { get; init; } = string.Empty;
    public string? Unit { get; init; }
    public string Dosage { get; init; } = string.Empty;
    public string Frequency { get; init; } = string.Empty;
    public string Duration { get; init; } = string.Empty;
    public string? Instruction { get; init; }
}

public sealed class DiagnosedBySnapshot
{
    public Guid DoctorId { get; init; }
    public string? DoctorName { get; init; }
}
