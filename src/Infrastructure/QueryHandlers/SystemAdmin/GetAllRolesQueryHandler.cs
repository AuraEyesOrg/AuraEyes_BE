using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Permissions.Common;
using Application.SystemAdmin.Permissions.Queries.GetAllRoles;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.QueryHandlers.SystemAdmin;

/// <summary>Returns all Identity roles ordered by name.</summary>
public class GetAllRolesQueryHandler : IQueryHandler<GetAllRolesQuery, List<ApplicationRoleDto>>
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public GetAllRolesQueryHandler(RoleManager<ApplicationRole> roleManager)
        => _roleManager = roleManager;

    public async Task<Result<List<ApplicationRoleDto>>> Handle(
        GetAllRolesQuery request,
        CancellationToken cancellationToken)
    {
        var roles = await _roleManager.Roles
            .OrderBy(r => r.Name)
            .Select(r => new ApplicationRoleDto(r.Id, r.Name ?? string.Empty))
            .ToListAsync(cancellationToken);

        return Result<List<ApplicationRoleDto>>.Success(roles);
    }
}
