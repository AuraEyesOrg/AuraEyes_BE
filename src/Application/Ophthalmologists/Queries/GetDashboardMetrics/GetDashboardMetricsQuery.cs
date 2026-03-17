using Application.Common.Interfaces;

namespace Application.Ophthalmologists.Queries.GetDashboardMetrics;

public record GetDashboardMetricsQuery(Guid UserId) : IQuery<OphthalmologistDashboardMetricsDto>;