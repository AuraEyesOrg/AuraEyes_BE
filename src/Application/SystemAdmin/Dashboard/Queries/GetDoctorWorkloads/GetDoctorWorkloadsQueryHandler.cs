using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Dashboard.Queries.GetDoctorWorkloads;

public class GetDoctorWorkloadsQueryHandler : IQueryHandler<GetDoctorWorkloadsQuery, PagedResult<DoctorWorkloadListItemDto>>
{
    private readonly IDashboardMetricsService _dashboardMetricsService;

    public GetDoctorWorkloadsQueryHandler(IDashboardMetricsService dashboardMetricsService)
    {
        _dashboardMetricsService = dashboardMetricsService;
    }

    public async Task<Result<PagedResult<DoctorWorkloadListItemDto>>> Handle(
        GetDoctorWorkloadsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.PageNumber < 1)
            return Result<PagedResult<DoctorWorkloadListItemDto>>.Failure("PageNumber must be greater than or equal to 1.");

        if (request.PageSize < 1 || request.PageSize > 100)
            return Result<PagedResult<DoctorWorkloadListItemDto>>.Failure("PageSize must be between 1 and 100.");

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var normalizedStatus = request.Status.Trim().ToUpperInvariant();
            if (normalizedStatus != "OK" && normalizedStatus != "UNDER")
                return Result<PagedResult<DoctorWorkloadListItemDto>>.Failure("Status must be either OK or UNDER.");
        }

        var dto = await _dashboardMetricsService.GetDoctorWorkloadsAsync(
            request.PeriodType,
            request.Date,
            request.SearchTerm,
            request.EmploymentType,
            request.Status,
            request.WarningOnly,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return Result<PagedResult<DoctorWorkloadListItemDto>>.Success(dto);
    }
}
