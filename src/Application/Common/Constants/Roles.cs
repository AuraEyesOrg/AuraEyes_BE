namespace Application.Common.Constants;

/// <summary>
/// Application role constants.
/// Centralized role definitions for authorization.
/// </summary>
public static class Roles
{
    /// <summary>
    /// Patient role - end users who use the screening service.
    /// </summary>
    public const string Patient = nameof(Patient);
    
    /// <summary>
    /// Ophthalmologist role - medical professionals who review screenings.
    /// </summary>
    public const string Ophthalmologist = nameof(Ophthalmologist);
    
    /// <summary>
    /// Organization Admin role - manages an organization's users and settings.
    /// </summary>
    public const string OrgAdmin = nameof(OrgAdmin);
    
    /// <summary>
    /// System Admin role - full system access.
    /// </summary>
    public const string SystemAdmin = nameof(SystemAdmin);
    
    /// <summary>
    /// All available roles.
    /// </summary>
    public static readonly string[] All = { Patient, Ophthalmologist, OrgAdmin, SystemAdmin };
    
    /// <summary>
    /// Medical roles (Patient + Ophthalmologist).
    /// </summary>
    public static readonly string[] Medical = { Patient, Ophthalmologist };
    
    /// <summary>
    /// Admin roles (OrgAdmin + SystemAdmin).
    /// </summary>
    public static readonly string[] Admins = { OrgAdmin, SystemAdmin };
}
