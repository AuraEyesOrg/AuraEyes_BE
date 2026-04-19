using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Organisations.Common;
using Domain.Common;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Organisations.Queries.GetOrganisationSettings;

public class GetOrganisationSettingsQueryHandler
    : IQueryHandler<GetOrganisationSettingsQuery, OrganisationSettingsDto>
{
    private readonly IRepository<Organisation> _organisationRepository;
    private readonly IIdentityService _identityService;

    public GetOrganisationSettingsQueryHandler(
        IRepository<Organisation> organisationRepository,
        IIdentityService identityService)
    {
        _organisationRepository = organisationRepository;
        _identityService = identityService;
    }

    public async Task<Result<OrganisationSettingsDto>> Handle(
        GetOrganisationSettingsQuery request,
        CancellationToken cancellationToken)
    {
        var organisation = await _organisationRepository.Query()
            .FirstOrDefaultAsync(o => o.OwnerId == request.OrgAdminUserId, cancellationToken);

        if (organisation is null)
        {
            return Result<OrganisationSettingsDto>.NotFound("Organisation not found.");
        }

        var owner = await _identityService.GetUserByIdAsync(request.OrgAdminUserId, cancellationToken);
        var ownerDetails = await _identityService.GetUserDetailsAsync(request.OrgAdminUserId, cancellationToken);

        return Result<OrganisationSettingsDto>.Success(new OrganisationSettingsDto
        {
            OrganisationId = organisation.Id,
            Name = organisation.Name,
            OrgType = organisation.OrgType.ToString(),
            Address = organisation.Address,
            LicenseNumber = organisation.LicenseNumber,
            TaxCode = organisation.TaxCode,
            Description = organisation.Description,
            ContactFullName = owner?.FullName ?? string.Empty,
            ContactEmail = owner?.Email ?? string.Empty,
            ContactPhone = ownerDetails?.PhoneNumber,
            AvatarUrl = ownerDetails?.AvatarUrl
        });
    }
}
