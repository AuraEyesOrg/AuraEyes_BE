namespace Application.SystemAdmin.AiModels.Queries.GetAiModelMetrics;

/// <summary>
/// DTO for AI model metrics
/// </summary>
public class AiModelMetricsDto
{
    // Current active model info
    public string? ActiveModelVersion { get; set; }
    public string? ActiveModelName { get; set; }
    public DateTime? DeployedAt { get; set; }

    // Performance metrics (3.9.2-3.9.4)
    public decimal GlobalAccuracy { get; set; }
    public decimal AccuracyTarget { get; set; } = 95.0m;
    public bool AccuracyMeetsTarget { get; set; }

    public decimal FalsePositiveRate { get; set; }
    public decimal FalsePositiveRateThreshold { get; set; } = 5.0m;
    public bool FalsePositiveWithinSafetyMargin { get; set; }

    public double AverageInferenceTimeMs { get; set; }
    public double InferenceTimeTarget { get; set; } = 200.0;
    public bool InferenceTimeWithinTarget { get; set; }

    // Additional metrics
    public decimal Sensitivity { get; set; }
    public decimal Specificity { get; set; }
    public long TotalInferences { get; set; }

    // Model status
    public bool ModelOperational { get; set; }
    public string ModelStatus { get; set; } = string.Empty;
}
