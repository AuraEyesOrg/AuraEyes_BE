using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Medical Diagnosis entity - doctor's diagnosis based on AI screening
/// </summary>
public class MedicalDiagnosis : BaseEntity, IAggregateRoot
{
    public Guid AiScreeningId { get; private set; }
    public Guid DoctorId { get; private set; }
    public Guid? ConsultationSessionId { get; private set; }
    public string? DiagnosesCode { get; private set; }
    public string? DiagnosesText { get; private set; }
    public string? TreatmentPlan { get; private set; }
    public bool ReferralRequired { get; private set; }
    public DateTime? FollowUpDate { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }

    private MedicalDiagnosis() { } // EF Core

    public MedicalDiagnosis(Guid aiScreeningId, Guid doctorId, string? diagnosesCode = null, string? diagnosesText = null)
    {
        AiScreeningId = aiScreeningId;
        DoctorId = doctorId;
        DiagnosesCode = diagnosesCode;
        DiagnosesText = diagnosesText;
        ReferralRequired = false;
    }

    public MedicalDiagnosis(Guid aiScreeningId, Guid doctorId, string? diagnosesCode = null,
        string? diagnosesText = null, string? treatmentPlan = null)
    {
        AiScreeningId = aiScreeningId;
        DoctorId = doctorId;
        DiagnosesCode = diagnosesCode;
        DiagnosesText = diagnosesText;
        TreatmentPlan = treatmentPlan;
        ReferralRequired = false;
    }

    public void UpdateDiagnosis(string? diagnosesCode, string? diagnosesText, string? treatmentPlan)
    {
        DiagnosesCode = diagnosesCode;
        DiagnosesText = diagnosesText;
        TreatmentPlan = treatmentPlan;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetReferral(bool required)
    {
        ReferralRequired = required;
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
