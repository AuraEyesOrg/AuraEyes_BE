namespace Domain.Enums;

/// <summary>
/// Clinic staff sub-role used for internal permission differentiation within the ClinicStaff role.
/// A single staff member may hold multiple sub-roles (e.g. Receptionist + Cashier).
/// </summary>
public enum ClinicStaffRole
{
    /// <summary>Handles patient check-in and appointment scheduling.</summary>
    Receptionist = 1,

    /// <summary>Takes retinal images and operates the AI screening pipeline.</summary>
    Coordinator = 2,

    /// <summary>Processes billing, medication pricing, and receives payment.</summary>
    Cashier = 3
}
