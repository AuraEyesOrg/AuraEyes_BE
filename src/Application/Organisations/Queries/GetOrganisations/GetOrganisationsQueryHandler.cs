using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Organisations.Common;
using Domain.Repositories;

namespace Application.Organisations.Queries.GetOrganisations;

/// <summary>
/// Handler for GetOrganisationsQuery.
/// </summary>
public class GetOrganisationsQueryHandler : IQueryHandler<GetOrganisationsQuery, PagedResult<OrganisationListDto>>
{
    private readonly IOrganisationRepository _organisationRepository;

    public GetOrganisationsQueryHandler(IOrganisationRepository organisationRepository)
    {
        _organisationRepository = organisationRepository;
    }

    public async Task<Result<PagedResult<OrganisationListDto>>> Handle(GetOrganisationsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _organisationRepository.GetPagedAsync(
            request.SearchTerm,
            request.OrgType,
            request.IsActive,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(org => new OrganisationListDto
        {
            Id = org.Id,
            Name = org.Name,
            Address = org.Address,
            LicenseNumber = org.LicenseNumber,
            OrgType = org.OrgType.ToString(),
            IsActive = !org.IsDeleted,
            CreatedAt = org.CreatedAt
        }).ToList();

        var pagedResult = new PagedResult<OrganisationListDto>(dtos, totalCount, request.PageNumber, request.PageSize);
        
        return Result<PagedResult<OrganisationListDto>>.Success(pagedResult);
    }
}
