using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.Screenings.Commands.CreateAiScreeningSession;

/// <summary>
/// Command to create a new AI screening session for a patient.
/// Should be called at the beginning of a screening workflow.
/// </summary>
public record CreateAiScreeningSessionCommand : ICommand<CreateAiScreeningSessionResponse>
{
    /// <summary>
    /// Model version to use for screening
    /// </summary>
    public string ModelVersion { get; init; } = "AURA_v1.0";

    /// <summary>
    /// Retinal images uploaded for this screening
    /// </summary>
    public required List<RetinalImageData> RetinalImages { get; init; }
}

/// <summary>
/// Retinal image data
/// </summary>
public record RetinalImageData
{
    /// <summary>
    /// Public URL in Supabase
    /// </summary>
    public required string ImageUrl { get; init; }

    /// <summary>
    /// Which eye (Left=1, Right=2, Both=3)
    /// </summary>
    public required EyeSide EyeSide { get; init; }

    /// <summary>
    /// Optional device name
    /// </summary>
    public string? DeviceName { get; init; }
}

/// <summary>
/// Response from creating an AI screening session
/// </summary>
public record CreateAiScreeningSessionResponse
{
    /// <summary>
    /// The newly created screening session ID
    /// </summary>
    public Guid ScreeningId { get; init; }

    /// <summary>
    /// Patient ID
    /// </summary>
    public Guid PatientId { get; init; }

    /// <summary>
    /// Model version
    /// </summary>
    public string ModelVersion { get; init; } = string.Empty;

    /// <summary>
    /// Images associated with this screening
    /// </summary>
    public List<RetinalImageResponse> Images { get; init; } = new();

    /// <summary>
    /// Timestamp of creation
    /// </summary>
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Retinal image response
/// </summary>
public record RetinalImageResponse
{
    public Guid Id { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public string EyeSide { get; init; } = string.Empty;
}
