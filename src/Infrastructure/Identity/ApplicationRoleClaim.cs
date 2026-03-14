using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

/// <summary>
/// Role claim entity
/// </summary>
public class ApplicationRoleClaim : IdentityRoleClaim<Guid>
{
}
