using MediatR;
using Application.Common.Models;

namespace Application.MedicalRecords.Commands.CreateMedicalRecord;

public record CreateMedicalRecordCommand : IRequest<Result<Guid>>
{
    public Guid PatientId { get; init; }
    public Guid? ConsultationSessionId { get; init; }
    public string MedicalRecordNumber { get; init; } = string.Empty;
    public string AdministrativeDataJson { get; init; } = string.Empty;
}
