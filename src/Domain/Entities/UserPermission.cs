using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// User Permission entity - many-to-many join table with extra fields
/// </summary>
public class UserPermission : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid PermissionId { get; private set; }
    public Guid? GrantedBy { get; private set; }
    public DateTime GrantedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public bool IsActive { get; private set; }

    private UserPermission() { } // EF Core

    public UserPermission(Guid userId, Guid permissionId, Guid? grantedBy = null, DateTime? expiresAt = null)
    {
        UserId = userId;
        PermissionId = permissionId;
        GrantedBy = grantedBy;
        GrantedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        IsActive = true;
    }

    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;

    public void Revoke()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Extend(DateTime newExpiryDate)
    {
        if (newExpiryDate <= DateTime.UtcNow)
            throw new ArgumentException("Expiry date must be in the future", nameof(newExpiryDate));

        ExpiresAt = newExpiryDate;
        UpdatedAt = DateTime.UtcNow;
    }
}
