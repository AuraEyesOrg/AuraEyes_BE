using Application.Common.Models;
using Application.SystemAdmin.AuditLogs.Queries.GetAuditLogs;
using Application.SystemAdmin.Ophthalmologists.Queries.GetOphthalmologists;
using Application.SystemAdmin.Patients.Queries.GetPatientMetrics;
using Application.SystemAdmin.Patients.Queries.GetPatients;

namespace Application.SystemAdmin.Interfaces;

/// <summary>
/// Service interface for admin queries that require cross-layer joins
/// (Domain entities joined with Identity/User data).
/// Implemented in Infrastructure layer using ApplicationDbContext.
/// </summary>
public interface IAdminQueryService
{
    Task<PagedResult<OphthalmologistListDto>> GetOphthalmologistsAsync(
        string? searchTerm,
        string? verificationStatus,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PagedResult<PatientListDto>> GetPatientsAsync(
        string? searchTerm,
        string? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PatientMetricsDto> GetPatientMetricsAsync(
        CancellationToken cancellationToken = default);

    Task<PagedResult<AuditLogDto>> GetAuditLogsAsync(
        string? searchTerm,
        string? action,
        string? entityName,
        Guid? userId,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
