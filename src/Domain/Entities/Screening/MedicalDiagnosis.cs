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

    public string? DiagnosesCode { get; private set; }
    public string? DiagnosesText { get; private set; }
    public string? TreatmentPlan { get; private set; }

    /// <summary>Lifestyle advice — replaces drug prescriptions to protect online doctors.</summary>
    public string? LifestyleAdvice { get; private set; }

    /// <summary>True if patient should be referred to a specialist / higher facility.</summary>
    public bool IsReferralNeeded { get; private set; }

    public DateTime? FollowUpDate { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }

    private MedicalDiagnosis() { } // EF Core

    public MedicalDiagnosis(Guid aiScreeningId, Guid doctorId, Guid consultationSessionId,
        string? diagnosesCode = null, string? diagnosesText = null, string? treatmentPlan = null,
        string? lifestyleAdvice = null)
    {
        AiScreeningId = aiScreeningId;
        DoctorId = doctorId;
        ConsultationSessionId = consultationSessionId;
        DiagnosesCode = diagnosesCode;
        DiagnosesText = diagnosesText;
        TreatmentPlan = treatmentPlan;
        LifestyleAdvice = lifestyleAdvice;
        IsReferralNeeded = false;
    }

    public void UpdateDiagnosis(string? diagnosesCode, string? diagnosesText,
        string? treatmentPlan, string? lifestyleAdvice)
    {
        DiagnosesCode = diagnosesCode;
        DiagnosesText = diagnosesText;
        TreatmentPlan = treatmentPlan;
        LifestyleAdvice = lifestyleAdvice;
        UpdatedAt = DateTime.UtcNow;
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
        UpdatedAt = DateTime.UtcNow;
    }
}
