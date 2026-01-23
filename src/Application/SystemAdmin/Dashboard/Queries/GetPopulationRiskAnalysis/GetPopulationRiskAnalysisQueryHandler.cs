using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Dashboard.Queries.GetPopulationRiskAnalysis;

/// <summary>
/// Handler for GetPopulationRiskAnalysisQuery - Returns mock data
/// </summary>
public class GetPopulationRiskAnalysisQueryHandler : IQueryHandler<GetPopulationRiskAnalysisQuery, PopulationRiskAnalysisDto>
{
    public Task<Result<PopulationRiskAnalysisDto>> Handle(GetPopulationRiskAnalysisQuery request, CancellationToken cancellationToken)
    {
        // Mock risk distribution data
        var riskCategories = new List<RiskCategoryDto>
        {
            new() { RiskLevel = "Low", Count = 8500, Percentage = 68.0m },
            new() { RiskLevel = "Moderate", Count = 2500, Percentage = 20.0m },
            new() { RiskLevel = "High", Count = 1000, Percentage = 8.0m },
            new() { RiskLevel = "Critical", Count = 500, Percentage = 4.0m }
        };

        var dto = new PopulationRiskAnalysisDto
        {
            TotalPatients = 12500,
            RiskCategories = riskCategories
        };

        return Task.FromResult(Result<PopulationRiskAnalysisDto>.Success(dto));
    }
}
