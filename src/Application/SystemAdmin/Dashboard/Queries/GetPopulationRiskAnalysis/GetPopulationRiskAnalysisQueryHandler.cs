using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Dashboard.Queries.GetPopulationRiskAnalysis;

/// <summary>
/// Handler for GetPopulationRiskAnalysisQuery - Returns real data
/// </summary>
public class GetPopulationRiskAnalysisQueryHandler : IQueryHandler<GetPopulationRiskAnalysisQuery, PopulationRiskAnalysisDto>
{
    private readonly IDashboardMetricsService _dashboardMetricsService;

    public GetPopulationRiskAnalysisQueryHandler(IDashboardMetricsService dashboardMetricsService)
    {
        _dashboardMetricsService = dashboardMetricsService;
    }

    public async Task<Result<PopulationRiskAnalysisDto>> Handle(GetPopulationRiskAnalysisQuery request, CancellationToken cancellationToken)
    {
        var dto = await _dashboardMetricsService.GetPopulationRiskAnalysisAsync(cancellationToken);
        return Result<PopulationRiskAnalysisDto>.Success(dto);
    }
}
