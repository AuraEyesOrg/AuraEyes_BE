using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.AuditLogs.Queries.GetAuditLogs;

/// <summary>
/// Query to get audit logs with pagination and filtering
/// Screen: Audit Logs & Compliance
/// </summary>
public record GetAuditLogsQuery : IQuery<PagedResult<AuditLogDto>>
{
    public string? SearchTerm { get; init; }
    public string? Action { get; init; }
    public string? EntityName { get; init; }
    public Guid? UserId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
