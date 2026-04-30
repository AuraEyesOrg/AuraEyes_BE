using MediatR;
using Application.Common.Models;

namespace Application.MedicalRecords.Commands.UpdateMedicalRecordClinical;

public record UpdateMedicalRecordClinicalCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string ClinicalDataJson { get; init; } = string.Empty;
    public string AdministrativeDataJson { get; init; } = string.Empty;
    public string FinalDiagnosis { get; init; } = string.Empty;
    public string TreatmentPlan { get; init; } = string.Empty;
}
