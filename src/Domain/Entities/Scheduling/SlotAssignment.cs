using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Scheduling;

/// <summary>
/// SlotAssignment - Assigns a staff member to a specific appointment slot with a role.
/// Enables tracking which doctors/technicians/receptionists are assigned to which slots.
/// </summary>
public class SlotAssignment : BaseEntity, IAggregateRoot
{
    /// <summary>FK to AppointmentSlot.</summary>
    public Guid AppointmentSlotId { get; private set; }

    /// <summary>FK to ClinicStaff (staff member assigned to this slot).</summary>
    public Guid StaffId { get; private set; }

    /// <summary>Role of the staff member for this slot assignment.</summary>
    public SlotAssignmentRole Role { get; private set; }

    // Navigation
    public AppointmentSlot? AppointmentSlot { get; private set; }

    private SlotAssignment() { } // EF Core

    public SlotAssignment(
        Guid appointmentSlotId,
        Guid staffId,
        SlotAssignmentRole role)
    {
        if (appointmentSlotId == Guid.Empty)
            throw new ArgumentException("Appointment slot ID is required.", nameof(appointmentSlotId));
        if (staffId == Guid.Empty)
            throw new ArgumentException("Staff ID is required.", nameof(staffId));

        AppointmentSlotId = appointmentSlotId;
        StaffId = staffId;
        Role = role;
    }

    /// <summary>
    /// Update the role of this assignment.
    /// </summary>
    public void UpdateRole(SlotAssignmentRole newRole)
    {
        Role = newRole;
        UpdatedAt = DateTime.UtcNow;
    }
}
