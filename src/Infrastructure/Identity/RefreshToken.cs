namespace Infrastructure.Identity;

/// <summary>
/// Refresh token entity for JWT token rotation.
/// 
/// Design decisions:
/// - Placed in Infrastructure (not Domain): RefreshToken is an identity/auth concern,
///   not a core business entity. It's tightly coupled to ApplicationUser.
/// - Token stored as hash: Security best practice (never store plain tokens)
/// - Rotation support: New token issued on each refresh, old token revoked
/// - Device/family tracking: Optional fields for advanced scenarios
/// 
/// Security considerations:
/// - Tokens are hashed using SHA256 before storage
/// - One user can have multiple tokens (multi-device support)
/// - Expired and revoked tokens are kept for audit (can be cleaned up periodically)
/// </summary>
public class RefreshToken
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// SHA256 hash of the actual token. Never store plain text tokens.
    /// </summary>
    public string TokenHash { get; set; } = string.Empty;
    
    /// <summary>
    /// Jti (JWT ID) of the associated access token.
    /// Used for token binding and revocation.
    /// </summary>
    public string JwtId { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    
    /// <summary>
    /// When the token was used (for rotation tracking).
    /// </summary>
    public DateTime? UsedAt { get; set; }
    
    /// <summary>
    /// When the token was revoked (logout, security event).
    /// </summary>
    public DateTime? RevokedAt { get; set; }
    
    /// <summary>
    /// Reason for revocation (logout, token_rotation, security_breach, etc.)
    /// </summary>
    public string? RevokedReason { get; set; }
    
    /// <summary>
    /// If this token was replaced by another token (rotation chain).
    /// </summary>
    public Guid? ReplacedByTokenId { get; set; }
    
    /// <summary>
    /// Device information for multi-device management.
    /// </summary>
    public string? DeviceInfo { get; set; }
    
    /// <summary>
    /// IP address when token was created.
    /// </summary>
    public string? IpAddress { get; set; }
    
    // Foreign key
    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;
    
    // Computed properties
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsActive => !IsRevoked && !IsExpired && !UsedAt.HasValue;

    public RefreshToken()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Factory method to create a new refresh token.
    /// </summary>
    public static RefreshToken Create(
        Guid userId, 
        string tokenHash, 
        string jwtId, 
        int expiryDays = 7,
        string? deviceInfo = null,
        string? ipAddress = null)
    {
        return new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            JwtId = jwtId,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
            DeviceInfo = deviceInfo,
            IpAddress = ipAddress
        };
    }
    
    /// <summary>
    /// Mark token as used during rotation.
    /// </summary>
    public void MarkAsUsed(Guid replacedByTokenId)
    {
        UsedAt = DateTime.UtcNow;
        ReplacedByTokenId = replacedByTokenId;
    }
    
    /// <summary>
    /// Revoke the token.
    /// </summary>
    public void Revoke(string reason = "manual_revocation")
    {
        RevokedAt = DateTime.UtcNow;
        RevokedReason = reason;
    }
}
