using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

/// <summary>
/// Extended UserToken with additional fields
/// </summary>
public class ApplicationUserToken : IdentityUserToken<Guid>
{
    public DateTime? ExpiryDate { get; set; }
    public string? DeviceInfo { get; set; }
    public string? IpAddress { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReasonRevoked { get; set; }

    public ApplicationUserToken()
    {
        CreatedAt = DateTime.UtcNow;
        IsRevoked = false;
    }
}
