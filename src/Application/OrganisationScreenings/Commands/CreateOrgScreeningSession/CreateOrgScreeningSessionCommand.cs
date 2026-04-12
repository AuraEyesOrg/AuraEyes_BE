using Application.Common.Interfaces;
using Application.Screenings.Commands.CreateAiScreeningSession;

namespace Application.OrganisationScreenings.Commands.CreateOrgScreeningSession;

/// <summary>
/// Command to create an AI screening session on behalf of a patient (organisation flow).
/// Called by OrgAdmin/staff, not by the patient themselves.
/// </summary>
public record CreateOrgScreeningSessionCommand : ICommand<CreateOrgScreeningSessionResponse>
{
    /// <summary>
    /// The patient to screen on behalf of.
    /// </summary>
    public required Guid PatientId { get; init; }

    /// <summary>
    /// AI model version to use for screening.
    /// </summary>
    public string ModelVersion { get; init; } = "AURA_v1.0";

    /// <summary>
    /// Retinal images uploaded for this screening.
    /// </summary>
    public required List<RetinalImageData> RetinalImages { get; init; }
}

public record CreateOrgScreeningSessionResponse
{
    public Guid ScreeningId { get; init; }
    public Guid PatientId { get; init; }
    public string ModelVersion { get; init; } = string.Empty;
    public List<RetinalImageResponse> Images { get; init; } = new();
    public DateTime CreatedAt { get; init; }
}
