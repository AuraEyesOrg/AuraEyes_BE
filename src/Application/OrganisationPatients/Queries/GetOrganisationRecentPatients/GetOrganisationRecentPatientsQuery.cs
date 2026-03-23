using Application.Common.Interfaces;

namespace Application.OrganisationPatients.Queries.GetOrganisationRecentPatients;

public record GetOrganisationRecentPatientsQuery(Guid OrgAdminUserId, int Take)
    : IQuery<IReadOnlyList<OrganisationRecentPatientDto>>;
