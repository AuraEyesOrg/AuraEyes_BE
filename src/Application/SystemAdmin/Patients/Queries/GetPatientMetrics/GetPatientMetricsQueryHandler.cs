using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Interfaces;

namespace Application.SystemAdmin.Patients.Queries.GetPatientMetrics;

public class GetPatientMetricsQueryHandler
    : IQueryHandler<GetPatientMetricsQuery, PatientMetricsDto>
{
    private readonly IAdminQueryService _queryService;

    public GetPatientMetricsQueryHandler(IAdminQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<Result<PatientMetricsDto>> Handle(
        GetPatientMetricsQuery request,
        CancellationToken cancellationToken)
    {
        var metrics = await _queryService.GetPatientMetricsAsync(cancellationToken);
        return Result<PatientMetricsDto>.Success(metrics);
    }
}
