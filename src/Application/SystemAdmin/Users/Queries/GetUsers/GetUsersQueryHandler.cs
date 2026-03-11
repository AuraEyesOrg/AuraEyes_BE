using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Users.Queries.GetUsers;

/// <summary>
/// Handler for GetUsersQuery - delegates to IIdentityService for user data with roles.
/// </summary>
public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, PagedResult<UserListDto>>
{
    private readonly IIdentityService _identityService;

    public GetUsersQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<PagedResult<UserListDto>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var (users, totalCount) = await _identityService.GetUsersAsync(
            request.SearchTerm,
            request.RoleFilter,
            request.StatusFilter,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var items = users.Select(u => new UserListDto
        {
            Id = u.Id,
            Email = u.Email,
            FullName = u.FullName,
            PhoneNumber = u.PhoneNumber,
            Roles = u.Roles,
            Status = u.Status,
            IsActive = u.IsActive,
            EmailConfirmed = u.EmailConfirmed,
            CreatedAt = u.CreatedAt,
            LastLoginAt = u.LastLoginAt
        }).ToList();

        var pagedResult = new PagedResult<UserListDto>(
            items, totalCount, request.PageNumber, request.PageSize);

        return Result<PagedResult<UserListDto>>.Success(pagedResult);
    }
}
