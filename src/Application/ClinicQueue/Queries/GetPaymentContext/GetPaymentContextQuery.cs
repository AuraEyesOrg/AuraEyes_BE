using System.Text.Json;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.ClinicQueue.Queries.GetPaymentContext;

public record GetPaymentContextQuery : IQuery<ClinicPaymentContextDto>
{
    public Guid VisitId { get; init; }
}

public sealed class GetPaymentContextQueryHandler
    : IQueryHandler<GetPaymentContextQuery, ClinicPaymentContextDto>
{
    private readonly IPatientVisitRepository _patientVisitRepository;
    private readonly IConsultationSessionRepository _consultationSessionRepository;
    private readonly IRepository<MedicalDiagnosis> _diagnosisRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IIdentityService _identityService;

    public GetPaymentContextQueryHandler(
        IPatientVisitRepository patientVisitRepository,
        IConsultationSessionRepository consultationSessionRepository,
        IRepository<MedicalDiagnosis> diagnosisRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IIdentityService identityService)
    {
        _patientVisitRepository = patientVisitRepository;
        _consultationSessionRepository = consultationSessionRepository;
        _diagnosisRepository = diagnosisRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _identityService = identityService;
    }

    public async Task<Result<ClinicPaymentContextDto>> Handle(
        GetPaymentContextQuery request,
        CancellationToken cancellationToken)
    {
        if (request.VisitId == Guid.Empty)
            return Result<ClinicPaymentContextDto>.Failure("VisitId is required.");

        var visit = await _patientVisitRepository.Query().AsNoTracking()
            .Include(v => v.Patient)
            .Include(v => v.MedicalRecord)
            .FirstOrDefaultAsync(v => v.Id == request.VisitId, cancellationToken);

        if (visit is null)
            return Result<ClinicPaymentContextDto>.NotFound($"Visit '{request.VisitId}' not found.");

        if (visit.MedicalRecord?.ConsultationSessionId == null)
            return Result<ClinicPaymentContextDto>.Failure("Consultation session not linked to this visit yet.");

        var consultationSessionId = visit.MedicalRecord.ConsultationSessionId.Value;

        var consultation = await _consultationSessionRepository.Query().AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == consultationSessionId && !c.IsDeleted, cancellationToken);

        if (consultation is null)
            return Result<ClinicPaymentContextDto>.Failure("Consultation session not found for this visit.");

        var diagnosis = await _diagnosisRepository.Query().AsNoTracking()
            .Where(d => d.ConsultationSessionId == consultation.Id && !d.IsDeleted)
            .OrderByDescending(d => d.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (diagnosis is null)
            return Result<ClinicPaymentContextDto>.Failure("Diagnosis not found for this visit.");

        var snapshot = ExtractDiagnosisSnapshot(diagnosis);
        var diagnosedByDoctorName = snapshot.DiagnosedBy?.DoctorName;
        if (string.IsNullOrWhiteSpace(diagnosedByDoctorName))
        {
            var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(diagnosis.DoctorId, cancellationToken);
            if (ophthalmologist != null)
            {
                var doctorUser = await _identityService.GetUserByIdAsync(ophthalmologist.UserId, cancellationToken);
                diagnosedByDoctorName = doctorUser?.FullName?.Trim();
            }
        }

        var patientName = visit.Patient?.FullName;
        if (string.IsNullOrWhiteSpace(patientName) && visit.Patient?.UserId != null)
        {
            var patientUser = await _identityService.GetUserByIdAsync(visit.Patient.UserId.Value, cancellationToken);
            patientName = patientUser?.FullName?.Trim();
        }
        if (string.IsNullOrWhiteSpace(patientName))
        {
            patientName = "Unknown Patient";
        }

        var dto = new ClinicPaymentContextDto
        {
            VisitId = visit.Id,
            PatientId = visit.PatientId,
            PatientName = patientName,
            ConsultationSessionId = consultation.Id,
            ScreeningId = diagnosis.AiScreeningId,
            Diagnosis = new DiagnosisSnapshotDto
            {
                DiagnosisId = diagnosis.Id,
                DiagnosisCode = snapshot.DiagnosisCode ?? diagnosis.DiagnosisCode,
                CodingSystem = snapshot.CodingSystem ?? diagnosis.CodingSystem,
                ClinicalFindings = snapshot.ClinicalFindings ?? diagnosis.ClinicalFindings,
                SeverityLevel = snapshot.SeverityLevel ?? diagnosis.SeverityLevel,
                Recommendations = snapshot.Recommendations ?? diagnosis.Recommendations,
                FollowUpDate = snapshot.FollowUpDate ?? diagnosis.FollowUpDate,
                DiagnosedBy = new DiagnosedByDto
                {
                    DoctorId = diagnosis.DoctorId,
                    DoctorName = diagnosedByDoctorName
                },
                FinalizedAt = snapshot.FinalizedAt ?? diagnosis.FinalizedAt,
                PrescriptionItems = snapshot.PrescriptionItems
                    .Select(item => new PrescriptionItemDto
                    {
                        MedicineName = item.MedicineName,
                        Unit = item.Unit,
                        Dosage = item.Dosage,
                        Frequency = item.Frequency,
                        Duration = item.Duration,
                        Instruction = item.Instruction
                    })
                    .ToList(),
                PrescriptionNote = snapshot.PrescriptionNote,
                NoMedicationPrescribed = snapshot.NoMedicationPrescribed
            }
        };

        return Result<ClinicPaymentContextDto>.Success(dto);
    }

    private static DiagnosisSnapshotCarrier ExtractDiagnosisSnapshot(MedicalDiagnosis diagnosis)
    {
        var carrier = new DiagnosisSnapshotCarrier
        {
            DiagnosisCode = diagnosis.DiagnosisCode,
            CodingSystem = diagnosis.CodingSystem,
            ClinicalFindings = diagnosis.ClinicalFindings,
            SeverityLevel = diagnosis.SeverityLevel,
            Recommendations = diagnosis.Recommendations,
            FollowUpDate = diagnosis.FollowUpDate,
            FinalizedAt = diagnosis.FinalizedAt,
            DiagnosedBy = new DiagnosedByDto { DoctorId = diagnosis.DoctorId },
            PrescriptionItems = Array.Empty<PrescriptionItemDto>(),
            NoMedicationPrescribed = false
        };

        var marker = "DIAGNOSIS_SNAPSHOT_JSON::";
        var source = diagnosis.LifestyleAdvice ?? string.Empty;
        var markerIndex = source.LastIndexOf(marker, StringComparison.Ordinal);
        if (markerIndex < 0)
            return carrier;

        var json = source[(markerIndex + marker.Length)..].Trim();
        if (string.IsNullOrWhiteSpace(json))
            return carrier;

        try
        {
            var parsed = JsonSerializer.Deserialize<DiagnosisSnapshotCarrier>(json);
            if (parsed is null)
                return carrier;

            return new DiagnosisSnapshotCarrier
            {
                DiagnosisCode = parsed.DiagnosisCode ?? carrier.DiagnosisCode,
                CodingSystem = parsed.CodingSystem ?? carrier.CodingSystem,
                ClinicalFindings = parsed.ClinicalFindings ?? carrier.ClinicalFindings,
                SeverityLevel = parsed.SeverityLevel ?? carrier.SeverityLevel,
                Recommendations = parsed.Recommendations ?? carrier.Recommendations,
                FollowUpDate = parsed.FollowUpDate ?? carrier.FollowUpDate,
                FinalizedAt = parsed.FinalizedAt ?? carrier.FinalizedAt,
                DiagnosedBy = new DiagnosedByDto
                {
                    DoctorId = parsed.DiagnosedBy?.DoctorId ?? carrier.DiagnosedBy?.DoctorId ?? Guid.Empty,
                    DoctorName = parsed.DiagnosedBy?.DoctorName
                },
                PrescriptionItems = parsed.PrescriptionItems ?? Array.Empty<PrescriptionItemDto>(),
                PrescriptionNote = parsed.PrescriptionNote,
                NoMedicationPrescribed = parsed.NoMedicationPrescribed
            };
        }
        catch
        {
            return carrier;
        }
    }
}

