namespace Application.Screenings.Interfaces;

public sealed record PatientAiFindingDetail
{
    public int Rank { get; init; }
    public string DiseaseName { get; init; } = string.Empty;
    public decimal ConfidencePercentage { get; init; }
    public string? Status { get; init; }
}

public sealed record PatientAiLocalizationBox
{
    public decimal X { get; init; }
    public decimal Y { get; init; }
    public decimal Width { get; init; }
    public decimal Height { get; init; }
    public decimal? Confidence { get; init; }
}

public sealed record PatientScreeningReportPdfModel
{
    public Guid ScreeningId { get; init; }
    public Guid PatientId { get; init; }
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
    public string? ReportedByDoctorName { get; init; }
    public string? DiagnosisCode { get; init; }
    public string? CodingSystem { get; init; }
    public string? ClinicalFindings { get; init; }
    public string? SeverityLevel { get; init; }
    public decimal? ConfidenceLevel { get; init; }
    public string? TreatmentPlan { get; init; }
    public string? Recommendations { get; init; }
    public string? LifestyleAdvice { get; init; }
    public string? ClinicalStatus { get; init; }
    public bool IsUrgent { get; init; }
    public bool IsReferralNeeded { get; init; }
    public DateTime? FollowUpDate { get; init; }
    public DateTime? FinalizedAt { get; init; }
    public List<PatientAiFindingDetail> AiFindingDetails { get; init; } = new();
    public List<PatientAiLocalizationBox> LocalizationBoxes { get; init; } = new();
}

public interface IPatientScreeningPdfService
{
    byte[] GenerateScreeningReportPdf(PatientScreeningReportPdfModel model);
}
