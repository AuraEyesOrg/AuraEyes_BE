namespace Application.Common.Interfaces;

/// <summary>
/// Current user service - provides access to the authenticated user.
/// Implemented in API layer using HttpContext.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Current authenticated user ID.
    /// </summary>
    Guid? UserId { get; }
    
    /// <summary>
    /// Current user's email.
    /// </summary>
    string? Email { get; }
    
    /// <summary>
    /// Current user's roles.
    /// </summary>
    IEnumerable<string> Roles { get; }
    
    /// <summary>
    /// Check if current user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }
    
    /// <summary>
    /// Check if current user is in a specific role.
    /// </summary>
    bool IsInRole(string role);
}
