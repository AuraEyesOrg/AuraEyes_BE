using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity;

/// <summary>
/// Refresh token service implementation.
/// Handles refresh token storage, validation, and rotation.
/// </summary>
public class RefreshTokenService : IRefreshTokenService
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateRefreshTokenAsync(
        Guid userId,
        string tokenHash,
        string jwtId,
        int expiryDays = 7,
        string? deviceInfo = null,
        string? ipAddress = null,
        CancellationToken cancellationToken = default)
    {
        var refreshToken = RefreshToken.Create(
            userId,
            tokenHash,
            jwtId,
            expiryDays,
            deviceInfo,
            ipAddress
        );

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return refreshToken.Id;
    }

    public async Task<RefreshTokenDto?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        var token = await _context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

        if (token == null) return null;

        return new RefreshTokenDto(
            token.Id,
            token.UserId,
            token.JwtId,
            token.CreatedAt,
            token.ExpiresAt,
            token.UsedAt,
            token.RevokedAt,
            token.IsActive
        );
    }

    public async Task<Guid> RotateRefreshTokenAsync(
        Guid oldTokenId,
        string newTokenHash,
        string newJwtId,
        int expiryDays = 7,
        string? deviceInfo = null,
        string? ipAddress = null,
        CancellationToken cancellationToken = default)
    {
        var oldToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Id == oldTokenId, cancellationToken);

        if (oldToken == null)
        {
            throw new InvalidOperationException("Original token not found");
        }

        // Create new token
        var newToken = RefreshToken.Create(
            oldToken.UserId,
            newTokenHash,
            newJwtId,
            expiryDays,
            deviceInfo,
            ipAddress
        );

        _context.RefreshTokens.Add(newToken);

        // Mark old token as used
        oldToken.MarkAsUsed(newToken.Id);

        await _context.SaveChangesAsync(cancellationToken);

        return newToken.Id;
    }

    public async Task RevokeTokenAsync(
        Guid tokenId,
        string reason = "manual_revocation",
        CancellationToken cancellationToken = default)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Id == tokenId, cancellationToken);

        if (token != null)
        {
            token.Revoke(reason);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RevokeAllUserTokensAsync(
        Guid userId,
        string reason = "logout_all",
        CancellationToken cancellationToken = default)
    {
        var tokens = await _context.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.Revoke(reason);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeTokenFamilyAsync(
        Guid tokenId,
        string reason = "token_reuse_detected",
        CancellationToken cancellationToken = default)
    {
        // Find the original token and trace the chain
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Id == tokenId, cancellationToken);

        if (token == null) return;

        // Revoke all tokens for this user as a security measure
        // This is the recommended approach for token reuse detection
        await RevokeAllUserTokensAsync(token.UserId, reason, cancellationToken);
    }

    public async Task<int> CleanupExpiredTokensAsync(
        int daysToKeep = 30,
        CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);

        var tokensToDelete = await _context.RefreshTokens
            .Where(t => t.ExpiresAt < cutoffDate ||
                       (t.RevokedAt.HasValue && t.RevokedAt < cutoffDate))
            .ToListAsync(cancellationToken);

        _context.RefreshTokens.RemoveRange(tokensToDelete);
        await _context.SaveChangesAsync(cancellationToken);

        return tokensToDelete.Count;
    }
}
