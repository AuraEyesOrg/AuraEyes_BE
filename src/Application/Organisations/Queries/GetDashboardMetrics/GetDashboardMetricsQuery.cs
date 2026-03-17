using Application.Common.Interfaces;

namespace Application.Organisations.Queries.GetDashboardMetrics;

public record GetDashboardMetricsQuery(Guid UserId) : IQuery<OrganisationDashboardMetricsDto>;