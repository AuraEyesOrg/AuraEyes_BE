using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Organisations.Queries.GetOrganisationMetrics;

/// <summary>
/// Handler for GetOrganisationMetricsQuery - Returns mock data
/// </summary>
public class GetOrganisationMetricsQueryHandler : IQueryHandler<GetOrganisationMetricsQuery, OrganisationMetricsDto>
{
    public Task<Result<OrganisationMetricsDto>> Handle(GetOrganisationMetricsQuery request, CancellationToken cancellationToken)
    {
        // Mock organisation metrics
        var dto = new OrganisationMetricsDto
        {
            TotalOrganisations = 45,
            ActiveOrganisations = 42,
            InactiveOrganisations = 3,
            MonthlyChangePercentage = 5.2m,
            TotalDevices = 120,
            OnlineDevices = 105,
            CalibrationRequiredDevices = 8,
            CalibrationActionNeeded = true,
            HospitalCount = 12,
            ClinicCount = 20,
            PrivatePracticeCount = 8,
            OtherCount = 5
        };

        return Task.FromResult(Result<OrganisationMetricsDto>.Success(dto));
    }
}
