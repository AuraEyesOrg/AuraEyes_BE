using Application.Common.Interfaces;

namespace Application.Guest.Queries.GetOverviewMetrics;

public record GetOverviewMetricsQuery : IQuery<GuestOverviewMetricsDto>;
