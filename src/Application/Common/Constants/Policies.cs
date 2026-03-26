namespace Application.Common.Constants;

/// <summary>
/// Authorization policy names.
/// Used with [Authorize(Policy = Policies.XYZ)] attribute.
/// </summary>
public static class Policies
{
    /// <summary>
    /// Requires authenticated user.
    /// </summary>
    public const string Authenticated = nameof(Authenticated);

    /// <summary>
    /// Requires Patient role.
    /// </summary>
    public const string PatientOnly = nameof(PatientOnly);

    /// <summary>
    /// Requires Ophthalmologist role.
    /// </summary>
    public const string OphthalmologistOnly = nameof(OphthalmologistOnly);

    /// <summary>
    /// Requires OrgAdmin role.
    /// </summary>
    public const string OrgAdminOnly = nameof(OrgAdminOnly);

    /// <summary>
    /// Requires either Ophthalmologist or OrgAdmin role.
    /// Used for managing schedules and available slots.
    /// </summary>
    public const string OphthalmologistOrOrgAdmin = nameof(OphthalmologistOrOrgAdmin);

    /// <summary>
    /// Requires SystemAdmin role.
    /// </summary>
    public const string SystemAdminOnly = nameof(SystemAdminOnly);

    /// <summary>
    /// Requires any admin role (OrgAdmin or SystemAdmin).
    /// </summary>
    public const string AdminsOnly = nameof(AdminsOnly);

    /// <summary>
    /// Requires medical staff role (Patient or Ophthalmologist).
    /// </summary>
    public const string MedicalStaff = nameof(MedicalStaff);

    /// <summary>
    /// Requires verified ophthalmologist.
    /// </summary>
    public const string VerifiedOphthalmologist = nameof(VerifiedOphthalmologist);

    /// <summary>
    /// Requires user to belong to an organization.
    /// </summary>
    public const string OrganizationMember = nameof(OrganizationMember);
}
