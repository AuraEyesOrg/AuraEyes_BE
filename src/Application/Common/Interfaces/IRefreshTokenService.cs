namespace Application.Common.Interfaces;

/// <summary>
/// Refresh token service interface.
/// Handles refresh token storage, validation, and rotation.
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>
    /// Store a new refresh token for a user.
    /// </summary>
    Task<Guid> CreateRefreshTokenAsync(
        Guid userId,
        string tokenHash,
        string jwtId,
        int expiryDays = 7,
        string? deviceInfo = null,
        string? ipAddress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate and get refresh token by its hash.
    /// </summary>
    Task<RefreshTokenDto?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Rotate refresh token (mark old as used, create new).
    /// </summary>
    Task<Guid> RotateRefreshTokenAsync(
        Guid oldTokenId,
        string newTokenHash,
        string newJwtId,
        int expiryDays = 7,
        string? deviceInfo = null,
        string? ipAddress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoke a specific refresh token.
    /// </summary>
    Task RevokeTokenAsync(
        Guid tokenId,
        string reason = "manual_revocation",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoke all refresh tokens for a user (logout from all devices).
    /// </summary>
    Task RevokeAllUserTokensAsync(
        Guid userId,
        string reason = "logout_all",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoke token family (detect token reuse attack).
    /// </summary>
    Task RevokeTokenFamilyAsync(
        Guid tokenId,
        string reason = "token_reuse_detected",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Clean up expired tokens (maintenance job).
    /// </summary>
    Task<int> CleanupExpiredTokensAsync(
        int daysToKeep = 30,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Refresh token DTO for cross-layer communication.
/// </summary>
public record RefreshTokenDto(
    Guid Id,
    Guid UserId,
    string JwtId,
    DateTime CreatedAt,
    DateTime ExpiresAt,
    DateTime? UsedAt,
    DateTime? RevokedAt,
    bool IsActive
);
