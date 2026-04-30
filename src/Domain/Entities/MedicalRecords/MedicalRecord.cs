using Domain.Common;
using Domain.Entities.Scheduling;

namespace Domain.Entities.MedicalRecords;

public class MedicalRecord : BaseEntity, IAggregateRoot
{
    public Guid PatientId { get; private set; }
    public Guid? ConsultationSessionId { get; private set; }
    public Guid? PatientVisitId { get; private set; }
    public string MedicalRecordNumber { get; private set; } = default!; // Mã YT
    public string? PdfUrl { get; private set; }
    public MedicalRecordStatus Status { get; private set; }

    // Navigation properties
    public virtual Domain.Entities.Users.Patient Patient { get; private set; }
    
    // Administrative Data (Section I & II) - Stored as JSON for flexibility or flat fields
    public string AdministrativeDataJson { get; private set; } = default!;
    
    // Clinical Data (Section III & Part A)
    public string ClinicalDataJson { get; private set; } = default!;
    
    // Final Diagnosis
    public string FinalDiagnosis { get; private set; } = default!;
    public string TreatmentPlan { get; private set; } = default!;

    private MedicalRecord() { } // For EF

    public MedicalRecord(Guid patientId, string medicalRecordNumber)
    {
        PatientId = patientId;
        MedicalRecordNumber = medicalRecordNumber;
        Status = MedicalRecordStatus.DraftAdmin;
        
        // Initialize non-nullable fields with empty/default values to prevent DB null constraint violations
        AdministrativeDataJson = "{}";
        ClinicalDataJson = "{}";
        FinalDiagnosis = string.Empty;
        TreatmentPlan = string.Empty;
    }

    private bool IsFinalizedState()
    {
        // Backward compatibility: older deployments may still have numeric status values > Finalized.
        return (int)Status >= (int)MedicalRecordStatus.Finalized;
    }

    private void EnsureNotFinalized()
    {
        if (IsFinalizedState())
            throw new InvalidOperationException("Cannot modify a finalized medical record.");
    }

    public void UpdateAdministrativeInfo(string jsonData)
    {
        EnsureNotFinalized();
        AdministrativeDataJson = jsonData;
        Status = MedicalRecordStatus.PendingClinical;
    }

    public void UpdateClinicalInfo(string clinicalJson, string administrativeJson, string finalDiagnosis, string treatmentPlan)
    {
        EnsureNotFinalized();
        ClinicalDataJson = clinicalJson;
        AdministrativeDataJson = administrativeJson;
        FinalDiagnosis = finalDiagnosis;
        TreatmentPlan = treatmentPlan;
        Status = MedicalRecordStatus.PendingClinical;
    }

    public void StartDoctorFilling()
    {
        EnsureNotFinalized();
        if (Status == MedicalRecordStatus.DraftAdmin)
            Status = MedicalRecordStatus.PendingClinical;
    }

    public void FinalizeRecord()
    {
        EnsureNotFinalized();

        if (string.IsNullOrWhiteSpace(ClinicalDataJson) || string.IsNullOrWhiteSpace(FinalDiagnosis))
            throw new InvalidOperationException("Clinical data must be completed before finalizing the EMR.");

        Status = MedicalRecordStatus.Finalized;
    }

    public void LinkToConsultation(Guid sessionId)
    {
        ConsultationSessionId = sessionId;
    }

    public void LinkToPatientVisit(Guid visitId)
    {
        PatientVisitId = visitId;
    }

    public void UpdatePdfUrl(string url)
    {
        PdfUrl = url;
    }
}
