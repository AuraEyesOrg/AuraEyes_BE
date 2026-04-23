namespace Application.Common.Constants;

/// <summary>
/// Authorization policy names — Digital Clinic model.
/// Used with [Authorize(Policy = Policies.XYZ)] attribute.
/// </summary>
public static class Policies
{
    /// <summary>Requires authenticated user.</summary>
    public const string Authenticated = nameof(Authenticated);

    /// <summary>Requires Patient role.</summary>
    public const string PatientOnly = nameof(PatientOnly);

    /// <summary>Requires Ophthalmologist role.</summary>
    public const string OphthalmologistOnly = nameof(OphthalmologistOnly);

    /// <summary>Requires ClinicStaff role (Receptionist / Coordinator / Cashier).</summary>
    public const string ClinicStaffOnly = nameof(ClinicStaffOnly);

    /// <summary>Requires SystemAdmin role (Clinic Owner).</summary>
    public const string SystemAdminOnly = nameof(SystemAdminOnly);

    /// <summary>Requires any admin-level role (currently only SystemAdmin).</summary>
    public const string AdminsOnly = nameof(AdminsOnly);

    /// <summary>Requires medical staff role (Patient or Ophthalmologist).</summary>
    public const string MedicalStaff = nameof(MedicalStaff);

    /// <summary>Requires verified Ophthalmologist (IsVerified claim = True).</summary>
    public const string VerifiedOphthalmologist = nameof(VerifiedOphthalmologist);

    /// <summary>Requires Ophthalmologist or ClinicStaff role — all clinic team members.</summary>
    public const string ClinicalTeam = nameof(ClinicalTeam);

    /// <summary>
    /// Backward-compat alias for ClinicalTeam — previously named OphthalmologistOrOrgAdmin.
    /// Controllers using this policy now correctly target Ophthalmologist + ClinicStaff roles.
    /// </summary>
    public const string OphthalmologistOrOrgAdmin = ClinicalTeam;
}
