using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Identity.Authorization;

public class AuthorizePermissionAttribute : AuthorizeAttribute
{
    public AuthorizePermissionAttribute(params string[] permissions) 
        : base(string.Join(",", permissions))
    {
    }
}
