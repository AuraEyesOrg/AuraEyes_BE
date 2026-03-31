using Application.Common.Interfaces;

namespace Application.Screenings.Queries.GetRecentScreeningSessions;

public record GetRecentScreeningSessionsQuery : IQuery<IReadOnlyList<ScreeningSessionSummaryDto>>
{
    public int Limit { get; init; } = 10;
}

public record ScreeningSessionSummaryDto
{
    public Guid ScreeningId { get; init; }
    public string ModelVersion { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
    public bool IsActive { get; init; }
    public int ImagesCount { get; init; }
    public string? ThumbnailUrl { get; init; }
    public string? LatestRiskLevel { get; init; }
}
