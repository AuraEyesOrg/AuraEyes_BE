using Application.Ophthalmologists.Queries.GetDashboardMetrics;
using Application.Organisations.Queries.GetDashboardMetrics;
using Application.Patients.Queries.GetDashboardMetrics;
using Application.SystemAdmin.Dashboard.Queries.GetDashboardMetrics;
using Application.SystemAdmin.Dashboard.Queries.GetPopulationRiskAnalysis;
using Application.SystemAdmin.Dashboard.Queries.GetRecentScreenings;
using Application.SystemAdmin.Dashboard.Queries.GetScreeningVolumeTrends;
using Application.SystemAdmin.Dashboard.Queries.GetSystemHealth;
using Application.SystemAdmin.Dashboard.Queries.GetDoctorWorkload;
using Application.SystemAdmin.Dashboard.Queries.GetDoctorWorkloads;
using Application.Common.Models;
using Domain.Enums;

namespace Application.Common.Interfaces;

public interface IDashboardMetricsService
{
    Task<DashboardMetricsDto> GetSystemAdminMetricsAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<RecentScreeningDto>> GetRecentScreeningsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<ScreeningVolumeTrendsDto> GetScreeningVolumeTrendsAsync(string timeRange, int periods, CancellationToken cancellationToken = default);
    Task<PopulationRiskAnalysisDto> GetPopulationRiskAnalysisAsync(CancellationToken cancellationToken = default);
    Task<SystemHealthDto> GetSystemHealthAsync(CancellationToken cancellationToken = default);
    Task<DoctorWorkloadDto?> GetDoctorWorkloadAsync(
        Guid doctorId,
        WorkloadPeriodType periodType,
        DateOnly date,
        CancellationToken cancellationToken = default);
    Task<PagedResult<DoctorWorkloadListItemDto>> GetDoctorWorkloadsAsync(
        WorkloadPeriodType periodType,
        DateOnly date,
        string? searchTerm,
        OphthalmologistEmploymentType? employmentType,
        string? status,
        bool warningOnly,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<OphthalmologistDashboardMetricsDto> GetOphthalmologistMetricsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<OrganisationDashboardMetricsDto> GetOrganisationMetricsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<PatientDashboardMetricsDto> GetPatientMetricsAsync(Guid userId, CancellationToken cancellationToken = default);
}