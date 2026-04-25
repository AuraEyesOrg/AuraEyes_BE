using Domain.Common;
using Domain.Entities.Scheduling;

namespace Domain.Entities.MedicalRecords;

public class MedicalRecord : BaseEntity, IAggregateRoot
{
    public Guid PatientId { get; private set; }
    public Guid? ConsultationSessionId { get; private set; }
    public string MedicalRecordNumber { get; private set; } // Mã YT
    public MedicalRecordStatus Status { get; private set; }
    
    // Administrative Data (Section I & II) - Stored as JSON for flexibility or flat fields
    public string AdministrativeDataJson { get; private set; }
    
    // Clinical Data (Section III & Part A)
    public string ClinicalDataJson { get; private set; }
    
    // Final Diagnosis
    public string FinalDiagnosis { get; private set; }
    public string TreatmentPlan { get; private set; }

    private MedicalRecord() { } // For EF

    public MedicalRecord(Guid patientId, string medicalRecordNumber)
    {
        PatientId = patientId;
        MedicalRecordNumber = medicalRecordNumber;
        Status = MedicalRecordStatus.Filling;
    }

    public void UpdateAdministrativeInfo(string jsonData)
    {
        AdministrativeDataJson = jsonData;
    }

    public void UpdateClinicalInfo(string jsonData, string finalDiagnosis, string treatmentPlan)
    {
        ClinicalDataJson = jsonData;
        FinalDiagnosis = finalDiagnosis;
        TreatmentPlan = treatmentPlan;
    }

    public void FinalizeRecord()
    {
        Status = MedicalRecordStatus.Finalized;
    }

    public void LinkToConsultation(Guid sessionId)
    {
        ConsultationSessionId = sessionId;
    }
}
