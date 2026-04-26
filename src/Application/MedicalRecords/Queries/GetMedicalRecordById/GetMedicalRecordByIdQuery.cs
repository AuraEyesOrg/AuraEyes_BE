using MediatR;
using Application.Common.Models;
using Application.MedicalRecords.Common;

namespace Application.MedicalRecords.Queries.GetMedicalRecordById;

public record GetMedicalRecordByIdQuery(Guid Id) : IRequest<Result<MedicalRecordDto>>;
