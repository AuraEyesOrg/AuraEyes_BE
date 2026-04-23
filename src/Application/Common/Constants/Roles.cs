namespace Application.Common.Constants;

/// <summary>
/// Application role constants — Digital Clinic model.
/// Centralized role definitions for authorization.
/// </summary>
public static class Roles
{
    /// <summary>Patient role — end users who use the retinal screening service.</summary>
    public const string Patient = nameof(Patient);

    /// <summary>Ophthalmologist role — medical professionals who diagnose and treat.</summary>
    public const string Ophthalmologist = nameof(Ophthalmologist);

    /// <summary>
    /// ClinicStaff role — clinic employees (Receptionist, Coordinator, Cashier).
    /// Sub-roles are managed via the ClinicStaff entity and fine-grained UserPermissions.
    /// </summary>
    public const string ClinicStaff = nameof(ClinicStaff);

    /// <summary>SystemAdmin role — clinic owner, full system access.</summary>
    public const string SystemAdmin = nameof(SystemAdmin);

    /// <summary>All available roles.</summary>
    public static readonly string[] All = { Patient, Ophthalmologist, ClinicStaff, SystemAdmin };

    /// <summary>Medical roles — Ophthalmologist + Patient.</summary>
    public static readonly string[] Medical = { Patient, Ophthalmologist };

    /// <summary>Admin-level roles.</summary>
    public static readonly string[] Admins = { SystemAdmin };

    /// <summary>Clinical staff roles — everyone who works inside the clinic.</summary>
    public static readonly string[] ClinicalTeam = { Ophthalmologist, ClinicStaff };

    /// <summary>All staff (non-patient) roles.</summary>
    public static readonly string[] Staff = { Ophthalmologist, ClinicStaff, SystemAdmin };
}
