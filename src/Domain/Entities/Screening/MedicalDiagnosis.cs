using Domain.Common;

namespace Domain.Entities.Screening;

/// <summary>
/// Medical Diagnosis entity - the clinical outcome of a ConsultationSession.
/// Mandatory FK to ConsultationSession ensures every diagnosis has a supervising doctor.
/// Provides lifestyle advice and referral flag instead of prescriptions (online risk mitigation).
/// </summary>
public class MedicalDiagnosis : BaseEntity, IAggregateRoot
{
    public Guid AiScreeningId { get; private set; }
    public Guid DoctorId { get; private set; }

    /// <summary>Required: every diagnosis must originate from a consultation session.</summary>
    public Guid ConsultationSessionId { get; private set; }

    public string? DiagnosisCode { get; private set; }
    public string? CodingSystem { get; private set; }
    public string? ClinicalFindings { get; private set; }
    public string? SeverityLevel { get; private set; }
    public decimal? ConfidenceLevel { get; private set; }
    public string? TreatmentPlan { get; private set; }
    public string? Recommendations { get; private set; }
    public bool IsUrgent { get; private set; }
    public string? Status { get; private set; }
    public DateTime? FinalizedAt { get; private set; }

    /// <summary>Lifestyle advice — replaces drug prescriptions to protect online doctors.</summary>
    public string? LifestyleAdvice { get; private set; }

    /// <summary>True if patient should be referred to a specialist / higher facility.</summary>
    public bool IsReferralNeeded { get; private set; }

    public DateTime? FollowUpDate { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }

    private MedicalDiagnosis() { } // EF Core

    public MedicalDiagnosis(Guid aiScreeningId, Guid doctorId, Guid consultationSessionId,
        string? diagnosisCode = null, string? codingSystem = null, string? clinicalFindings = null,
        string? severityLevel = null, decimal? confidenceLevel = null, string? treatmentPlan = null,
        string? recommendations = null, string? lifestyleAdvice = null, bool isUrgent = false,
        string? status = null, DateTime? followUpDate = null, bool isReferralNeeded = false,
        DateTime? finalizedAt = null)
    {
        AiScreeningId = aiScreeningId;
        DoctorId = doctorId;
        ConsultationSessionId = consultationSessionId;

        DiagnosisCode = diagnosisCode;
        CodingSystem = codingSystem;
        ClinicalFindings = clinicalFindings;
        SeverityLevel = severityLevel;
        ConfidenceLevel = confidenceLevel;
        TreatmentPlan = treatmentPlan;
        Recommendations = recommendations;
        LifestyleAdvice = lifestyleAdvice;
        IsUrgent = isUrgent;
        Status = status;
        FollowUpDate = followUpDate;
        IsReferralNeeded = isReferralNeeded;
        FinalizedAt = finalizedAt;
        ConfirmedAt = finalizedAt;
    }

    public void UpdateClinicalAssessment(
        string? diagnosisCode,
        string? codingSystem,
        string? clinicalFindings,
        string? severityLevel,
        decimal? confidenceLevel,
        string? treatmentPlan,
        string? recommendations,
        string? lifestyleAdvice,
        bool isUrgent,
        string? status,
        DateTime? followUpDate,
        bool isReferralNeeded)
    {
        DiagnosisCode = diagnosisCode;
        CodingSystem = codingSystem;
        ClinicalFindings = clinicalFindings;
        SeverityLevel = severityLevel;
        ConfidenceLevel = confidenceLevel;
        TreatmentPlan = treatmentPlan;
        Recommendations = recommendations;
        LifestyleAdvice = lifestyleAdvice;
        IsUrgent = isUrgent;
        Status = status;
        FollowUpDate = followUpDate;
        IsReferralNeeded = isReferralNeeded;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDiagnosis(string? diagnosisCode, string? clinicalFindings,
        string? treatmentPlan, string? lifestyleAdvice)
    {
        UpdateClinicalAssessment(
            diagnosisCode,
            CodingSystem,
            clinicalFindings,
            SeverityLevel,
            ConfidenceLevel,
            treatmentPlan,
            Recommendations,
            lifestyleAdvice,
            IsUrgent,
            Status,
            FollowUpDate,
            IsReferralNeeded);
    }

    public void SetReferral(bool needed)
    {
        IsReferralNeeded = needed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetFollowUp(DateTime followUpDate)
    {
        if (followUpDate <= DateTime.UtcNow)
            throw new ArgumentException("Follow-up date must be in the future", nameof(followUpDate));

        FollowUpDate = followUpDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Confirm()
    {
        ConfirmedAt = DateTime.UtcNow;
        FinalizedAt = ConfirmedAt;
        Status = "Finalized";
        UpdatedAt = DateTime.UtcNow;
    }
}