public sealed class ClinicPaymentContextDto
{
    public Guid VisitId { get; init; }
    public Guid PatientId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public Guid ConsultationSessionId { get; init; }
    public Guid ScreeningId { get; init; }
    public DiagnosisSnapshotDto Diagnosis { get; init; } = new();
}

public sealed class DiagnosisSnapshotDto
{
    public Guid DiagnosisId { get; init; }
    public string? DiagnosisCode { get; init; }
    public string? CodingSystem { get; init; }
    public string? ClinicalFindings { get; init; }
    public string? SeverityLevel { get; init; }
    public string? Recommendations { get; init; }
    public DateTime? FollowUpDate { get; init; }
    public DiagnosedByDto DiagnosedBy { get; init; } = new();
    public DateTime? FinalizedAt { get; init; }
    public IReadOnlyList<PrescriptionItemDto> PrescriptionItems { get; init; } = Array.Empty<PrescriptionItemDto>();
    public string? PrescriptionNote { get; init; }
    public bool NoMedicationPrescribed { get; init; }
}

public sealed class DiagnosedByDto
{
    public Guid DoctorId { get; init; }
    public string? DoctorName { get; init; }
}

public sealed class PrescriptionItemDto
{
    public string MedicineName { get; init; } = string.Empty;
    public string? Unit { get; init; }
    public string Dosage { get; init; } = string.Empty;
    public string Frequency { get; init; } = string.Empty;
    public string Duration { get; init; } = string.Empty;
    public string? Instruction { get; init; }
}

internal sealed class DiagnosisSnapshotCarrier
{
    public string? DiagnosisCode { get; init; }
    public string? CodingSystem { get; init; }
    public string? ClinicalFindings { get; init; }
    public string? SeverityLevel { get; init; }
    public IReadOnlyList<PrescriptionItemDto> PrescriptionItems { get; init; } = Array.Empty<PrescriptionItemDto>();
    public string? PrescriptionNote { get; init; }
    public bool NoMedicationPrescribed { get; init; }
    public string? Recommendations { get; init; }
    public DateTime? FollowUpDate { get; init; }
    public DiagnosedByDto? DiagnosedBy { get; init; }
    public DateTime? FinalizedAt { get; init; }
}

