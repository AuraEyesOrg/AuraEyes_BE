using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.SystemAdmin.Organisations.Queries.GetOrganisations;

/// <summary>
/// Handler for GetOrganisationsQuery - queries real Organisation data via Repository.
/// </summary>
public class GetOrganisationsQueryHandler : IQueryHandler<GetOrganisationsQuery, PagedResult<OrganisationListDto>>
{
    private readonly IRepository<Organisation> _organisationRepository;
    private readonly IRepository<OrganisationPatientLink> _organisationPatientLinkRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IIdentityService _identityService;

    public GetOrganisationsQueryHandler(
        IRepository<Organisation> organisationRepository,
        IRepository<OrganisationPatientLink> organisationPatientLinkRepository,
        IRepository<Patient> patientRepository,
        IIdentityService identityService)
    {
        _organisationRepository = organisationRepository;
        _organisationPatientLinkRepository = organisationPatientLinkRepository;
        _patientRepository = patientRepository;
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

        var organisationIds = orgPage.Select(x => x.Id).ToList();

        var patientCountLookup = await (
            from link in _organisationPatientLinkRepository.Query()
            join patient in _patientRepository.Query() on link.PatientId equals patient.Id
            where organisationIds.Contains(link.OrganisationId)
            group patient by link.OrganisationId
            into grouped
            select new
            {
                OrganisationId = grouped.Key,
                ManagedPatientCount = grouped.Select(x => x.Id).Distinct().Count(),
                RegisteredPatientCount = grouped.Count(x => x.UserId != null),
                WalkInPatientCount = grouped.Count(x => x.UserId == null)
            })
            .ToDictionaryAsync(x => x.OrganisationId, cancellationToken);

        var items = new List<OrganisationListDto>(orgPage.Count);
        foreach (var org in orgPage)
        {
            var owner = await _identityService.GetUserByIdAsync(org.OwnerId, cancellationToken);
            patientCountLookup.TryGetValue(org.Id, out var patientCounts);

            items.Add(new OrganisationListDto
            {
                Id = org.Id,
                Name = org.Name,
                Address = org.Address,
                LicenseNumber = org.LicenseNumber,
                TaxCode = org.TaxCode,
                OrgType = org.OrgType.ToString(),
                RatingAverage = org.RatingAverage,
                RatingCount = org.RatingCount,
                OwnerAvatarUrl = owner?.AvatarUrl,
                DeviceCount = 0, // TODO: join with Device count when available
                PurchasedAiQuota = org.PurchasedAiQuota,
                MonthlyQuotaLimit = org.MonthlyQuotaLimit,
                MonthlyQuotaUsed = org.MonthlyQuotaUsed,
                MonthlyQuotaRemaining = Math.Max(0, org.MonthlyQuotaLimit - org.MonthlyQuotaUsed),
                ManagedPatientCount = patientCounts?.ManagedPatientCount ?? 0,
                RegisteredPatientCount = patientCounts?.RegisteredPatientCount ?? 0,
                WalkInPatientCount = patientCounts?.WalkInPatientCount ?? 0,
                IsActive = !org.IsDeleted,
                CreatedAt = org.CreatedAt
            });
        }

        var pagedResult = new PagedResult<OrganisationListDto>(
            items, totalCount, request.PageNumber, request.PageSize);

        return Result<PagedResult<OrganisationListDto>>.Success(pagedResult);
    }
}
