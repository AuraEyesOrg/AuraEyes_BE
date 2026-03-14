using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

/// <summary>
/// User login entity for external providers
/// </summary>
public class ApplicationUserLogin : IdentityUserLogin<Guid>
{
}
