using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Identity.Authorization;

public class AuthorizePermissionAttribute : AuthorizeAttribute
{
    public AuthorizePermissionAttribute(string permission) : base(permission)
    {
    }
}
