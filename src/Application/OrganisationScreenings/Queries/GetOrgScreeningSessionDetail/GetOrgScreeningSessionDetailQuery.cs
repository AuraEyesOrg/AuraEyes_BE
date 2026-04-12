using Application.Common.Interfaces;
using Application.Screenings.Queries.GetScreeningSessionDetail;

namespace Application.OrganisationScreenings.Queries.GetOrgScreeningSessionDetail;

public record GetOrgScreeningSessionDetailQuery(Guid OrgAdminUserId, Guid ScreeningId)
    : IQuery<ScreeningSessionDetailDto>;
