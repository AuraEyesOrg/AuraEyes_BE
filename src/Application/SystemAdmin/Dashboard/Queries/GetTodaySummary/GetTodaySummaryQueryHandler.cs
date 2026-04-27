using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Dashboard.Queries.GetTodaySummary;

public class GetTodaySummaryQueryHandler
    : IQueryHandler<GetTodaySummaryQuery, TodaySummaryDto>
{
    private readonly IDashboardMetricsService _dashboardMetricsService;

    public GetTodaySummaryQueryHandler(IDashboardMetricsService dashboardMetricsService)
    {
        _dashboardMetricsService = dashboardMetricsService;
    }

    public async Task<Result<TodaySummaryDto>> Handle(
        GetTodaySummaryQuery request,
        CancellationToken cancellationToken)
    {
        var dto = await _dashboardMetricsService.GetTodaySummaryAsync(cancellationToken);
        return Result<TodaySummaryDto>.Success(dto);
    }
}
