namespace Domain.Enums;

/// <summary>
/// Role assigned to a staff member for a specific appointment slot.
/// Maps current ClinicStaffRole taxonomy to scheduling context.
/// </summary>
public enum SlotAssignmentRole
{
    /// <summary>Ophthalmologist assigned to see patients in this slot.</summary>
    Doctor = 1,

    /// <summary>Technician operating screening equipment (maps from ClinicStaffRole.Coordinator).</summary>
    Technician = 2,

    /// <summary>Receptionist handling check-in for this slot.</summary>
    Receptionist = 3
}
