using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Interfaces;

namespace Application.SystemAdmin.AuditLogs.Queries.GetAuditLogs;

/// <summary>
/// Handler for GetAuditLogsQuery - Queries real audit log data from the database.
/// FR-43: Log system activities and financial transactions for auditing and compliance.
/// </summary>
public class GetAuditLogsQueryHandler : IQueryHandler<GetAuditLogsQuery, PagedResult<AuditLogDto>>
{
    private readonly IAdminQueryService _adminQueryService;

    public GetAuditLogsQueryHandler(IAdminQueryService adminQueryService)
    {
        _adminQueryService = adminQueryService;
    }

    public async Task<Result<PagedResult<AuditLogDto>>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var pagedResult = await _adminQueryService.GetAuditLogsAsync(
            request.SearchTerm,
            request.Action,
            request.EntityName,
            request.UserId,
            request.FromDate,
            request.ToDate,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return Result<PagedResult<AuditLogDto>>.Success(pagedResult);
    }
}
