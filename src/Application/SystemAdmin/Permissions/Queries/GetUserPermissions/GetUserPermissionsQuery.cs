using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Permissions.Common;

namespace Application.SystemAdmin.Permissions.Queries.GetUserPermissions;

/// <summary>
/// Query: get a user's effective permissions
/// (role-based + user-level overrides, with final computed set).
/// </summary>
public record GetUserPermissionsQuery(Guid UserId) : IQuery<UserEffectivePermissionsDto>;
