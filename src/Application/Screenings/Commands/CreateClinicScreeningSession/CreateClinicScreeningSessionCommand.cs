using Application.Common.Models;
using Application.Screenings.Commands.CreateAiScreeningSession;
using MediatR;

namespace Application.Screenings.Commands.CreateClinicScreeningSession;

public record CreateClinicScreeningSessionCommand : IRequest<Result<Guid>>
{
    public required Guid PatientId { get; init; }

    /// <summary>Clinic queue row / check-in this screening belongs to.</summary>
    public required Guid PatientVisitId { get; init; }

    public string ModelVersion { get; init; } = "AURA_v1.0";
    public required List<RetinalImageData> RetinalImages { get; init; }
}
