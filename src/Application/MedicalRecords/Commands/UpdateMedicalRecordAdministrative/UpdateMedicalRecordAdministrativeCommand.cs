using Application.Common.Models;
using MediatR;

namespace Application.MedicalRecords.Commands.UpdateMedicalRecordAdministrative;

public record UpdateMedicalRecordAdministrativeCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string AdministrativeDataJson { get; init; } = string.Empty;
}
