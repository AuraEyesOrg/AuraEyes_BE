namespace Application.PatientRoadmaps.Common;

public class PatientRoadmapGenerationInput
{
    public Guid PatientId { get; init; }
    public Guid ScreeningId { get; init; }

    public string AiScreeningRawJson { get; init; } = string.Empty;

    public string? DiagnosisCode { get; init; }
    public string? CodingSystem { get; init; }
    public string? ClinicalFindings { get; init; }
    public string? SeverityLevel { get; init; }
    public decimal? ConfidenceLevel { get; init; }
    public string? TreatmentPlan { get; init; }
    public string? Recommendations { get; init; }
    public string? LifestyleAdvice { get; init; }
    public bool IsUrgent { get; init; }
    public string? Status { get; init; }
    public DateTime? FollowUpDate { get; init; }
    public bool IsReferralNeeded { get; init; }
}
