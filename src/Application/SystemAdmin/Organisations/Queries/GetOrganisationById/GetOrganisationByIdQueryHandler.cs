using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Organisations.Queries.GetOrganisations;
using Domain.Common;
using Domain.Entities.Users;

namespace Application.SystemAdmin.Organisations.Queries.GetOrganisationById;

public class GetOrganisationByIdQueryHandler
    : IQueryHandler<GetOrganisationByIdQuery, OrganisationListDto>
{
    private readonly IRepository<Organisation> _organisationRepository;

    public GetOrganisationByIdQueryHandler(IRepository<Organisation> organisationRepository)
    {
        _organisationRepository = organisationRepository;
    }

    public async Task<Result<OrganisationListDto>> Handle(
        GetOrganisationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var organisation = await _organisationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (organisation is null)
            return Result<OrganisationListDto>.NotFound("Organisation not found");

        var dto = new OrganisationListDto
        {
            Id = organisation.Id,
            Name = organisation.Name,
            Address = organisation.Address,
            LicenseNumber = organisation.LicenseNumber,
            OrgType = organisation.OrgType.ToString(),
            DeviceCount = 0,
            PurchasedAiQuota = organisation.PurchasedAiQuota,
            ManagedPatientCount = 0,
            RegisteredPatientCount = 0,
            WalkInPatientCount = 0,
            IsActive = !organisation.IsDeleted,
            CreatedAt = organisation.CreatedAt,
        };

        return Result<OrganisationListDto>.Success(dto);
    }
}
