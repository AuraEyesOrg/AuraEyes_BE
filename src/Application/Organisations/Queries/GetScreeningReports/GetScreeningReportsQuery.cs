using Application.Common.Interfaces;

namespace Application.Organisations.Queries.GetScreeningReports;

public record GetScreeningReportsQuery(Guid OrgAdminUserId) : IQuery<OrganisationScreenings.OrgScreeningReportDto>;
