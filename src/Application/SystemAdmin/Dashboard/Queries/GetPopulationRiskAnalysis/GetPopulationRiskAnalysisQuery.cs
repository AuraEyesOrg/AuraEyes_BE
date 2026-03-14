using Application.Common.Interfaces;

namespace Application.SystemAdmin.Dashboard.Queries.GetPopulationRiskAnalysis;

/// <summary>
/// Query to get population risk distribution
/// Screen: 3.3.6 View Population Risk Analysis
/// </summary>
public record GetPopulationRiskAnalysisQuery : IQuery<PopulationRiskAnalysisDto>;
