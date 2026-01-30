using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Organisations.Common;
using Domain.Repositories;

namespace Application.Organisations.Queries.GetOrganisation;

/// <summary>
/// Handler for GetOrganisationQuery.
/// </summary>
public class GetOrganisationQueryHandler : IQueryHandler<GetOrganisationQuery, OrganisationDto>
{
    private readonly IOrganisationRepository _organisationRepository;
    private readonly IIdentityService _identityService;

    public GetOrganisationQueryHandler(
        IOrganisationRepository organisationRepository,
        IIdentityService identityService)
    {
        _organisationRepository = organisationRepository;
        _identityService = identityService;
    }

    public async Task<Result<OrganisationDto>> Handle(GetOrganisationQuery request, CancellationToken cancellationToken)
    {
        var organisation = await _organisationRepository.GetByIdAsync(request.Id, cancellationToken);

        if (organisation is null || organisation.IsDeleted)
        {
            return Result<OrganisationDto>.NotFound($"Organisation with ID '{request.Id}' was not found.");
        }

        // Get owner information
        var owner = await _identityService.GetUserByIdAsync(organisation.OwnerId, cancellationToken);

        var dto = new OrganisationDto
        {
            Id = organisation.Id,
            OwnerId = organisation.OwnerId,
            OwnerFullName = owner?.FullName,
            OwnerEmail = owner?.Email,
            Name = organisation.Name,
            Address = organisation.Address,
            LicenseNumber = organisation.LicenseNumber,
            OrgType = organisation.OrgType.ToString(),
            IsActive = !organisation.IsDeleted,
            CreatedAt = organisation.CreatedAt,
            UpdatedAt = organisation.UpdatedAt
        };

        return Result<OrganisationDto>.Success(dto);
    }
}
