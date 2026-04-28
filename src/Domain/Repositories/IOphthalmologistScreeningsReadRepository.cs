namespace Domain.Repositories;

/// <summary>
/// Read-only repository for ophthalmologist screening queries.
/// </summary>
public interface IOphthalmologistScreeningsReadRepository
{
    /// <summary>
    /// Get all AI screenings visible to an ophthalmologist (via linked consultation sessions).
    /// </summary>
    Task<IReadOnlyList<OphthalmologistScreeningListReadModel>> ListForOphthalmologistAsync(
        Guid ophthalmologistProfileId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get detailed screening information with access control.
    /// Assigned doctors for verification/clinic sessions can always view full screening data.
    /// Returns null if consultation session not found or access denied.
    /// </summary>
    Task<OphthalmologistScreeningDetailReadModel?> GetDetailForOphthalmologistAsync(
        Guid ophthalmologistProfileId,
        Guid screeningId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Read model for ophthalmologist screening list view.
/// </summary>
public sealed record OphthalmologistScreeningListReadModel
{
    public required Guid ScreeningId { get; init; }
    public required Guid PatientId { get; init; }
    public required string PatientName { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
    public required string ModelVersion { get; init; }
    public required int ImagesCount { get; init; }
    public string? ThumbnailUrl { get; init; }
    public string? LatestRiskLevel { get; init; }
    public decimal? ConfidenceScore { get; init; }
    public string? AiPrimaryLabel { get; init; }
    public string? SummarySnippet { get; init; }
    public required string ReviewStatus { get; init; }
}

/// <summary>
/// Read model for ophthalmologist screening detail view.
/// </summary>
public sealed record OphthalmologistScreeningDetailReadModel
{
    public required Guid ScreeningId { get; init; }
    public required Guid PatientId { get; init; }
    public required string PatientFullName { get; init; }
    public required string ModelVersion { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
    public string? RawJsonOutput { get; init; }
    public required IReadOnlyList<OphthalmologistRetinalImageReadModel> Images { get; init; }
    public OphthalmologistScreeningResultReadModel? LatestResult { get; init; }
    /// <summary>pending-review | reviewed | approved | flagged</summary>
    public required string ReviewStatus { get; init; }
    public Guid? MedicalRecordId { get; init; }
}

public sealed record OphthalmologistRetinalImageReadModel
{
    public required Guid Id { get; init; }
    public required string ImageUrl { get; init; }
    public required string EyeSide { get; init; }
    public string? DeviceName { get; init; }
    public decimal? QualityScore { get; init; }
    public required DateTime CapturedAt { get; init; }
}

public sealed record OphthalmologistScreeningResultReadModel
{
    public required Guid ScreeningResultId { get; init; }
    public required string RiskLevel { get; init; }
    public required decimal ConfidenceScore { get; init; }
    public string? Summary { get; init; }
    public string? Findings { get; init; }
    public required DateTime AssessedAt { get; init; }
}
