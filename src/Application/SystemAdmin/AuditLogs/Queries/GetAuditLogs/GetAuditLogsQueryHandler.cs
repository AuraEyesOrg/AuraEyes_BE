using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.AuditLogs.Queries.GetAuditLogs;

/// <summary>
/// Handler for GetAuditLogsQuery - Returns mock data
/// </summary>
public class GetAuditLogsQueryHandler : IQueryHandler<GetAuditLogsQuery, PagedResult<AuditLogDto>>
{
    public Task<Result<PagedResult<AuditLogDto>>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        // Mock audit logs
        var items = new List<AuditLogDto>
        {
            new() { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), UserName = "admin@aura.com", Action = "UserLogin", EntityName = "User", EntityId = null, IpAddress = "192.168.1.100", CreatedAt = DateTime.UtcNow.AddHours(-1) },
            new() { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), UserName = "doctor1@aura.com", Action = "ScreeningCreated", EntityName = "Screening", EntityId = Guid.NewGuid().ToString(), IpAddress = "192.168.1.101", CreatedAt = DateTime.UtcNow.AddHours(-2) },
            new() { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), UserName = "admin@aura.com", Action = "UserRoleUpdated", EntityName = "User", EntityId = Guid.NewGuid().ToString(), OldValue = "{\"Role\":\"Staff\"}", NewValue = "{\"Role\":\"Doctor\"}", IpAddress = "192.168.1.100", CreatedAt = DateTime.UtcNow.AddHours(-3) },
            new() { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), UserName = "doctor2@aura.com", Action = "ScreeningApproved", EntityName = "Screening", EntityId = Guid.NewGuid().ToString(), IpAddress = "192.168.1.102", CreatedAt = DateTime.UtcNow.AddHours(-4) },
            new() { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), UserName = "admin@aura.com", Action = "OrganisationCreated", EntityName = "Organisation", EntityId = Guid.NewGuid().ToString(), IpAddress = "192.168.1.100", CreatedAt = DateTime.UtcNow.AddHours(-5) }
        };

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            items = items.Where(x =>
                x.Action.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                x.EntityName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (x.UserName?.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
            ).ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.Action))
        {
            items = items.Where(x => x.Action.Equals(request.Action, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.EntityName))
        {
            items = items.Where(x => x.EntityName.Equals(request.EntityName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        var pagedResult = new PagedResult<AuditLogDto>(items, 500, request.PageNumber, request.PageSize);
        return Task.FromResult(Result<PagedResult<AuditLogDto>>.Success(pagedResult));
    }
}
