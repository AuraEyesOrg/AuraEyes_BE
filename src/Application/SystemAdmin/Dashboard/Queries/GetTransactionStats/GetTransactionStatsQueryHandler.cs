using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Dashboard.Queries.GetTransactionStats;

public class GetTransactionStatsQueryHandler
    : IQueryHandler<GetTransactionStatsQuery, IReadOnlyList<TransactionStatsDto>>
{
    private readonly IDashboardMetricsService _dashboardMetricsService;

    public GetTransactionStatsQueryHandler(IDashboardMetricsService dashboardMetricsService)
    {
        _dashboardMetricsService = dashboardMetricsService;
    }

    public async Task<Result<IReadOnlyList<TransactionStatsDto>>> Handle(
        GetTransactionStatsQuery request,
        CancellationToken cancellationToken)
    {
        var dto = await _dashboardMetricsService.GetTransactionStatsAsync(request.Period, cancellationToken);
        return Result<IReadOnlyList<TransactionStatsDto>>.Success(dto);
    }
}
