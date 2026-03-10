using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Permissions.Common;
using Application.SystemAdmin.Permissions.Queries.GetPermissions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.QueryHandlers.SystemAdmin;

/// <summary>
/// Handles GetPermissionsQuery — returns a paged list of permissions.
/// Placed in Infrastructure to allow direct EF Core query access.
/// </summary>
public class GetPermissionsQueryHandler : IQueryHandler<GetPermissionsQuery, PagedResult<PermissionDto>>
{
    private readonly ApplicationDbContext _context;

    public GetPermissionsQueryHandler(ApplicationDbContext context) => _context = context;

    public async Task<Result<PagedResult<PermissionDto>>> Handle(
        GetPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Permissions
            .AsNoTracking()
            .Where(p => !p.IsDeleted);

        // Filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(term) ||
                p.DisplayName.ToLower().Contains(term) ||
                (p.Description != null && p.Description.ToLower().Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(request.Category))
            query = query.Where(p => p.Category != null && p.Category.ToLower() == request.Category.ToLower());

        if (request.IsActive.HasValue)
            query = query.Where(p => p.IsActive == request.IsActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PermissionDto
            {
                Id = p.Id,
                Name = p.Name,
                DisplayName = p.DisplayName,
                Description = p.Description,
                Category = p.Category,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<PermissionDto>>.Success(
            new PagedResult<PermissionDto>(items, totalCount, request.PageNumber, request.PageSize));
    }
}
