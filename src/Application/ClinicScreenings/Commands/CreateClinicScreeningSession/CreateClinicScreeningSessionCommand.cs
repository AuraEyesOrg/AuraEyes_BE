using Application.Common.Interfaces;
using Application.Screenings.Commands.CreateAiScreeningSession;

namespace Application.ClinicScreenings.Commands.CreateClinicScreeningSession;

public record CreateClinicScreeningSessionCommand : ICommand<CreateClinicScreeningSessionResponse>
{
    public Guid PatientId { get; init; }
    public string ModelVersion { get; init; } = "AURA_v1.0";
    public List<RetinalImageData> RetinalImages { get; init; } = new();
}

public record CreateClinicScreeningSessionResponse
{
    public Guid ScreeningId { get; init; }
    public Guid PatientId { get; init; }
    public string ModelVersion { get; init; } = string.Empty;
    public List<RetinalImageResponse> Images { get; init; } = new();
    public DateTime CreatedAt { get; init; }
}

public record RetinalImageResponse
{
    public Guid Id { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public string EyeSide { get; init; } = string.Empty;
}
