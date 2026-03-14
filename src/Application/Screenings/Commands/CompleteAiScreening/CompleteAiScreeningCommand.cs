using Application.Common.Interfaces;

namespace Application.Screenings.Commands.CompleteAiScreening;

/// <summary>
/// Command to complete AI screening process and notify the patient
/// Used when AI processing is finished and results are ready
/// </summary>
public record CompleteAiScreeningCommand : ICommand<CompleteAiScreeningResponse>
{
    /// <summary>
    /// The AI screening session ID
    /// </summary>
    public Guid ScreeningId { get; init; }

    /// <summary>
    /// Raw JSON output from AI model
    /// </summary>
    public string RawJsonOutput { get; init; } = string.Empty;

    /// <summary>
    /// Overall result status (e.g., "Normal", "Abnormal", "RequiresReview")
    /// </summary>
    public string ResultStatus { get; init; } = string.Empty;
}

/// <summary>
/// Response for CompleteAiScreening command
/// </summary>
public record CompleteAiScreeningResponse
{
    public Guid ScreeningId { get; init; }
    public string ResultStatus { get; init; } = string.Empty;
    public DateTime ProcessedAt { get; init; }
}
