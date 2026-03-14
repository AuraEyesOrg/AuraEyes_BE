namespace Application.Common.Interfaces;

/// <summary>
/// JWT token service interface.
/// Defined in Application layer for dependency inversion.
/// Implemented in Infrastructure layer.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generate JWT access token for a user.
    /// </summary>
    Task<TokenResult> GenerateAccessTokenAsync(
        Guid userId, 
        string email, 
        string fullName,
        IEnumerable<string> roles,
        IEnumerable<System.Security.Claims.Claim>? additionalClaims = null);
    
    /// <summary>
    /// Generate a secure refresh token.
    /// </summary>
    string GenerateRefreshToken();
    
    /// <summary>
    /// Validate a JWT token and return the principal.
    /// </summary>
    System.Security.Claims.ClaimsPrincipal? ValidateToken(string token);
    
    /// <summary>
    /// Get user ID from token claims.
    /// </summary>
    Guid? GetUserIdFromToken(string token);
    
    /// <summary>
    /// Get Jti (JWT ID) from token.
    /// </summary>
    string? GetJtiFromToken(string token);
}

/// <summary>
/// Token generation result.
/// </summary>
public record TokenResult(
    string AccessToken,
    string Jti,
    DateTime ExpiresAt
);
