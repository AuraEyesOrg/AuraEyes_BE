using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

/// <summary>
/// Application role extending ASP.NET Core Identity
/// DO NOT inherit from BaseEntity - Identity has its own base
/// </summary>
public class ApplicationRole : IdentityRole<Guid>
{
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public ApplicationRole()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public ApplicationRole(string roleName) : this()
    {
        Name = roleName;
    }
}
