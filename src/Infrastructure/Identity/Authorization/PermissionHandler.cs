using Microsoft.AspNetCore.Authorization;
using Application.Common.Constants;

namespace Infrastructure.Identity.Authorization;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        PermissionRequirement requirement)
    {
        if (context.User == null)
        {
            return Task.CompletedTask;
        }

        // System Admin bypass (optional, if you want SysAdmin to have ALL permissions)
        if (context.User.IsInRole(Roles.SystemAdmin))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Check for specific permission claim
        var permissions = context.User.FindAll("permission");
        if (permissions.Any(p => p.Value == requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
