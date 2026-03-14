using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Permissions.Common;

namespace Application.SystemAdmin.Permissions.Queries.GetPermissionById;

/// <summary>Query: get a single permission by ID.</summary>
public record GetPermissionByIdQuery(Guid PermissionId) : IQuery<PermissionDto>;
