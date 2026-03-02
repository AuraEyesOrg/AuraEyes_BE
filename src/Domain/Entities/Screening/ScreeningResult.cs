using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Screening;

/// <summary>
/// Screening Result entity - AI screening analysis results
/// </summary>
public class ScreeningResult : BaseEntity
{
    public Guid AiScreeningId { get; private set; }
    public RiskLevel RiskLevel { get; private set; }
    public string? Summary { get; private set; }
    public decimal ConfidenceScore { get; private set; }
    public string? Findings { get; private set; }

    private ScreeningResult() { } // EF Core

    public ScreeningResult(Guid aiScreeningId, RiskLevel riskLevel, decimal confidenceScore, string? summary = null, string? findings = null)
    {
        if (confidenceScore < 0 || confidenceScore > 100)
            throw new ArgumentException("Confidence score must be between 0 and 100", nameof(confidenceScore));

        AiScreeningId = aiScreeningId;
        RiskLevel = riskLevel;
        ConfidenceScore = confidenceScore;
        Summary = summary;
        Findings = findings;
    }

    public void UpdateFindings(string? summary, string? findings)
    {
        Summary = summary;
        Findings = findings;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRiskLevel(RiskLevel riskLevel, decimal confidenceScore)
    {
        if (confidenceScore < 0 || confidenceScore > 100)
            throw new ArgumentException("Confidence score must be between 0 and 100", nameof(confidenceScore));

        RiskLevel = riskLevel;
        ConfidenceScore = confidenceScore;
        UpdatedAt = DateTime.UtcNow;
    }
}
