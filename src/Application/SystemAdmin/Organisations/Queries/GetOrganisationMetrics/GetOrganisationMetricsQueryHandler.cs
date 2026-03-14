using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Enums;

namespace Application.SystemAdmin.Organisations.Queries.GetOrganisationMetrics;

/// <summary>
/// Handler for GetOrganisationMetricsQuery - queries real Organisation data.
/// </summary>
public class GetOrganisationMetricsQueryHandler : IQueryHandler<GetOrganisationMetricsQuery, OrganisationMetricsDto>
{
    private readonly IRepository<Organisation> _organisationRepository;

    public GetOrganisationMetricsQueryHandler(IRepository<Organisation> organisationRepository)
    {
        _organisationRepository = organisationRepository;
    }

    public async Task<Result<OrganisationMetricsDto>> Handle(
        GetOrganisationMetricsQuery request,
        CancellationToken cancellationToken)
    {
        var allOrgs = await _organisationRepository.GetAllAsync(cancellationToken);
        var orgList = allOrgs.ToList();

        var active = orgList.Count(o => !o.IsDeleted);
        var inactive = orgList.Count(o => o.IsDeleted);

        var dto = new OrganisationMetricsDto
        {
            TotalOrganisations = orgList.Count,
            ActiveOrganisations = active,
            InactiveOrganisations = inactive,
            MonthlyChangePercentage = 0,
            TotalDevices = 0,
            OnlineDevices = 0,
            CalibrationRequiredDevices = 0,
            CalibrationActionNeeded = false,
            HospitalCount = orgList.Count(o => o.OrgType == OrgType.Hospital),
            ClinicCount = orgList.Count(o => o.OrgType == OrgType.Clinic),
            PrivatePracticeCount = 0,
            OtherCount = 0
        };

        return Result<OrganisationMetricsDto>.Success(dto);
    }
}
