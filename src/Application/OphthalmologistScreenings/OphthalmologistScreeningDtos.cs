namespace Application.OphthalmologistScreenings;

/// <summary>List row for /ophthalmologist/screenings UI.</summary>
public sealed record OphthalmologistScreeningListItemDto
{
    public Guid ScreeningId { get; init; }
    public Guid PatientId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
    public string ModelVersion { get; init; } = string.Empty;
    public int ImagesCount { get; init; }
    public string? ThumbnailUrl { get; init; }
    public string? LatestRiskLevel { get; init; }
    public decimal? ConfidenceScore { get; init; }
    public string? AiPrimaryLabel { get; init; }
    public string? SummarySnippet { get; init; }
    /// <summary>pending-review | reviewed | approved | flagged</summary>
    public string ReviewStatus { get; init; } = "pending-review";
}

/// <summary>Detail for review page.</summary>
public sealed record OphthalmologistScreeningDetailDto
{
    public Guid ScreeningId { get; init; }
    public Guid PatientId { get; init; }
    public string PatientFullName { get; init; } = string.Empty;
    public string ModelVersion { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
    public string? RawJsonOutput { get; init; }
    public IReadOnlyList<OphthalmologistRetinalImageDto> Images { get; init; } = [];
    public OphthalmologistScreeningResultDto? LatestResult { get; init; }
}

public sealed record OphthalmologistRetinalImageDto
{
    public Guid Id { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public string EyeSide { get; init; } = string.Empty;
    public string? DeviceName { get; init; }
    public decimal? QualityScore { get; init; }
    public DateTime CapturedAt { get; init; }
}

public sealed record OphthalmologistScreeningResultDto
{
    public Guid ScreeningResultId { get; init; }
    public string RiskLevel { get; init; } = string.Empty;
    public decimal ConfidenceScore { get; init; }
    public string? Summary { get; init; }
    public string? Findings { get; init; }
    public DateTime AssessedAt { get; init; }
}
