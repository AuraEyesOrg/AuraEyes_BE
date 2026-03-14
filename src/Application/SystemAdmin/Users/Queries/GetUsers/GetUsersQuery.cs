using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Users.Queries.GetUsers;

/// <summary>
/// Query to get users with pagination and filtering
/// Screen: 3.5.1 View User & Role Management Overview
/// </summary>
public record GetUsersQuery : IQuery<PagedResult<UserListDto>>
{
    public string? SearchTerm { get; init; }
    public string? RoleFilter { get; init; }
    public string? StatusFilter { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
