using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Dashboard.Queries.GetDoctorStatus;

public class GetDoctorStatusQueryHandler
    : IQueryHandler<GetDoctorStatusQuery, IReadOnlyList<DoctorStatusDto>>
{
    private readonly IDashboardMetricsService _dashboardMetricsService;

    public GetDoctorStatusQueryHandler(IDashboardMetricsService dashboardMetricsService)
    {
        _dashboardMetricsService = dashboardMetricsService;
    }

    public async Task<Result<IReadOnlyList<DoctorStatusDto>>> Handle(
        GetDoctorStatusQuery request,
        CancellationToken cancellationToken)
    {
        var dto = await _dashboardMetricsService.GetDoctorStatusAsync(cancellationToken);
        return Result<IReadOnlyList<DoctorStatusDto>>.Success(dto);
    }
}
