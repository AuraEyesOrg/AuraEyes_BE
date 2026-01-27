using Application.Common.Interfaces;

namespace Application.SystemAdmin.AiModels.Queries.GetAiModelMetrics;

/// <summary>
/// Query to get AI model performance metrics
/// Screen: 3.9.1-3.9.4 View AI Model Monitoring Overview
/// </summary>
public record GetAiModelMetricsQuery : IQuery<AiModelMetricsDto>;
