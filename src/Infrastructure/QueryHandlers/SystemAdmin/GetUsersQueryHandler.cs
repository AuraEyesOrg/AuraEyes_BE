using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Users.Queries.GetUsers;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.QueryHandlers.SystemAdmin;

/// <summary>
/// Handler for GetUsersQuery - queries real user data with roles.
/// </summary>
public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, PagedResult<UserListDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUsersQueryHandler(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<Result<PagedResult<UserListDto>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Users.AsNoTracking()
            .Where(u => !u.IsDeleted);

        // Search filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(u =>
                u.FullName.ToLower().Contains(term) ||
                u.Email!.ToLower().Contains(term) ||
                (u.UserName != null && u.UserName.ToLower().Contains(term)));
        }

        // Status filter
        if (!string.IsNullOrWhiteSpace(request.StatusFilter))
        {
            query = request.StatusFilter.ToLower() switch
            {
                "active" => query.Where(u => u.IsActive && u.EmailConfirmed),
                "pending" => query.Where(u => !u.EmailConfirmed),
                "suspended" => query.Where(u => !u.IsActive),
                _ => query
            };
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Get roles for each user
        var items = new List<UserListDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            // Apply role filter
            if (!string.IsNullOrWhiteSpace(request.RoleFilter) &&
                !roles.Contains(request.RoleFilter, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            var status = !user.EmailConfirmed ? "Pending"
                : !user.IsActive ? "Suspended"
                : "Active";

            items.Add(new UserListDto
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Roles = roles.ToList(),
                Status = status,
                IsActive = user.IsActive,
                EmailConfirmed = user.EmailConfirmed,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            });
        }

        var pagedResult = new PagedResult<UserListDto>(items, totalCount, request.PageNumber, request.PageSize);
        return Result<PagedResult<UserListDto>>.Success(pagedResult);
    }
}
