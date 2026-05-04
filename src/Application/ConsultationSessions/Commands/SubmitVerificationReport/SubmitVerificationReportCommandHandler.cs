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
        var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
        if (session is null)
            return Result.NotFound($"Session '{request.SessionId}' not found.");

        var acceptsReport =
            session.Type == ConsultationSessionType.Verification ||
            session.Type == ConsultationSessionType.VideoCall ||
            session.Type == ConsultationSessionType.ClinicBooking;

        if (!acceptsReport)
            return Result.Failure("Only verification, video call, or clinic booking sessions accept reports.");

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
        var isFinalized = request.Status?.Equals("Finalized", StringComparison.OrdinalIgnoreCase) == true;
        var finalizedAtUtc = request.FinalizedAt ?? (isFinalized ? DateTime.UtcNow : null);
        var normalizedPrescriptionItems = (request.PrescriptionItems ?? Array.Empty<PrescriptionItemInput>())
            .Select(item => new
            {
                MedicineName = item.MedicineName?.Trim(),
                Unit = item.Unit?.Trim(),
                Dosage = item.Dosage?.Trim(),
                Frequency = item.Frequency?.Trim(),
                Duration = item.Duration?.Trim(),
                Instruction = item.Instruction?.Trim()
            })
            .Where(item =>
                !string.IsNullOrWhiteSpace(item.MedicineName) ||
                !string.IsNullOrWhiteSpace(item.Unit) ||
                !string.IsNullOrWhiteSpace(item.Dosage) ||
                !string.IsNullOrWhiteSpace(item.Frequency) ||
                !string.IsNullOrWhiteSpace(item.Duration) ||
                !string.IsNullOrWhiteSpace(item.Instruction))
            .ToList();

        if (isFinalized && !request.NoMedicationPrescribed && normalizedPrescriptionItems.Count == 0)
            return Result.Failure("At least one prescription item is required to finalize this report.");

        var hasInvalidPrescriptionRow = normalizedPrescriptionItems.Any(item =>
            string.IsNullOrWhiteSpace(item.MedicineName) ||
            string.IsNullOrWhiteSpace(item.Dosage) ||
            string.IsNullOrWhiteSpace(item.Frequency) ||
            string.IsNullOrWhiteSpace(item.Duration));
        if (isFinalized && !request.NoMedicationPrescribed && hasInvalidPrescriptionRow)
            return Result.Failure("Each prescription item must include medicine name, dosage, frequency, and duration.");

        var prescriptionSnapshot = normalizedPrescriptionItems.Count > 0 || request.NoMedicationPrescribed || !string.IsNullOrWhiteSpace(request.PrescriptionNote)
            ? JsonSerializer.Serialize(new
            {
                request.NoMedicationPrescribed,
                PrescriptionNote = request.PrescriptionNote?.Trim(),
                Items = normalizedPrescriptionItems
            })
            : null;

        var doctorUser = await _identityService.GetUserByIdAsync(request.DoctorId, cancellationToken);
        var doctorName = doctorUser?.FullName?.Trim();

        var diagnosisSnapshot = new FinalizedDiagnosisSnapshot
        {
            DiagnosisCode = diagnosisCode?.Trim(),
            CodingSystem = request.CodingSystem?.Trim(),
            ClinicalFindings = clinicalFindings?.Trim(),
            SeverityLevel = request.SeverityLevel?.Trim(),
            PrescriptionItems = normalizedPrescriptionItems
                .Select(item => new PrescriptionItemSnapshot
                {
                    MedicineName = item.MedicineName ?? string.Empty,
                    Unit = item.Unit,
                    Dosage = item.Dosage ?? string.Empty,
                    Frequency = item.Frequency ?? string.Empty,
                    Duration = item.Duration ?? string.Empty,
                    Instruction = item.Instruction
                })
                .ToList(),
            PrescriptionNote = request.PrescriptionNote?.Trim(),
            NoMedicationPrescribed = request.NoMedicationPrescribed,
            Recommendations = request.Recommendations?.Trim(),
            FollowUpDate = request.FollowUpDate,
            DiagnosedBy = new DiagnosedBySnapshot
            {
                DoctorId = request.DoctorId,
                DoctorName = doctorName
            },
            FinalizedAt = finalizedAtUtc
        };
        var diagnosisSnapshotJson = JsonSerializer.Serialize(diagnosisSnapshot);

        var normalizedLifestyleAdvice = request.LifestyleAdvice?.Trim();
        if (!string.IsNullOrWhiteSpace(prescriptionSnapshot))
        {
            normalizedLifestyleAdvice = string.IsNullOrWhiteSpace(normalizedLifestyleAdvice)
                ? $"PRESCRIPTION_JSON::{prescriptionSnapshot}"
                : $"{normalizedLifestyleAdvice}\n\nPRESCRIPTION_JSON::{prescriptionSnapshot}";
        }
        if (isFinalized)
        {
            normalizedLifestyleAdvice = string.IsNullOrWhiteSpace(normalizedLifestyleAdvice)
                ? $"DIAGNOSIS_SNAPSHOT_JSON::{diagnosisSnapshotJson}"
                : $"{normalizedLifestyleAdvice}\n\nDIAGNOSIS_SNAPSHOT_JSON::{diagnosisSnapshotJson}";
        }

        var screening = await _screeningRepository.GetByIdAsync(session.AiScreeningId.Value, cancellationToken);
        if (screening is null)
            return Result.NotFound($"AI screening '{session.AiScreeningId.Value}' was not found.");

        if (string.IsNullOrWhiteSpace(screening.RawJsonOutput))
            return Result.Failure("AI screening output is unavailable for roadmap generation.");

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
                    normalizedLifestyleAdvice,
                    request.IsUrgent,
                    request.Status,
                    request.FollowUpDate,
                    request.IsReferralNeeded,
                    finalizedAtUtc);

            await _diagnosisRepository.AddAsync(diagnosis, cancellationToken);

            session.OpenChat();
            await _sessionRepository.UpdateAsync(session, cancellationToken);

            // Resolve the visit for this session — never pick "any" active visit for the same patient.
            PatientVisit? visitToComplete = null;
            if (screening.PatientVisitId.HasValue)
            {
                visitToComplete = await _patientVisitRepository.GetByIdAsync(
                    screening.PatientVisitId.Value,
                    cancellationToken);
                if (visitToComplete is not null &&
                    visitToComplete.PatientId != session.PatientId)
                {
                    visitToComplete = null;
                }
            }

            if (visitToComplete is null)
            {
                var visitIdFromMr = await _medicalRecordRepository
                    .Query()
                    .Where(mr => mr.ConsultationSessionId == session.Id && mr.PatientVisitId != null)
                    .Select(mr => mr.PatientVisitId!.Value)
                    .FirstOrDefaultAsync(cancellationToken);

                if (visitIdFromMr != Guid.Empty)
                {
                    visitToComplete = await _patientVisitRepository.GetByIdAsync(
                        visitIdFromMr,
                        cancellationToken);
                    if (visitToComplete is not null &&
                        visitToComplete.PatientId != session.PatientId)
                    {
                        visitToComplete = null;
                    }
                }
            }

            if (visitToComplete is null && isFinalized)
            {
                visitToComplete = await _patientVisitRepository.Query()
                    .Where(v =>
                        v.PatientId == session.PatientId &&
                        (v.Status == PatientVisitStatus.CheckedIn ||
                         v.Status == PatientVisitStatus.InProgress))
                    .OrderByDescending(v => v.CheckedInAt ?? v.CreatedAt)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            var patient = await _patientRepository.GetByIdAsync(session.PatientId, cancellationToken);

            if (visitToComplete != null && isFinalized)
            {
                if (visitToComplete.Status == PatientVisitStatus.CheckedIn)
                {
                    visitToComplete.Start();
                }

                visitToComplete.FinishConsultation(clinicalFindings);
                await _patientVisitRepository.UpdateAsync(visitToComplete, cancellationToken);

                // Notify Cashier (ClinicStaff)
                await _notificationService.SendToRoleAsync(
                    roleName: Application.Common.Constants.Roles.ClinicStaff,
                    title: "Ready for Payment",
                    message: $"Consultation finished. Patient {patient?.FullName ?? "Unknown"} is waiting for payment.",
                    type: NotificationType.SystemAlert,
                    payload: new
                    {
                        Action = "cashier_payment_ready",
                        VisitId = visitToComplete.Id,
                        PatientId = visitToComplete.PatientId,
                        ConsultationSessionId = session.Id,
                        ScreeningId = session.AiScreeningId,
                        DiagnosisId = diagnosis.Id,
                        DoctorId = request.DoctorId,
                        DoctorName = doctorName,
                        HasPrescription = normalizedPrescriptionItems.Count > 0 && !request.NoMedicationPrescribed,
                        NoMedicationPrescribed = request.NoMedicationPrescribed,
                        FinalizedAt = finalizedAtUtc?.ToString("O", CultureInfo.InvariantCulture)
                    },
                    cancellationToken: cancellationToken
                );
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            if (patient is not null && patient.UserId.HasValue)
            {
                // Send real-time notification to Patient [FR-46]
                await _notificationService.SendAsync(
                    patient.UserId.Value,
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
