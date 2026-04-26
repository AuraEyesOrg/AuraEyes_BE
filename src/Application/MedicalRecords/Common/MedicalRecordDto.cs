using Domain.Entities.MedicalRecords;
using AutoMapper;

namespace Application.MedicalRecords.Common;

public class MedicalRecordDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid? ConsultationSessionId { get; set; }
    public Guid? PatientVisitId { get; set; }
    public string MedicalRecordNumber { get; set; } = string.Empty;
    public string? PdfUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public string AdministrativeDataJson { get; set; } = string.Empty;
    public string ClinicalDataJson { get; set; } = string.Empty;
    public string FinalDiagnosis { get; set; } = string.Empty;
    public string TreatmentPlan { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public PatientDto? Patient { get; set; }
}

public class PatientDto
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

