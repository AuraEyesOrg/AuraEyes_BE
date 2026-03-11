using Application.Common.Interfaces;
using Application.SystemAdmin.Permissions.Common;

namespace Application.SystemAdmin.Permissions.Queries.GetAllRoles;

/// <summary>Returns all Identity roles in the system (id + name).</summary>
public record GetAllRolesQuery : IQuery<List<ApplicationRoleDto>>;
