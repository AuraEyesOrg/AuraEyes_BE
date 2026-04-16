namespace Application.OrganisationScreenings.Interfaces;

public sealed record AiFindingDetail
{
    public int Rank { get; init; }
    public string DiseaseName { get; init; } = string.Empty;
    public decimal ConfidencePercentage { get; init; }
    public string? Status { get; init; }
}

public sealed record AiLocalizationBox
{
    public decimal X { get; init; }
    public decimal Y { get; init; }
    public decimal Width { get; init; }
    public decimal Height { get; init; }
    public decimal? Confidence { get; init; }
}

public sealed record OrgScreeningReportPdfModel
{
    public Guid ScreeningId { get; init; }
    public Guid PatientId { get; init; }
    public string OrganisationName { get; init; } = "AuraEyes Partner Organisation";
    public string PatientName { get; init; } = "Patient";
    public DateTime CreatedAt { get; init; }
    public string ModelVersion { get; init; } = string.Empty;
    public int ImagesCount { get; init; }
    public List<string> OriginalImageUrls { get; init; } = new();
    public string? AnnotatedImageUrl { get; init; }
    public string? HeatmapImageUrl { get; init; }
    public string? RiskLevel { get; init; }
    public decimal? ConfidenceScore { get; init; }
    public string? Summary { get; init; }
    public string? Findings { get; init; }
    public DateTime? AssessedAt { get; init; }
    public List<AiFindingDetail> AiFindingDetails { get; init; } = new();
    public List<AiLocalizationBox> LocalizationBoxes { get; init; } = new();
    /// <summary>
    /// Doctor-edited heatmap matrix (rows × cols, values 0–1).
    /// When present, takes priority over HeatmapImageUrl for PDF rendering.
    /// </summary>
    public float[][]? HeatmapMatrix { get; init; }
}

public interface IOrganisationScreeningPdfService
{
    byte[] GenerateScreeningReportPdf(OrgScreeningReportPdfModel model);
}
