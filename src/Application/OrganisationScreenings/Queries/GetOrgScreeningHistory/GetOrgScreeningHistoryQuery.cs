using Application.Common.Interfaces;

namespace Application.OrganisationScreenings.Queries.GetOrgScreeningHistory;

public record GetOrgScreeningHistoryQuery : IQuery<IReadOnlyList<OrgScreeningHistoryItemDto>>
{
    public required Guid OrgAdminUserId { get; init; }
    public int Take { get; init; } = 50;
}
