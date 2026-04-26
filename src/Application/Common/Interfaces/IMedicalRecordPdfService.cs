namespace Application.Common.Interfaces;

public interface IMedicalRecordPdfService
{
    byte[] GenerateMedicalRecordPdf(MedicalRecordPdfModel model);
}

public class MedicalRecordPdfModel
{
    public string MedicalRecordNumber { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? FinalDiagnosis { get; set; }
    public string? TreatmentPlan { get; set; }
    public string AdministrativeDataJson { get; set; } = string.Empty;
    public string ClinicalDataJson { get; set; } = string.Empty;
}
