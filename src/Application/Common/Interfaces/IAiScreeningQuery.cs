namespace Application.Common.Interfaces;

/// <summary>
/// Read-side queries for AI screening persistence. Keeps EF-specific execution out of command handlers.
/// </summary>
public interface IAiScreeningQuery
{
    /// <summary>
    /// Counts retinal images linked to a screening (respects global soft-delete filters on the DbContext).
    /// </summary>
    Task<int> CountRetinalImagesForScreeningAsync(
        Guid screeningId,
        CancellationToken cancellationToken = default);
}
