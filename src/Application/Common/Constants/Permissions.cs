namespace Application.Common.Constants;

/// <summary>
/// Centralized permission name constants — Digital Clinic model.
///
/// Convention: "{resource}:{action}"
/// These names are seeded into the Permissions table on first startup.
/// They match what the app actually supports — do NOT create permissions here
/// unless you have an endpoint / business rule that checks for them.
///
/// Default role assignments are also seeded (see DatabaseSeeder.SeedPermissionsAsync).
/// Per-user overrides are managed at runtime via the Admin UI.
/// </summary>
public static class Permissions
{
    // ─── Users ──────────────────────────────────────────────────────────────────
    public const string UsersRead        = "users:read";
    public const string UsersCreate      = "users:create";
    public const string UsersUpdate      = "users:update";
    public const string UsersDelete      = "users:delete";
    public const string UsersManageRoles = "users:manage-roles";

    // ─── Permissions ────────────────────────────────────────────────────────────
    public const string PermissionsRead   = "permissions:read";
    public const string PermissionsManage = "permissions:manage";

    // ─── Patients ───────────────────────────────────────────────────────────────
    public const string PatientsRead   = "patients:read";
    public const string PatientsCreate = "patients:create";
    public const string PatientsUpdate = "patients:update";
    public const string PatientsDelete = "patients:delete";

    // ─── Ophthalmologists ───────────────────────────────────────────────────────
    public const string OphthalmologistsRead   = "ophthalmologists:read";
    public const string OphthalmologistsCreate = "ophthalmologists:create";
    public const string OphthalmologistsUpdate = "ophthalmologists:update";
    public const string OphthalmologistsVerify = "ophthalmologists:verify";
    public const string OphthalmologistsDelete = "ophthalmologists:delete";

    // ─── Clinic Staff ───────────────────────────────────────────────────────────
    public const string ClinicStaffRead   = "clinic-staff:read";
    public const string ClinicStaffCreate = "clinic-staff:create";
    public const string ClinicStaffUpdate = "clinic-staff:update";
    public const string ClinicStaffDelete = "clinic-staff:delete";

    // ─── Screening ──────────────────────────────────────────────────────────────
    public const string ScreeningRead    = "screening:read";
    public const string ScreeningCreate  = "screening:create";
    public const string ScreeningApprove = "screening:approve";

    // ─── Consultations ──────────────────────────────────────────────────────────
    public const string ConsultationsRead   = "consultations:read";
    public const string ConsultationsCreate = "consultations:create";
    public const string ConsultationsUpdate = "consultations:update";

    // ─── Appointments & Scheduling ──────────────────────────────────────────────
    public const string AppointmentsRead   = "appointments:read";
    public const string AppointmentsCreate = "appointments:create";
    public const string AppointmentsManage = "appointments:manage";
    public const string ApptSlotsManage    = "appt-slots:manage";
    public const string SchedulesManage    = "schedules:manage";

    // ─── Visit Records ──────────────────────────────────────────────────────────
    public const string VisitsRead   = "visits:read";
    public const string VisitsManage = "visits:manage";

    // ─── Medical Records (EMR 23/BV-01) ──────────────────────────────────────────
    public const string MedicalRecordsRead     = "medical-records:read";
    public const string MedicalRecordsCreate   = "medical-records:create";
    public const string MedicalRecordsUpdate   = "medical-records:update";
    public const string MedicalRecordsFinalize = "medical-records:finalize";

    // ─── Orders & Billing ───────────────────────────────────────────────────────
    public const string OrdersRead   = "orders:read";
    public const string OrdersManage = "orders:manage";

    // ─── Payments ───────────────────────────────────────────────────────────────
    public const string PaymentsRead   = "payments:read";
    public const string PaymentsManage = "payments:manage";

    // ─── Platform & Models ─────────────────────────────────────────────────────
    public const string AiModelsRead   = "aimodels:read";
    public const string AiModelsManage = "aimodels:manage";
    public const string NetworkManage  = "network:manage";
    public const string FeedbackRead   = "feedback:read";
    public const string RoadmapsManage = "roadmaps:manage";

    // ─── Settings ───────────────────────────────────────────────────────────────
    public const string SettingsRead   = "settings:read";
    public const string SettingsManage = "settings:manage";

    // ─── Notifications ──────────────────────────────────────────────────────────
    public const string NotificationsManage = "notifications:manage";

    // ─── Audit ──────────────────────────────────────────────────────────────────
    public const string AuditLogsRead = "audit-logs:read";

    // ─── Dashboard ──────────────────────────────────────────────────────────────
    public const string DashboardRead = "dashboard:read";

    // ────────────────────────────────────────────────────────────────────────────
    // Grouped for seeding and UI display
    // ────────────────────────────────────────────────────────────────────────────

