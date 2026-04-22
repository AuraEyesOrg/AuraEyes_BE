using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Dashboard.Queries.GetDoctorWorkload;

public class GetDoctorWorkloadQueryHandler : IQueryHandler<GetDoctorWorkloadQuery, DoctorWorkloadDto>
{
    private readonly IDashboardMetricsService _dashboardMetricsService;

    public GetDoctorWorkloadQueryHandler(IDashboardMetricsService dashboardMetricsService)
    {
        _dashboardMetricsService = dashboardMetricsService;
    }

    public async Task<Result<DoctorWorkloadDto>> Handle(GetDoctorWorkloadQuery request, CancellationToken cancellationToken)
    {
        if (request.DoctorId == Guid.Empty)
            return Result<DoctorWorkloadDto>.Failure("DoctorId is required.");

        var dto = await _dashboardMetricsService.GetDoctorWorkloadAsync(
            request.DoctorId,
            request.PeriodType,
            request.Date,
            cancellationToken);

        if (dto is null)
            return Result<DoctorWorkloadDto>.NotFound($"Doctor '{request.DoctorId}' not found.");

        return Result<DoctorWorkloadDto>.Success(dto);
    }
}
