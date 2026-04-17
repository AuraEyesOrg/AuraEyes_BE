using Application.Common.Constants;
using Application.Common.Models;
using Application.SystemAdmin.AuditLogs.Queries.GetAuditLogs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Infrastructure.Identity.Authorization;

namespace API.Controllers.SystemAdmin;

/// <summary>
/// System Admin Audit Logs and Compliance endpoints
/// Provides audit trail access for compliance and security monitoring.
/// </summary>
[Route("api/system-admin/audit-logs")]
[AuthorizePermission(Permissions.AuditLogsRead)]
public class AuditLogsController : BaseApiController
{
    private readonly IMediator _mediator;

    public AuditLogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get audit logs with pagination and filtering
    /// </summary>
    /// <param name="searchTerm">Search in action, entity name, or entity ID</param>
    /// <param name="action">Filter by action type</param>
    /// <param name="entityName">Filter by entity name</param>
    /// <param name="userId">Filter by user ID</param>
    /// <param name="fromDate">Filter from date</param>
    /// <param name="toDate">Filter to date</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <remarks>
    /// Screen: Audit Logs and Compliance
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AuditLogDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? action = null,
        [FromQuery] string? entityName = null,
        [FromQuery] Guid? userId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetAuditLogsQuery
        {
            SearchTerm = searchTerm,
            Action = action,
            EntityName = entityName,
            UserId = userId,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }
}
