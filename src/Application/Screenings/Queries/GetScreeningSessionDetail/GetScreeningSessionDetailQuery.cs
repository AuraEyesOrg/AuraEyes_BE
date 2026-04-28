using Application.Common.Interfaces;

namespace Application.Screenings.Queries.GetScreeningSessionDetail;

public record GetScreeningSessionDetailQuery(Guid ScreeningId) : IQuery<ScreeningSessionDetailDto>
{
    public bool BypassPatientCheck { get; init; } = false;
}

public record ScreeningSessionDetailDto
{
    public Guid ScreeningId { get; init; }
    public Guid PatientId { get; init; }
    public string? PatientName { get; init; }
    public string? PatientEmail { get; init; }
    public bool IsWalkIn { get; init; }
    public string ModelVersion { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
    public string? RawJsonOutput { get; init; }
    public bool IsActive { get; init; }
    public List<RetinalImageItemDto> Images { get; init; } = new();
    public ScreeningResultItemDto? LatestResult { get; init; }
}

public record RetinalImageItemDto
{
    public Guid Id { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public string EyeSide { get; init; } = string.Empty;
    public string? DeviceName { get; init; }
    public decimal? QualityScore { get; init; }
    public DateTime CapturedAt { get; init; }
}

public record ScreeningResultItemDto
{
    public Guid ScreeningResultId { get; init; }
    public string RiskLevel { get; init; } = string.Empty;
    public decimal ConfidenceScore { get; init; }
    public string? Summary { get; init; }
    public string? Findings { get; init; }
    public DateTime AssessedAt { get; init; }
}
