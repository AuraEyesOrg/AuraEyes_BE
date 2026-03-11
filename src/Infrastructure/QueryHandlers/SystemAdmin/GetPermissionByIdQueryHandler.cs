using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Permissions.Common;
using Application.SystemAdmin.Permissions.Queries.GetPermissionById;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.QueryHandlers.SystemAdmin;

/// <summary>Handles GetPermissionByIdQuery.</summary>
public class GetPermissionByIdQueryHandler : IQueryHandler<GetPermissionByIdQuery, PermissionDto>
{
    private readonly ApplicationDbContext _context;

    public GetPermissionByIdQueryHandler(ApplicationDbContext context) => _context = context;

    public async Task<Result<PermissionDto>> Handle(
        GetPermissionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var permission = await _context.Permissions
            .AsNoTracking()
            .Where(p => !p.IsDeleted && p.Id == request.PermissionId)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (permission is null)
            return Result<PermissionDto>.NotFound($"Permission '{request.PermissionId}' not found.");

        return Result<PermissionDto>.Success(permission);
    }
}
