using Domain.Common;

namespace Domain.Entities.Screening;

/// <summary>
/// Patient-facing roadmap generated from AI screening output and doctor diagnosis.
/// This entity is derived guidance and must not replace the medical diagnosis source of truth.
/// </summary>
public class PatientRoadmap : BaseEntity, IAggregateRoot
{
    public Guid PatientId { get; private set; }
    public Guid MedicalDiagnosisId { get; private set; }

    public string RiskLevel { get; private set; } = string.Empty;
    public string Summary { get; private set; } = string.Empty;

    /// <summary>Serialized JSON array.</summary>
    public string NextStepsJson { get; private set; } = string.Empty;

    /// <summary>Serialized JSON array.</summary>
    public string LifestyleAdviceJson { get; private set; } = string.Empty;

    /// <summary>Serialized JSON array.</summary>
    public string WarningSignsJson { get; private set; } = string.Empty;

    public bool FollowUpNeeded { get; private set; }
    public string FollowUpTimeframe { get; private set; } = string.Empty;

    /// <summary>
    /// Raw model JSON retained only after successful validation.
    /// </summary>
    public string? RawAiResponse { get; private set; }

    /// <summary>Allowed values: AI or DOCTOR_OVERRIDE.</summary>
    public string Source { get; private set; } = string.Empty;

    public DateTime GeneratedAt { get; private set; }

    private PatientRoadmap() { } // EF Core

    public PatientRoadmap(
        Guid patientId,
        Guid medicalDiagnosisId,
        string riskLevel,
        string summary,
        string nextStepsJson,
        string lifestyleAdviceJson,
        string warningSignsJson,
        bool followUpNeeded,
        string followUpTimeframe,
        string? rawAiResponse,
        string source,
        DateTime generatedAt)
    {
        if (patientId == Guid.Empty)
            throw new ArgumentException("Patient ID is required.", nameof(patientId));

        if (medicalDiagnosisId == Guid.Empty)
            throw new ArgumentException("Medical diagnosis ID is required.", nameof(medicalDiagnosisId));

        if (string.IsNullOrWhiteSpace(riskLevel))
            throw new ArgumentException("Risk level is required.", nameof(riskLevel));

        if (string.IsNullOrWhiteSpace(summary))
            throw new ArgumentException("Summary is required.", nameof(summary));

        if (string.IsNullOrWhiteSpace(nextStepsJson))
            throw new ArgumentException("Next steps payload is required.", nameof(nextStepsJson));

        if (string.IsNullOrWhiteSpace(lifestyleAdviceJson))
            throw new ArgumentException("Lifestyle advice payload is required.", nameof(lifestyleAdviceJson));

        if (string.IsNullOrWhiteSpace(warningSignsJson))
            throw new ArgumentException("Warning signs payload is required.", nameof(warningSignsJson));

        if (string.IsNullOrWhiteSpace(source))
            throw new ArgumentException("Source is required.", nameof(source));

        PatientId = patientId;
        MedicalDiagnosisId = medicalDiagnosisId;
        RiskLevel = riskLevel.Trim();
        Summary = summary.Trim();
        NextStepsJson = nextStepsJson;
        LifestyleAdviceJson = lifestyleAdviceJson;
        WarningSignsJson = warningSignsJson;
        FollowUpNeeded = followUpNeeded;
        FollowUpTimeframe = followUpTimeframe?.Trim() ?? string.Empty;
        RawAiResponse = rawAiResponse;
        Source = source.Trim();
        GeneratedAt = generatedAt;
    }

    public void OverrideByDoctor(
        string riskLevel,
        string summary,
        string nextStepsJson,
        string lifestyleAdviceJson,
        string warningSignsJson,
        bool followUpNeeded,
        string followUpTimeframe)
    {
        if (string.IsNullOrWhiteSpace(riskLevel))
            throw new ArgumentException("Risk level is required.", nameof(riskLevel));

        if (string.IsNullOrWhiteSpace(summary))
            throw new ArgumentException("Summary is required.", nameof(summary));

        if (string.IsNullOrWhiteSpace(nextStepsJson))
            throw new ArgumentException("Next steps payload is required.", nameof(nextStepsJson));

        if (string.IsNullOrWhiteSpace(lifestyleAdviceJson))
            throw new ArgumentException("Lifestyle advice payload is required.", nameof(lifestyleAdviceJson));

        if (string.IsNullOrWhiteSpace(warningSignsJson))
            throw new ArgumentException("Warning signs payload is required.", nameof(warningSignsJson));

        RiskLevel = riskLevel.Trim();
        Summary = summary.Trim();
        NextStepsJson = nextStepsJson;
        LifestyleAdviceJson = lifestyleAdviceJson;
        WarningSignsJson = warningSignsJson;
        FollowUpNeeded = followUpNeeded;
        FollowUpTimeframe = followUpTimeframe?.Trim() ?? string.Empty;
        Source = "DOCTOR_OVERRIDE";
        UpdatedAt = DateTime.UtcNow;
    }
}
