using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.AiModels.Queries.GetAiModelMetrics;

/// <summary>
/// Handler for GetAiModelMetricsQuery - Returns mock data
/// </summary>
public class GetAiModelMetricsQueryHandler : IQueryHandler<GetAiModelMetricsQuery, AiModelMetricsDto>
{
    public Task<Result<AiModelMetricsDto>> Handle(GetAiModelMetricsQuery request, CancellationToken cancellationToken)
    {
        // Mock AI model metrics
        var dto = new AiModelMetricsDto
        {
            ActiveModelVersion = "2.3.1",
            ActiveModelName = "AuraEyes DR Detection Model",
            DeployedAt = DateTime.UtcNow.AddMonths(-2),
            GlobalAccuracy = 96.5m,
            AccuracyTarget = 95.0m,
            AccuracyMeetsTarget = true,
            FalsePositiveRate = 2.3m,
            FalsePositiveRateThreshold = 5.0m,
            FalsePositiveWithinSafetyMargin = true,
            AverageInferenceTimeMs = 145.5,
            InferenceTimeTarget = 200.0,
            InferenceTimeWithinTarget = true,
            Sensitivity = 97.2m,
            Specificity = 95.8m,
            TotalInferences = 125000,
            ModelOperational = true,
            ModelStatus = "Active"
        };

        return Task.FromResult(Result<AiModelMetricsDto>.Success(dto));
    }
}
