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
    private readonly IIdentityService _identityService;

    public GetOrganisationsQueryHandler(
        IRepository<Organisation> organisationRepository,
        IIdentityService identityService)
    {
        _organisationRepository = organisationRepository;
        _identityService = identityService;
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

        var orgPage = query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var items = new List<OrganisationListDto>(orgPage.Count);
        foreach (var org in orgPage)
        {
            var owner = await _identityService.GetUserByIdAsync(org.OwnerId, cancellationToken);

            items.Add(new OrganisationListDto
            {
                Id = org.Id,
                Name = org.Name,
                Address = org.Address,
                LicenseNumber = org.LicenseNumber,
                OrgType = org.OrgType.ToString(),
                RatingAverage = org.RatingAverage,
                RatingCount = org.RatingCount,
                OwnerAvatarUrl = owner?.AvatarUrl,
                DeviceCount = 0, // TODO: join with Device count when available
                IsActive = !org.IsDeleted,
                CreatedAt = org.CreatedAt
            });
        }

        var pagedResult = new PagedResult<OrganisationListDto>(
            items, totalCount, request.PageNumber, request.PageSize);

        return Result<PagedResult<OrganisationListDto>>.Success(pagedResult);
    }
}