    /// <summary>All permissions with metadata for seeding.</summary>
    public static readonly PermissionDefinition[] All =
    [
        // Users
        new(UsersRead,        "Read Users",          "View user list and profiles",                   "Users"),
        new(UsersCreate,      "Create Users",         "Create new user accounts",                      "Users"),
        new(UsersUpdate,      "Update Users",         "Edit user profiles and account info",            "Users"),
        new(UsersDelete,      "Delete Users",         "Soft-delete (deactivate) user accounts",         "Users"),
        new(UsersManageRoles, "Manage User Roles",    "Assign or remove roles from users",              "Users"),

        // Permissions
        new(PermissionsRead,   "Read Permissions",   "View permissions and role assignments",           "Permissions"),
        new(PermissionsManage, "Manage Permissions", "Create, update, and assign permissions to roles", "Permissions"),

        // Patients
        new(PatientsRead,   "Read Patients",   "View patient profiles and medical history",  "Patients"),
        new(PatientsCreate, "Create Patients", "Create patient records",                     "Patients"),
        new(PatientsUpdate, "Update Patients", "Edit patient profiles",                      "Patients"),
        new(PatientsDelete, "Delete Patients", "Soft-delete patient records",                "Patients"),

        // Ophthalmologists
        new(OphthalmologistsRead,   "Read Ophthalmologists",   "View ophthalmologist profiles",          "Ophthalmologists"),
        new(OphthalmologistsCreate, "Create Ophthalmologists", "Register new ophthalmologist accounts",   "Ophthalmologists"),
        new(OphthalmologistsUpdate, "Update Ophthalmologists", "Edit ophthalmologist profiles",           "Ophthalmologists"),
        new(OphthalmologistsVerify, "Verify Ophthalmologists", "Approve or reject credential verification","Ophthalmologists"),
        new(OphthalmologistsDelete, "Delete Ophthalmologists", "Soft-delete ophthalmologist profiles",    "Ophthalmologists"),

        // Clinic Staff
        new(ClinicStaffRead,   "Read Clinic Staff",   "View clinic staff profiles",                   "ClinicStaff"),
        new(ClinicStaffCreate, "Create Clinic Staff", "Onboard new clinic staff members",             "ClinicStaff"),
        new(ClinicStaffUpdate, "Update Clinic Staff", "Edit clinic staff profiles and sub-roles",     "ClinicStaff"),
        new(ClinicStaffDelete, "Delete Clinic Staff", "Deactivate clinic staff accounts",             "ClinicStaff"),

        // Screening
        new(ScreeningRead,    "Read Screening",    "View retinal screening results",          "Screening"),
        new(ScreeningCreate,  "Create Screening",  "Submit retinal images for screening",     "Screening"),
        new(ScreeningApprove, "Approve Screening", "Approve or flag AI screening results",    "Screening"),

        // Consultations & Appointments
        new(ConsultationsRead,   "Read Consultations",   "View consultation sessions",             "Consultations"),
        new(ConsultationsCreate, "Create Consultations", "Book new consultation sessions",         "Consultations"),
        new(ConsultationsUpdate, "Update Consultations", "Modify or cancel consultation sessions", "Consultations"),
        new(AppointmentsRead,    "Read Appointments",    "View clinic appointments",               "Appointments"),
        new(AppointmentsCreate,  "Create Appointments",  "Book clinic appointments",               "Appointments"),
        new(AppointmentsManage,  "Manage Appointments",  "Check-in, reschedule or cancel",         "Appointments"),

        // Scheduling
        new(ApptSlotsManage, "Manage Time Slots", "Generate or manage appointment slots",     "Scheduling"),
        new(SchedulesManage, "Manage Templates",  "Manage ophthalmologist weekly schedules",  "Scheduling"),

        // Visits
        new(VisitsRead,   "Read Visits",   "View clinic visit records",                    "Visits"),
        new(VisitsManage, "Manage Visits", "Check-in patients, assign coordinators",       "Visits"),

        // Orders & Billing
        new(OrdersRead,   "Read Orders",   "View patient orders and billing details",      "Orders"),
        new(OrdersManage, "Manage Orders", "Create and update orders (drugs, services)",   "Orders"),

        // Payments
        new(PaymentsRead,   "Read Payments",   "View payment records",                   "Payments"),
        new(PaymentsManage, "Manage Payments", "Process patient payments at the clinic",  "Payments"),

        // Platform
        new(AiModelsRead,   "Read AI Models",  "View available AI screening models",        "Platform"),
        new(AiModelsManage, "Manage AI Models","Update model metadata or active status",    "Platform"),
        new(NetworkManage,  "Manage Network",  "Manage health network and affiliations",    "Platform"),
        new(FeedbackRead,   "Read Feedback",   "View user feedback and survey results",     "Platform"),
        new(RoadmapsManage, "Manage Roadmaps", "Curate patient health journey roadmaps",    "Platform"),

        // Settings
        new(SettingsRead,   "Read Settings",   "View system settings",             "Settings"),
        new(SettingsManage, "Manage Settings", "Modify system-wide settings",      "Settings"),

        // Notifications
        new(NotificationsManage, "Manage Notifications", "Send system-wide broadcast notifications", "Notifications"),

        // Audit
        new(AuditLogsRead, "Read Audit Logs", "View system audit trail", "Audit"),
        
        // Medical Records
        new(MedicalRecordsRead,     "Read Medical Records",     "View EMR 23/BV-01 patient records",           "MedicalRecords"),
        new(MedicalRecordsCreate,   "Create Medical Records",   "Initialize new 23/BV-01 medical records",     "MedicalRecords"),
        new(MedicalRecordsUpdate,   "Update Medical Records",   "Fill clinical pathology and diagnosis",       "MedicalRecords"),
        new(MedicalRecordsFinalize, "Finalize Medical Records", "Lock records for billing and legal archiving","MedicalRecords"),

        // Dashboard
        new(DashboardRead, "Read Dashboard", "View dashboard metrics and analytics", "Dashboard"),
    ];

