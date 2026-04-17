using Application.Common.Interfaces;

namespace Application.Organisations.Queries.GetBillingSummary;

public record GetBillingSummaryQuery(Guid OrgAdminUserId) : IQuery<OrganisationScreenings.OrgBillingSummaryDto>;
