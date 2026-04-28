using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Screenings.Commands.CreateAiScreeningSession;
using Domain.Enums;
using MediatR;

namespace Application.Screenings.Commands.CreateClinicScreeningSession;

public record CreateClinicScreeningSessionCommand : IRequest<Result<Guid>>
{
    public required Guid PatientId { get; init; }
    public string ModelVersion { get; init; } = "AURA_v1.0";
    public required List<RetinalImageData> RetinalImages { get; init; }
}
