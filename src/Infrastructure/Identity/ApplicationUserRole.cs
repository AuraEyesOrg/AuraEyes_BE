using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

/// <summary>
/// User role join entity
/// </summary>
public class ApplicationUserRole : IdentityUserRole<Guid>
{
}
