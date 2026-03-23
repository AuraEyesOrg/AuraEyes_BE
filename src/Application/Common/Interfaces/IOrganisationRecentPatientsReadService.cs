using Application.OrganisationPatients;

namespace Application.Common.Interfaces;

public interface IOrganisationRecentPatientsReadService
{
    Task<IReadOnlyList<OrganisationRecentPatientDto>> GetRecentPatientsForOrganisationAdminAsync(
        Guid orgAdminUserId,
        int take,
        CancellationToken cancellationToken = default);
}

