namespace Application.Common.Constants;

/// <summary>
/// Centralized permission name constants.
/// 
/// Convention: "{resource}:{action}"
/// These names are seeded into the Permissions table on first startup.
/// They match what the app actually supports — do NOT create permissions here
/// unless you have an endpoint / business rule that checks for them.
/// 
/// Default role assignments are also seeded (see DatabaseSeeder.SeedPermissionsAsync).
/// Per-user overrides are managed at runtime via the Admin UI (/api/system-admin/permissions/users).
/// </summary>
public static class Permissions
{
    // ─── Users ────────────────────────────────────────────────────────────────
    public const string UsersRead   = "users:read";
    public const string UsersCreate = "users:create";
    public const string UsersUpdate = "users:update";
    public const string UsersDelete = "users:delete";
    public const string UsersManageRoles = "users:manage-roles";

    // ─── Permissions ──────────────────────────────────────────────────────────
    public const string PermissionsRead   = "permissions:read";
    public const string PermissionsManage = "permissions:manage";

    // ─── Patients ─────────────────────────────────────────────────────────────
    public const string PatientsRead   = "patients:read";
    public const string PatientsCreate = "patients:create";
    public const string PatientsUpdate = "patients:update";
    public const string PatientsDelete = "patients:delete";

    // ─── Ophthalmologists ─────────────────────────────────────────────────────
    public const string OphthalmologistsRead   = "ophthalmologists:read";
    public const string OphthalmologistsCreate = "ophthalmologists:create";
    public const string OphthalmologistsUpdate = "ophthalmologists:update";
    public const string OphthalmologistsVerify = "ophthalmologists:verify";

    // ─── Organisations ────────────────────────────────────────────────────────
    public const string OrganisationsRead   = "organisations:read";
    public const string OrganisationsCreate = "organisations:create";
    public const string OrganisationsUpdate = "organisations:update";
    public const string OrganisationsDelete = "organisations:delete";

    // ─── Screening ────────────────────────────────────────────────────────────
    public const string ScreeningRead    = "screening:read";
    public const string ScreeningCreate  = "screening:create";
    public const string ScreeningApprove = "screening:approve";

    // ─── Consultations ────────────────────────────────────────────────────────
    public const string ConsultationsRead   = "consultations:read";
    public const string ConsultationsCreate = "consultations:create";
    public const string ConsultationsUpdate = "consultations:update";

    // ─── Audits ───────────────────────────────────────────────────────────────
    public const string AuditLogsRead = "audit-logs:read";

    // ─── Dashboard / Analytics ───────────────────────────────────────────────
    public const string DashboardRead = "dashboard:read";

    // ─────────────────────────────────────────────────────────────────────────
    // Grouped for seeding and UI display
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>All permissions with metadata for seeding.</summary>
    public static readonly PermissionDefinition[] All =
    [
        // Users
        new(UsersRead,         "Read Users",         "View user list and profiles",                   "Users"),
        new(UsersCreate,       "Create Users",       "Create new user accounts",                      "Users"),
        new(UsersUpdate,       "Update Users",       "Edit user profiles and account info",            "Users"),
        new(UsersDelete,       "Delete Users",       "Soft-delete (deactivate) user accounts",         "Users"),
        new(UsersManageRoles,  "Manage User Roles",  "Assign or remove roles from users",              "Users"),

        // Permissions
        new(PermissionsRead,   "Read Permissions",   "View permissions and role assignments",          "Permissions"),
        new(PermissionsManage, "Manage Permissions", "Create, update, and assign permissions to roles","Permissions"),

        // Patients
        new(PatientsRead,   "Read Patients",   "View patient profiles and medical history",  "Patients"),
        new(PatientsCreate, "Create Patients", "Create patient records",                     "Patients"),
        new(PatientsUpdate, "Update Patients", "Edit patient profiles",                      "Patients"),
        new(PatientsDelete, "Delete Patients", "Soft-delete patient records",                "Patients"),

        // Ophthalmologists
        new(OphthalmologistsRead,   "Read Ophthalmologists",   "View ophthalmologist profiles",      "Ophthalmologists"),
        new(OphthalmologistsCreate, "Create Ophthalmologists", "Register new ophthalmologist accounts","Ophthalmologists"),
        new(OphthalmologistsUpdate, "Update Ophthalmologists", "Edit ophthalmologist profiles",       "Ophthalmologists"),
        new(OphthalmologistsVerify, "Verify Ophthalmologists", "Approve or reject credential verification","Ophthalmologists"),

        // Organisations
        new(OrganisationsRead,   "Read Organisations",   "View organisation list and details",  "Organisations"),
        new(OrganisationsCreate, "Create Organisations", "Register new organisations",          "Organisations"),
        new(OrganisationsUpdate, "Update Organisations", "Edit organisation information",       "Organisations"),
        new(OrganisationsDelete, "Delete Organisations", "Deactivate organisations",            "Organisations"),

        // Screening
        new(ScreeningRead,    "Read Screening",    "View retinal screening results",          "Screening"),
        new(ScreeningCreate,  "Create Screening",  "Submit retinal images for screening",     "Screening"),
        new(ScreeningApprove, "Approve Screening", "Approve or flag AI screening results",    "Screening"),

        // Consultations
        new(ConsultationsRead,   "Read Consultations",   "View consultation sessions",          "Consultations"),
        new(ConsultationsCreate, "Create Consultations", "Book new consultation sessions",      "Consultations"),
        new(ConsultationsUpdate, "Update Consultations", "Modify or cancel consultation sessions","Consultations"),

        // Audit
        new(AuditLogsRead, "Read Audit Logs", "View system audit trail",                      "Audit"),

        // Dashboard
        new(DashboardRead, "Read Dashboard", "View admin dashboard metrics and analytics",    "Dashboard"),
    ];

    /// <summary>
    /// Default permissions seeded per role (RBAC baseline).
    /// This defines which permissions each role gets out of the box.
    /// Admins can change these at runtime via the Permissions API.
    /// </summary>
    public static readonly Dictionary<string, string[]> DefaultRolePermissions = new()
    {
        [Roles.SystemAdmin] =
        [
            UsersRead, UsersCreate, UsersUpdate, UsersDelete, UsersManageRoles,
            PermissionsRead, PermissionsManage,
            PatientsRead, PatientsCreate, PatientsUpdate, PatientsDelete,
            OphthalmologistsRead, OphthalmologistsCreate, OphthalmologistsUpdate, OphthalmologistsVerify,
            OrganisationsRead, OrganisationsCreate, OrganisationsUpdate, OrganisationsDelete,
            ScreeningRead, ScreeningCreate, ScreeningApprove,
            ConsultationsRead, ConsultationsCreate, ConsultationsUpdate,
            AuditLogsRead,
            DashboardRead,
        ],

        [Roles.OrgAdmin] =
        [
            UsersRead,
            PatientsRead, PatientsUpdate,
            OphthalmologistsRead, OphthalmologistsCreate, OphthalmologistsUpdate,
            OrganisationsRead, OrganisationsUpdate,
            ScreeningRead,
            ConsultationsRead, ConsultationsUpdate,
            DashboardRead,
        ],

        [Roles.Ophthalmologist] =
        [
            PatientsRead,
            ScreeningRead, ScreeningApprove,
            ConsultationsRead, ConsultationsUpdate,
        ],

        [Roles.Patient] =
        [
            ScreeningRead, ScreeningCreate,
            ConsultationsRead, ConsultationsCreate,
        ],
    };
}

/// <summary>Seed metadata for a single permission.</summary>
public record PermissionDefinition(
    string Name,
    string DisplayName,
    string Description,
    string Category);
