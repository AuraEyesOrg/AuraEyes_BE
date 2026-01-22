using Application.Common.Interfaces;

namespace Application.SystemAdmin.Organisations.Queries.GetOrganisationMetrics;

/// <summary>
/// Query to get organisation overview metrics
/// Screen: 3.4.2-3.4.3 View Active Clinics Metric, Calibration Required Metric
/// </summary>
public record GetOrganisationMetricsQuery : IQuery<OrganisationMetricsDto>;
