using Domain.Common;
using Domain.Entities.Scheduling;

namespace Domain.Entities.MedicalRecords;

public class MedicalRecord : BaseEntity, IAggregateRoot
{
    public Guid PatientId { get; private set; }
    public Guid? ConsultationSessionId { get; private set; }
    public Guid? PatientVisitId { get; private set; }
    public string MedicalRecordNumber { get; private set; } // Mã YT
    public string? PdfUrl { get; private set; }
    public MedicalRecordStatus Status { get; private set; }

    // Navigation properties
    public virtual Domain.Entities.Users.Patient Patient { get; private set; }
    
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
        Status = MedicalRecordStatus.Draft;
    }

    private void EnsureNotLocked()
    {
        if (Status == MedicalRecordStatus.Locked)
        {
            throw new InvalidOperationException("Cannot modify a locked medical record.");
        }
    }

    public void UpdateAdministrativeInfo(string jsonData)
    {
        EnsureNotLocked();
        AdministrativeDataJson = jsonData;
        if (Status == MedicalRecordStatus.Draft)
        {
            Status = MedicalRecordStatus.ClinicFilling;
        }
    }

    public void UpdateClinicalInfo(string jsonData, string finalDiagnosis, string treatmentPlan)
    {
        EnsureNotLocked();
        ClinicalDataJson = jsonData;
        FinalDiagnosis = finalDiagnosis;
        TreatmentPlan = treatmentPlan;
        Status = MedicalRecordStatus.Completed;
    }

    public void StartDoctorFilling()
    {
        EnsureNotLocked();
        if (Status == MedicalRecordStatus.Draft || Status == MedicalRecordStatus.ClinicFilling)
        {
            Status = MedicalRecordStatus.DoctorFilling;
        }
    }

    public void FinalizeRecord()
    {
        EnsureNotLocked();
        Status = MedicalRecordStatus.Locked;
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
