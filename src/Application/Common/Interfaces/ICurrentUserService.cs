namespace Application.Common.Interfaces;

/// <summary>
/// Current user service - provides access to the authenticated user.
/// Implemented in API layer using HttpContext.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Current authenticated user ID (ApplicationUser.Id).
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Domain profile entity ID (Patient.Id or Ophthalmologist.Id).
    /// Resolved from the "profile_id" JWT claim. Use this for authorization checks
    /// </summary>
    Guid? ProfileId { get; }

    /// <summary>
    /// Current user's full name or display name.
    /// </summary>
    string? UserName { get; }

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
    /// Client IP address from the current HTTP request.
    /// </summary>
    string? IpAddress { get; }

    /// <summary>
    /// Check if current user is in a specific role.
    /// </summary>
    bool IsInRole(string role);
}
