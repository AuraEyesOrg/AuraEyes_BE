using System.Security.Claims;
using Application.Common.Interfaces;

namespace API.Services;

/// <summary>
/// Service to access current authenticated user information.
/// Reads from HttpContext.User claims.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("uid")?.Value
                           ?? _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    public Guid? ProfileId
    {
        get
        {
            var profileIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("profile_id")?.Value;
            return Guid.TryParse(profileIdClaim, out var profileId) ? profileId : null;
        }
    }

    public string? Email => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value
                         ?? _httpContextAccessor.HttpContext?.User.FindFirst("email")?.Value;

    public IEnumerable<string> Roles => _httpContextAccessor.HttpContext?.User
        .FindAll(ClaimTypes.Role)
        .Select(c => c.Value)
        ?? Enumerable.Empty<string>();

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    public string? IpAddress => _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

    public bool IsInRole(string role) => _httpContextAccessor.HttpContext?.User.IsInRole(role) ?? false;
}
