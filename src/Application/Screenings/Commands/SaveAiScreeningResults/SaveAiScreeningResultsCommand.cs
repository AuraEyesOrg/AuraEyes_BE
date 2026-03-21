using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.Screenings.Commands.SaveAiScreeningResults;

/// <summary>
/// Command to save AI screening results after analysis completes.
/// Stores the raw JSON output and analysis findings.
/// Images must be uploaded separately when creating the screening session.
/// </summary>
public record SaveAiScreeningResultsCommand : ICommand<SaveAiScreeningResultsResponse>
{
    /// <summary>
    /// The AI screening session ID
    /// </summary>
    public Guid ScreeningId { get; init; }

    /// <summary>
    /// Raw JSON output from AI model with all detection details
    /// </summary>
    public string RawJsonOutput { get; init; } = string.Empty;

    /// <summary>
    /// Overall risk level assessment
    /// </summary>
    public RiskLevel RiskLevel { get; init; }

    /// <summary>
    /// Confidence score for the risk assessment (0-100)
    /// </summary>
    public decimal ConfidenceScore { get; init; }

    /// <summary>
    /// Summary of findings
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// Detailed findings from the analysis
    /// </summary>
    public string? Findings { get; init; }
}

/// <summary>
/// Response from saving AI screening results
/// </summary>
public record SaveAiScreeningResultsResponse
{
    /// <summary>
    /// The screening ID
    /// </summary>
    public Guid ScreeningId { get; init; }

    /// <summary>
    /// The screening result ID
    /// </summary>
    public Guid ScreeningResultId { get; init; }

    /// <summary>
    /// Number of retinal images associated with this screening
    /// </summary>
    public int ImagesCount { get; init; }

    /// <summary>
    /// When the results were saved
    /// </summary>
    public DateTime SavedAt { get; init; }

    /// <summary>
    /// Risk level assessment
    /// </summary>
    public string RiskLevel { get; init; } = string.Empty;
}
