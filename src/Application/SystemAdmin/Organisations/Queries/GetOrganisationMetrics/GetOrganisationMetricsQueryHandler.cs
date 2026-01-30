using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enums;
using Domain.Repositories;

namespace Application.SystemAdmin.Organisations.Queries.GetOrganisationMetrics;

/// <summary>
/// Handler for GetOrganisationMetricsQuery.
/// </summary>
public class GetOrganisationMetricsQueryHandler : IQueryHandler<GetOrganisationMetricsQuery, OrganisationMetricsDto>
{
    private readonly IOrganisationRepository _organisationRepository;

    public GetOrganisationMetricsQueryHandler(IOrganisationRepository organisationRepository)
    {
        _organisationRepository = organisationRepository;
    }

    public async Task<Result<OrganisationMetricsDto>> Handle(GetOrganisationMetricsQuery request, CancellationToken cancellationToken)
    {
        // Get organisation counts by type
        var countByType = await _organisationRepository.GetCountByOrgTypeAsync(cancellationToken);
        var activeCount = await _organisationRepository.GetActiveCountAsync(cancellationToken);
        var totalCount = countByType.Values.Sum();

        var dto = new OrganisationMetricsDto
        {
            TotalOrganisations = totalCount,
            ActiveOrganisations = activeCount,
            InactiveOrganisations = totalCount - activeCount,
            MonthlyChangePercentage = 0m, // TODO: Calculate from historical data when available
            TotalDevices = 0, // TODO: Get from Device repository when available
            OnlineDevices = 0,
            CalibrationRequiredDevices = 0,
            CalibrationActionNeeded = false,
            HospitalCount = countByType.GetValueOrDefault(OrgType.Hospital),
            ClinicCount = countByType.GetValueOrDefault(OrgType.Clinic),
            PrivatePracticeCount = countByType.GetValueOrDefault(OrgType.PrivatePractice),
            OtherCount = countByType.GetValueOrDefault(OrgType.ResearchCenter) + 
                         countByType.GetValueOrDefault(OrgType.DiagnosticCenter)
        };

        return Result<OrganisationMetricsDto>.Success(dto);
    }
}
