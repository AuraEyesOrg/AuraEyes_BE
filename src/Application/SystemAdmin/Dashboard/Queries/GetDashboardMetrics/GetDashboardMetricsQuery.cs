using Application.Common.Interfaces;

namespace Application.SystemAdmin.Dashboard.Queries.GetDashboardMetrics;

/// <summary>
/// Query to get System Admin dashboard overview metrics
/// Screen: 3.3.1 View Dashboard, 3.3.2-3.3.4 View Total Screenings, AI Accuracy, Pending Reviews
/// </summary>
public record GetDashboardMetricsQuery : IQuery<DashboardMetricsDto>;
