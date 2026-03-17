using Application.Common.Interfaces;

namespace Application.Patients.Queries.GetDashboardMetrics;

public record GetDashboardMetricsQuery(Guid UserId) : IQuery<PatientDashboardMetricsDto>;