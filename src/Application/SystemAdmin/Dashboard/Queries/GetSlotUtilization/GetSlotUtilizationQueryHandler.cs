using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Dashboard.Queries.GetSlotUtilization;

public class GetSlotUtilizationQueryHandler
    : IQueryHandler<GetSlotUtilizationQuery, SlotUtilizationDto>
{
    private readonly IDashboardMetricsService _dashboardMetricsService;

    public GetSlotUtilizationQueryHandler(IDashboardMetricsService dashboardMetricsService)
    {
        _dashboardMetricsService = dashboardMetricsService;
    }

    public async Task<Result<SlotUtilizationDto>> Handle(
        GetSlotUtilizationQuery request,
        CancellationToken cancellationToken)
    {
        var dto = await _dashboardMetricsService.GetSlotUtilizationAsync(cancellationToken);
        return Result<SlotUtilizationDto>.Success(dto);
    }
}
