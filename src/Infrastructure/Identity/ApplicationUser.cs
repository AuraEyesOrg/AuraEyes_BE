using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

/// <summary>
/// Application user extending ASP.NET Core Identity.
/// DO NOT inherit from BaseEntity - Identity has its own base class.
/// 
/// Design decisions:
/// - IsActive: For account status (can be toggled by admin)
/// - IsDeleted: For soft-delete (GDPR compliance, audit trail)
/// - No manual Guid generation: Identity framework handles Id assignment
/// - RefreshTokens: One-to-many for token rotation and multi-device support
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    // Profile Information
    public string FullName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? AvatarUrl { get; set; }
    public string? CitizenId { get; set; }
    // Account Status (separate concerns)
    /// <summary>
    /// Indicates if the account is active. Inactive accounts cannot login.
    /// Controlled by administrators.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Soft delete flag for GDPR compliance and audit trail.
    /// Deleted users are excluded from queries but retained for historical records.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    // Audit Fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Organization Association
    public Guid? OrganizationId { get; set; }

    // Security policy flags
    public bool MustChangePassword { get; set; } = false;

    // Navigation Properties
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public ApplicationUser()
    {
        // Note: Do NOT set Id = Guid.NewGuid() here.
        // ASP.NET Core Identity uses IdentityUser<Guid> which auto-generates Guid
        // when the entity is created via UserManager.CreateAsync()
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
        IsDeleted = false;
    }

    // Domain methods for state changes
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }
}