    /// <summary>
    /// Default permissions seeded per role (RBAC baseline).
    /// Fine-grained overrides per staff member are managed via UserPermission table.
    /// </summary>
    public static readonly Dictionary<string, string[]> DefaultRolePermissions = new()
    {
        [Roles.SystemAdmin] =
        [
            UsersRead, UsersCreate, UsersUpdate, UsersDelete, UsersManageRoles,
            PermissionsRead, PermissionsManage,
            PatientsRead, PatientsCreate, PatientsUpdate, PatientsDelete,
            OphthalmologistsRead, OphthalmologistsCreate, OphthalmologistsUpdate,
            OphthalmologistsVerify, OphthalmologistsDelete,
            ClinicStaffRead, ClinicStaffCreate, ClinicStaffUpdate, ClinicStaffDelete,
            ScreeningRead, ScreeningCreate, ScreeningApprove,
            ConsultationsRead, ConsultationsCreate, ConsultationsUpdate,
            AppointmentsRead, AppointmentsCreate, AppointmentsManage,
            ApptSlotsManage, SchedulesManage,
            VisitsRead, VisitsManage,
            MedicalRecordsRead, MedicalRecordsCreate, MedicalRecordsUpdate, MedicalRecordsFinalize,
            OrdersRead, OrdersManage,
            PaymentsRead, PaymentsManage,
            AiModelsRead, AiModelsManage, NetworkManage, FeedbackRead, RoadmapsManage,
            SettingsRead, SettingsManage,
            NotificationsManage,
            AuditLogsRead,
            DashboardRead,
        ],

        // ─── Ophthalmologist — medical work + own scheduling —────────────────────────
        [Roles.Ophthalmologist] =
        [
            DashboardRead,
            PatientsRead, PatientsUpdate, PatientsCreate,
            OphthalmologistsUpdate,
            ScreeningRead, ScreeningCreate, ScreeningApprove,
            ConsultationsRead, ConsultationsCreate, ConsultationsUpdate,
            AppointmentsRead, AppointmentsCreate,
            SchedulesManage, ApptSlotsManage,
            VisitsRead,
            MedicalRecordsRead, MedicalRecordsUpdate, MedicalRecordsFinalize,
            SettingsRead,
        ],

        // ─── ClinicStaff baseline — all sub-roles get these ──────────────────────────
        [Roles.ClinicStaff] =
        [
            DashboardRead,
            SettingsRead,
        ],

        // ─── Patient ────────────────────────────────────────────────────────────────
        [Roles.Patient] =
        [
            DashboardRead,
            ScreeningRead, ScreeningCreate,
            ConsultationsRead, ConsultationsCreate,
            AppointmentsRead, AppointmentsCreate,
            PatientsRead, PatientsUpdate,
            MedicalRecordsRead,
            OrdersRead, PaymentsRead,
            SettingsRead,
        ],
    };

    // ─── Convenience permission groups for sub-role assignment ──────────────────────

    /// <summary>Extra permissions assigned to a Receptionist sub-role.</summary>
    public static readonly string[] ReceptionistExtras =
    [
        PatientsRead, PatientsCreate, PatientsUpdate,
        AppointmentsRead, AppointmentsCreate, AppointmentsManage,
        OrdersRead, PaymentsRead,
    ];

    /// <summary>Extra permissions assigned to a Coordinator sub-role.</summary>
    public static readonly string[] CoordinatorExtras =
    [
        PatientsRead,
        ScreeningRead, ScreeningCreate, ScreeningApprove,
        MedicalRecordsRead, MedicalRecordsCreate, MedicalRecordsFinalize,
        VisitsRead, VisitsManage,
        DashboardRead,
    ];

    /// <summary>Extra permissions assigned to a Cashier sub-role.</summary>
    public static readonly string[] CashierExtras =
    [
        PatientsRead,
        AppointmentsRead, AppointmentsManage,
        OrdersRead, OrdersManage,
        PaymentsRead, PaymentsManage,
    ];
}

/// <summary>Seed metadata for a single permission.</summary>
public record PermissionDefinition(
    string Name,
    string DisplayName,
    string Description,
    string Category);
