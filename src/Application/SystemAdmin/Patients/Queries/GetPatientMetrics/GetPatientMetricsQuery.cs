using Application.Common.Interfaces;

namespace Application.SystemAdmin.Patients.Queries.GetPatientMetrics;

public record GetPatientMetricsQuery : IQuery<PatientMetricsDto>;
