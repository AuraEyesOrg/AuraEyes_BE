using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Users;

namespace Application.SystemAdmin.Organisations.Queries.GetOrganisations;

/// <summary>
/// Handler for GetOrganisationsQuery - queries real Organisation data via Repository.
/// </summary>
public class GetOrganisationsQueryHandler : IQueryHandler<GetOrganisationsQuery, PagedResult<OrganisationListDto>>
{
    private readonly IRepository<Organisation> _organisationRepository;

    public GetOrganisationsQueryHandler(IRepository<Organisation> organisationRepository)
    {
        _organisationRepository = organisationRepository;
    }

    public async Task<Result<PagedResult<OrganisationListDto>>> Handle(
        GetOrganisationsQuery request,
        CancellationToken cancellationToken)
    {
        var allOrgs = await _organisationRepository.GetAllAsync(cancellationToken);
        var query = allOrgs.AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(x =>
                x.Name.ToLower().Contains(term) ||
                (x.Address != null && x.Address.ToLower().Contains(term)));
        }

        // Apply org type filter
        if (!string.IsNullOrWhiteSpace(request.OrgType))
        {
            if (Enum.TryParse<Domain.Enums.OrgType>(request.OrgType, true, out var orgType))
            {
                query = query.Where(x => x.OrgType == orgType);
            }
        }

        var totalCount = query.Count();

        var items = query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new OrganisationListDto
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                LicenseNumber = x.LicenseNumber,
                OrgType = x.OrgType.ToString(),
                DeviceCount = 0, // TODO: join with Device count when available
                IsActive = !x.IsDeleted,
                CreatedAt = x.CreatedAt
            })
            .ToList();

        var pagedResult = new PagedResult<OrganisationListDto>(
            items, totalCount, request.PageNumber, request.PageSize);

        return Result<PagedResult<OrganisationListDto>>.Success(pagedResult);
    }
}
