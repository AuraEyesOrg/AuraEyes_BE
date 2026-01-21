using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

/// <summary>
/// User claim entity
/// </summary>
public class ApplicationUserClaim : IdentityUserClaim<Guid>
{
}
