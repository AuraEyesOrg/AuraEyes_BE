using MediatR;
using Application.Common.Models;

namespace Application.MedicalRecords.Commands.FinalizeMedicalRecord;

public record FinalizeMedicalRecordCommand(Guid Id) : IRequest<Result>;
