using Application.Common.Models;
using MediatR;

namespace Application.MedicalRecords.Commands.StartDoctorFilling;

public record StartDoctorFillingCommand(Guid Id) : IRequest<Result>;
